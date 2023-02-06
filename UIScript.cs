using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public NewPlayer charlieStats;
    public GameManager gM;

    //Inventory
    public GameObject inventoryDisplay;
    public bool displayOn;
    // Start is called before the first frame update
    void Start()
    {
        displayOn = false;
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Inventory
        if (Input.GetKeyDown(KeyCode.I)) displayOn = !displayOn;
        if (displayOn)
        {
            gM.gameState = GameManager.AllGameStates.Pause;
            inventoryDisplay.SetActive(true);
            Time.timeScale = 0;
        }
        if (!displayOn)
        {
            gM.gameState = GameManager.AllGameStates.Play;
            inventoryDisplay.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
