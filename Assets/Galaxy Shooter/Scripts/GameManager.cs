using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public bool gameOver = true;
    public GameObject player;

    private UIManager uiManager;
    
    // Use this for initialization
	void Start ()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(gameOver)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Instantiate(player, Vector3.zero, Quaternion.identity);
                gameOver = false;
                uiManager.HideTitleScreen();
            }
        }
	}

    public void GameOver()
    {
        this.gameOver = true;
    }
}
