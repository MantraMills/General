using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimb : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Charlie")
        {
            print("hi");
            //Vector3(14.3999996,16.75,21.1599998) <- on the ledge
            GameObject mainBody = other.gameObject;
            player = mainBody;
            if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(Climbing());
        }
        else
        {
            StopCoroutine(Climbing());
        }
    }

    IEnumerator Climbing()
    {
        //yield return new WaitUntil(() => charlie._anim.GetBool("isClimbing") == true);
        yield return new WaitForSeconds(1f);
        player.transform.position = new Vector3(player.transform.position.x, this.transform.position.y, this.transform.position.z);
        print("start");
    }
}
