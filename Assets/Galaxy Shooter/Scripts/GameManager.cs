using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //gameOver always false
    public bool gameOver = false;
    public GameObject player;

    private UIManager uiManager;
    private Spawn_Manager spawn_manager;

    // Use this for initialization
	void Start ()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        spawn_manager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        //hide screen auto
        uiManager.HideTitleScreen();
    }
	
	// Update is called once per frame
	void Update ()
    {
        /*
        if (gameOver)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Instantiate(player, Vector3.zero, Quaternion.identity);
                spawn_manager.ResetHardLvlVars();
                gameOver = false;
                uiManager.HideTitleScreen();
            }
        } */  
    }

    public void GameOver()
    {
        this.gameOver = true;
    }
}
