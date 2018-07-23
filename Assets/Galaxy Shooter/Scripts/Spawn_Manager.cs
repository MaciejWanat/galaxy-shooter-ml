using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour {

    [SerializeField]
    private GameObject enemyShipPrefab;
    [SerializeField]
    private GameObject[] powerUps;
    private GameManager gameManager;

    private UIManager uiManager;

    [SerializeField]
    private float DifficultyUpInterval = 5.0f;
    [SerializeField]
    private float DifficultyRaiseTempo = 1.0f;
    private float EnemySpawnInterval = 6.0f;
    private int Difficulty = 0;

    private GameObject Player;
    
    // Use this for initialization
    void Start()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Player = GameObject.FindGameObjectWithTag("Player");
        //override
        gameManager.gameOver = false;
        StartSpawnRoutines();
    }

    public void StartSpawnRoutines()
    {
        StartCoroutine(EnemySpawnRoutine());
        //Disable power ups for now
        //StartCoroutine(PowerUpSpawnRoutine());
        StartCoroutine(SpiceUpTheTempoRoutine());
    }

    IEnumerator EnemySpawnRoutine()
    {
        while (gameManager.gameOver == false)
        {
            Instantiate(enemyShipPrefab, new Vector3(Random.Range(-7f, 7f), 7, 0), Quaternion.identity);
            //Spawn enemy at players face - to teach ML to avoid it
            //Instantiate(enemyShipPrefab, new Vector3(Player.transform.position.x, 7, 0), Quaternion.identity);
            yield return new WaitForSeconds(EnemySpawnInterval);
        }
    }

    IEnumerator PowerUpSpawnRoutine()
    {
        while(gameManager.gameOver == false)
        {
            int randomPowerUp = Random.Range(0, 3);
            Instantiate(powerUps[randomPowerUp], new Vector3(Random.Range(-7f, 7f), 7, 0), Quaternion.identity);
            int randomCooldown = Random.Range(0, 21);
            yield return new WaitForSeconds(randomCooldown);
        }
    }

    IEnumerator SpiceUpTheTempoRoutine()
    {
        while (gameManager.gameOver == false)
        {
            SpiceUpTheTempo();
            yield return new WaitForSeconds(DifficultyUpInterval);
        }
    }

    private void SpiceUpTheTempo()
    {         
        if(EnemySpawnInterval > 0)
        {
            Difficulty++;
            EnemySpawnInterval -= DifficultyRaiseTempo / Difficulty;
            uiManager.UpdateDifficulty(Difficulty);
        }
    }

    public void ResetHardLvlVars()
    {
        EnemySpawnInterval = 5.0f;
        Difficulty = 0;
    }
}

