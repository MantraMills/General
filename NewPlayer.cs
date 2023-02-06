using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayer : MonoBehaviour
{
    [Header("GameObject References")]
    private CharacterController characterController;
    private Animator anim;
    public InventoryManager inventoryManager;
    public SoundManager charlieSoundManager;

    [Header("Script References")]
    public GameManager gM;

    [Header("Player Stats")]
    float speed;
    public float walkSpeed;
    public float runSpeed;
    public float health;
    public float maxHealth = 100;

    [Header("Player Bools")]
    public bool moving;
    public bool canMove = true;
    public bool isPickingUp;

    [Header("Other")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public bool sprintDisabled = false;

    [Header("Jump")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] float jumpHeight;
    public bool jump;
    Vector3 verticalVelocity = Vector3.zero;
    [SerializeField] LayerMask groundMask;
    public bool isGrounded;
    public bool isJumping;
    public Animation jumpStart;

    [Header("Stamina")]
    public bool canSprint = true;
    public bool isSprinting;
    public bool regenerating = false;
    public float currentStamina = 100;
    public float maxStamina = 100;
    public float staminaUseMultiplier = 5;
    public float timeBeforeStaminaRegens;
    public float normalRegen;
    public float fatiguedRegen;
    public float staminaValue = 2;
    public float staminaTime = 0.1f;
    private Coroutine regenStamina;
    
    // Start is called before the first frame update
    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //General Movement
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(x, y);

        if(health > maxHealth) health = maxHealth;

        if (gM.gameState == GameManager.AllGameStates.Play)
        {
            if (canMove)
            {
                Walk(dir);
                Stamina(dir);

                if (dir.magnitude >= 0.1f)
                {
                    if (!Input.GetKey(KeyCode.LeftShift) && isGrounded && !jump)
                    {
                        charlieSoundManager.Play("charlie_footsteps");
                        anim.SetBool("isWalking", true);
                    }
                    moving = true;
                }
                else
                {
                    moving = false;
                    charlieSoundManager.Stop("charlie_footsteps");
                }

                if (dir.magnitude == 0f)
                {
                    anim.SetBool("isWalking", false);
                }

                if (Input.GetKey(KeyCode.LeftShift) && moving && canSprint && !regenerating && !sprintDisabled && currentStamina > 0)
                {
                    Run(dir);
                    isSprinting = true;
                    anim.SetBool("isRunning", true);

                    if (isGrounded && !jump && !isJumping)
                    {
                        charlieSoundManager.Stop("charlie_footsteps");
                        charlieSoundManager.Play("charlie_sprint");
                    }
                    else
                    {
                        charlieSoundManager.Stop("charlie_sprint");
                    }
                }
                else
                {
                    charlieSoundManager.Stop("charlie_sprint");
                    anim.SetBool("isRunning", false);
                    isSprinting = false;
                }

                //Look Direction

                float xRaw = Input.GetAxisRaw("Horizontal");
                float yRaw = Input.GetAxisRaw("Vertical");
                Vector3 direction = new Vector3(xRaw, 0, yRaw).normalized;

                if (direction.magnitude >= 0.1f)
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                    transform.rotation = Quaternion.Euler(0, angle, 0);
                }

                //Jump

                isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);
                if (isGrounded)
                {
                    verticalVelocity.y = 0;
                }

                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                {
                    StartCoroutine(JumpEnum());
                }

                if (jump)
                {
                    if (isGrounded)
                    {
                        verticalVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);
                    }
                    jump = false;
                }

                if (isPickingUp)
                {
                    StartCoroutine(ItemPickup());
                }

                verticalVelocity.y += gravity * Time.deltaTime;
                characterController.Move(verticalVelocity * Time.deltaTime);
            }

            if (!canMove)
            {
                speed = 0;
            }

            //Stamina Regen

            if (regenerating)
            {
                canSprint = false;
            }
            else canSprint = true;  

            if(regenStamina == null)
            {
                regenerating = false;
            }

            if (health <= 0)
            {
                gM.PlayerDeath();
            }
        }
        else
        {
            charlieSoundManager.Stop("charlie_footsteps");
            charlieSoundManager.Stop("charlie_sprint");
        }
    }

    void Walk(Vector3 dir)
    {
        speed = walkSpeed;
        Vector3 move = new Vector3(dir.x, 0, dir.y);
        characterController.Move(move * Time.deltaTime * speed);
    }

    void Run(Vector3 dir)
    {
        speed = runSpeed;
        Vector3 move = new Vector3(dir.x, 0, dir.y);
        characterController.Move(move * Time.deltaTime * speed);
    }

    IEnumerator JumpEnum()
    {
        anim.SetBool("jumpStart", true);
        isJumping = true;
        yield return new WaitForSeconds(0.5f);
        jump = true;
        anim.SetBool("jumpStart", false);
        anim.SetBool("jumpMid", true);
        yield return new WaitWhile(() => isGrounded);
        yield return new WaitUntil(() => isGrounded);
        characterController.Move(Vector3.zero);
        anim.SetBool("jumpMid", false);
        charlieSoundManager.PlayOne("charlie_jump");
        anim.SetBool("jumpEnd", true);
        canMove = false;
        yield return new WaitForSeconds(1f);
        characterController.Move(Vector3.zero);
        anim.SetBool("jumpEnd", false);
        isJumping = false;
        canMove = true;
    }

    IEnumerator ItemPickup()
    {
        anim.SetBool("isPickingUp", true);
        characterController.Move(Vector3.zero);
        canMove = false;
        yield return new WaitForSeconds(1.25f);
        anim.SetBool("isPickingUp", false);
        canMove = true;
        isPickingUp = false;
    }

    void Stamina(Vector3 dir)
    {
        if (isSprinting && dir.magnitude != 0)
        {
            if(regenStamina != null)
            {
                StopCoroutine(regenStamina);
                regenStamina = null;
            }

            currentStamina -= staminaUseMultiplier * Time.deltaTime;

            if(currentStamina < 0)
            {
                currentStamina = 0;
            }

            if(currentStamina <= 0)
            {
                canSprint = false;
                isSprinting = false;
            }
        }

        if(!isSprinting && currentStamina < maxStamina && regenStamina == null)
        {
            regenStamina = StartCoroutine(RegenStamina());
        }
    }

    IEnumerator RegenStamina()
    {
        if (currentStamina <= 0)
        {
            timeBeforeStaminaRegens = fatiguedRegen;
        }
        else timeBeforeStaminaRegens = normalRegen;

        yield return new WaitForSeconds(timeBeforeStaminaRegens);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaTime);
        speed = walkSpeed;

        while (currentStamina < maxStamina)
        {
            currentStamina += staminaValue;
            regenerating = true;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }

            yield return timeToWait;
        }

        regenStamina = null;
    }

    //Add Items to Inventory
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Item"))
        {
            int arrowAmount = other.gameObject.GetComponent<ItemCount>().amount;

            if (other.gameObject.tag == "Red Arrow") inventoryManager.arrowCount[0] = inventoryManager.arrowCount[0] + arrowAmount;
            if (other.gameObject.tag == "Blue Arrow") inventoryManager.arrowCount[1] = inventoryManager.arrowCount[1] + arrowAmount;
            if (other.gameObject.tag == "Yellow Arrow") inventoryManager.arrowCount[2] = inventoryManager.arrowCount[2] + arrowAmount;
            if (other.gameObject.tag == "Green Arrow") inventoryManager.arrowCount[3] = inventoryManager.arrowCount[3] + arrowAmount;
            if (other.gameObject.tag == "Purple Arrow") inventoryManager.arrowCount[4] = inventoryManager.arrowCount[4] + arrowAmount;

            Destroy(other.gameObject);

            isPickingUp = true;
        }
    }
}
