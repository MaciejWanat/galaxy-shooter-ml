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
    private SpaceShipAgent spaceShipAgent;
    
    // Use this for initialization
    void Start()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Player = GameObject.FindGameObjectWithTag("Player");
        spaceShipAgent = Player.GetComponent<SpaceShipAgent>();
        //override
        gameManager.gameOver = false;

        StartSpawnRoutines();
    }

    public void StartSpawnRoutines()
    {
        StartCoroutine(EnemySpawnRoutine());
        //StartCoroutine(PowerUpSpawnRoutine()); 
        StartCoroutine(SpiceUpTheTempoRoutine());
    }

    public void SpawnEnemy()
    {
        //spawn one enemy
        var enemy = Instantiate(enemyShipPrefab, new Vector3(Random.Range(-3.5f, 3.5f), 7, 0), Quaternion.identity);
        //spaceShipAgent.AddEnemyToInterval(enemy);
    }

    IEnumerator EnemySpawnRoutine()
    {
        while (gameManager.gameOver == false)
        {
            /*
            //Spawn enemy at players face from time to time - to teach ML to avoid it
            //int inYourFace = Random.Range(0, 3);

            if(inYourFace == 0)
            {
                var enemy = Instantiate(enemyShipPrefab, new Vector3(Player.transform.position.x, 7, 0), Quaternion.identity);
            }
            else
            {
                var enemy = Instantiate(enemyShipPrefab, new Vector3(Random.Range(-3.5f, 3.5f), 7, 0), Quaternion.identity);
            }
            */
            var enemy = Instantiate(enemyShipPrefab, new Vector3(Random.Range(-3.5f, 3.5f), 7, 0), Quaternion.identity);

            yield return new WaitForSeconds(EnemySpawnInterval);
        }
    }

    IEnumerator PowerUpSpawnRoutine()
    {
        while(gameManager.gameOver == false)
        {
            //0 - 2 range -> disable speedup, because it messes hardly with the neural network input
            int randomPowerUp = Random.Range(0, 2);
            Instantiate(powerUps[randomPowerUp], new Vector3(Random.Range(-3.5f, 3.5f), 7, 0), Quaternion.identity);
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
        uiManager.UpdateDifficulty(Difficulty);
    }
}

