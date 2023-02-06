using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class ProjectionLine : MonoBehaviour
{
    public enum state { aimMode, standBy, fire }
    public state playerState;

    public Camera mainCamera;
    public GameObject crosshair;
    public WeaponWheel _weaponWheel;
    public InventoryManager inventoryManager;
    public SoundManager bowSounds;

    //Arrow
    public List<GameObject> arrows;
    public int arrowType;
    public GameObject bow;
    public float shootForce, upwardForce, endLagTime;
    public Vector3 distance;

    public TextMeshProUGUI arrowText;
    public bool textFading;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        playerState = state.standBy;
        arrowText.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == state.standBy)
        {
            Cursor.visible = true;
            crosshair.SetActive(false);

            if (Input.GetMouseButtonDown(1) && _weaponWheel.menuActive == false)
            {
                playerState = state.aimMode;
            }
        }

        if (playerState == state.aimMode)
        {
            Cursor.visible = true;
            crosshair.SetActive(true);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                crosshair.transform.position = raycastHit.point;
            }
            distance = crosshair.transform.position - bow.transform.position;
            
            if (Input.GetMouseButtonDown(0)) CheckAvailability();
            //combine Charlie's Head through Transform.LookAt with crosshair 
        }

        if (playerState == state.fire)
        {
            Cursor.visible = false;
            GameObject currentArrow = Instantiate(arrows[arrowType], bow.transform.transform.position, Quaternion.identity);
            Physics.IgnoreCollision(currentArrow.GetComponent<Collider>(), GetComponent<Collider>());
            currentArrow.transform.forward = distance.normalized;
            currentArrow.GetComponent<Rigidbody>().AddForce(distance.normalized * shootForce, ForceMode.Impulse);
            bowSounds.PlayOne("bow_fire");
            playerState = state.standBy;
        }

        if (textFading)
        {
            arrowText.alpha -= 0.01f;

            if(arrowText.alpha <= 0)
            {
                textFading = false;
                arrowText.text = "";
            }
        }
    }

    void CheckAvailability()
    {
        if (arrowType == 0)
        {
            if (inventoryManager.arrowCount[0] == 0)
            {
                playerState = state.standBy;
                StartCoroutine(ArrowText());
                arrowText.text = "NO FIRE ARROWS LEFT";
            }
            else
            {
                inventoryManager.arrowCount[0]--;
                playerState = state.fire;
            }
        }
        if (arrowType == 1)
        {
            if (inventoryManager.arrowCount[1] == 0)
            {
                playerState = state.standBy;
                StartCoroutine(ArrowText());
                arrowText.text = "NO ICE ARROWS LEFT";
            }
            else
            {
                inventoryManager.arrowCount[1]--;
                playerState = state.fire;
            }
        }
        if (arrowType == 2)
        {
            if (inventoryManager.arrowCount[2] == 0)
            {
                playerState = state.standBy;
                StartCoroutine(ArrowText());
                arrowText.text = "NO ELECTRIC ARROWS LEFT";
            } 
            else
            {
                inventoryManager.arrowCount[2]--;
                playerState = state.fire;
            }
        }
        if (arrowType == 3)
        {
            if (inventoryManager.arrowCount[3] == 0)
            {
                playerState = state.standBy;
                StartCoroutine(ArrowText());
                arrowText.text = "NO ACID ARROWS LEFT";
            }
            else
            {
                inventoryManager.arrowCount[3]--;
                playerState = state.fire;
            }
        }
        if (arrowType == 4)
        {
            if (inventoryManager.arrowCount[4] == 0)
            {
                playerState = state.standBy;
                StartCoroutine(ArrowText());
                arrowText.text = "NO BOUNCE ARROWS LEFT";
            }
            else
            {
                inventoryManager.arrowCount[4]--;
                playerState = state.fire;
            }
        }
    }

    IEnumerator ArrowText()
    {
        arrowText.alpha = 1;
        textFading = true;
        yield return new WaitForSeconds(3f);       
    }
}
