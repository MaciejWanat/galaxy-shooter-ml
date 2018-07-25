using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipAgent : Agent
{
    Rigidbody2D rBody;
    public GameObject Target;
    public float speed = 5;
    private float previousDistance = float.MinValue;
    private Spawn_Manager spawn_manager;
    private GameObject player;
    private UIManager uiManager;

    //map informations
    [SerializeField]
    private int mapSizeX = 14;
    [SerializeField]
    private int mapSizeY = 10;
    [SerializeField]
    private int iterationStep = 1;
    private int leftSide;
    private int rightSide;
    private int upSide;
    private int downSide;
    private Dictionary<int, int> mappedCoordsX = new Dictionary<int, int>();
    private Dictionary<int, int> mappedCoordsY = new Dictionary<int, int>();
    private int intervalArrSizeX; // mapSizeX - iterationStep
    private int intervalArrSizeY; // mapSizeY - iterationStep
    private bool[,] globalIntervalsInfo;

    private void Start()
    {
        SetCoordsMap();
        rBody = GetComponent<Rigidbody2D>();
        spawn_manager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void CollectObservations()
    {
        globalIntervalsInfo = new bool[intervalArrSizeX, intervalArrSizeY];
        ScanEnemiesInIntervals();

        foreach (var interval in globalIntervalsInfo)
        {
            AddVectorObs(interval);
        }

        AddVectorObs(IsEnemyInFront());
        AddVectorObs(gameObject.transform.position.x);
        AddVectorObs(gameObject.transform.position.y);

        int playerPostionInterval = (int)Math.Ceiling(player.transform.position.x);
        AddVectorObs(playerPostionInterval);
    }

    public override void AgentReset()
    {
        //reset interval info
        globalIntervalsInfo = new bool[intervalArrSizeX, intervalArrSizeY];

        //destroy enemies
        var EnemiesArr = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in EnemiesArr)
        {
            Destroy(enemy);
        }

        //destroy lasers
        var Lasers = GameObject.FindGameObjectsWithTag("Laser");
        uiManager.ResetScore();

        //reset difficulty
        spawn_manager.ResetHardLvlVars();

        foreach (var laser in Lasers)
        {
            Destroy(laser);
        }

        this.transform.position = new Vector3(0, 0, 0);
        this.rBody.velocity = Vector2.zero;
        this.rBody.angularVelocity = 0;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        var Lasers = GameObject.FindGameObjectsWithTag("Laser");
        var Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //Enemy hit
        foreach (var enemy in Enemies)
        {
            foreach (var laser in Lasers)
            {          
                if (CheckCollision(laser, enemy))
                {
                    AddReward(10.0f + uiManager.score);
                    uiManager.UpdateScore();
                    enemy.GetComponent<EnemyAI>().PlayExplode();                    
                    Destroy(enemy);
                    Destroy(laser);
                }
            }

            //Collided
            if (CheckCollision(enemy))
            {
                AddReward(-1.0f);
                Done();
            }
        }

        //Enemy in front of you, and you are shooting
        if(IsEnemyInFront() && vectorAction[2] > 0)
        {
            AddReward(0.05f);
        }
        else
        //Enemy isn't in front of you, and you are shooting
        if (!IsEnemyInFront() && vectorAction[2] > 0)
        {
            AddReward(-0.04f);
        }

        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.y = vectorAction[1];

        transform.Translate(controlSignal * speed * Time.deltaTime);

        if(vectorAction[2] > 0)
        {
            player.GetComponent<Player>().Shoot();
        }
    }

    private bool CheckCollision(GameObject target)
    {
        var thisCollider = gameObject.GetComponent<Collider2D>();
        var contactFiler = new ContactFilter2D { useTriggers = true };

        return thisCollider.IsTouching(target.GetComponent<Collider2D>(), contactFiler);
    }

    private bool CheckCollision(GameObject obj1, GameObject obj2)
    {
        var obj1Collider = obj1.GetComponent<Collider2D>();
        var contactFiler = new ContactFilter2D { useTriggers = true };

        return obj1Collider.IsTouching(obj2.GetComponent<Collider2D>(), contactFiler);
    }

    public void RemoveEnemyFromInterval(GameObject enemy) //this method is called when spaceship is destroyed
    {
        int enemyPostionIntervalX = (int)Math.Ceiling(enemy.transform.position.x);
        int enemyPostionIntervalY = (int)Math.Ceiling(enemy.transform.position.y);

        if (enemyPostionIntervalX > leftSide && enemyPostionIntervalX < rightSide) // X
        {
            if (enemyPostionIntervalY > downSide && enemyPostionIntervalY < upSide) // Y
            {
                enemyPostionIntervalX = mappedCoordsX[enemyPostionIntervalX];
                enemyPostionIntervalY = mappedCoordsY[enemyPostionIntervalY];
                globalIntervalsInfo[enemyPostionIntervalX, enemyPostionIntervalY] = false;
            }
        }
    }

    public void AddEnemyToInterval(GameObject enemy) //this method is called on spaceship spawn
    {        
        int enemyPostionIntervalX = (int)Math.Ceiling(enemy.transform.position.x);
        int enemyPostionIntervalY = (int)Math.Ceiling(enemy.transform.position.y);

        if (enemyPostionIntervalX > leftSide && enemyPostionIntervalX < rightSide) // X
        {
            if (enemyPostionIntervalY > downSide && enemyPostionIntervalY < upSide) // Y
            {
                enemyPostionIntervalX = mappedCoordsX[enemyPostionIntervalX];
                enemyPostionIntervalY = mappedCoordsY[enemyPostionIntervalY];
                globalIntervalsInfo[enemyPostionIntervalX, enemyPostionIntervalY] = true;
            }
        }
    }

    private void ScanEnemiesInIntervals(int targetIntervalX, int targetIntervalY)
    {
        var EnemiesArr = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in EnemiesArr)
        {
            int enemyPostionIntervalX = (int)Math.Ceiling(enemy.transform.position.x);
            int enemyPostionIntervalY = (int)Math.Ceiling(enemy.transform.position.y);
            enemyPostionIntervalY = mappedCoordsY[enemyPostionIntervalY];
            enemyPostionIntervalX = mappedCoordsX[enemyPostionIntervalX];

            if (enemyPostionIntervalX == targetIntervalX && enemyPostionIntervalY == targetIntervalY)
            {
                globalIntervalsInfo[enemyPostionIntervalX, enemyPostionIntervalY] = true;
                break;
            }
        }
    }

    private void ScanEnemiesInIntervals()
    {                                
        var EnemiesArr = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(var enemy in EnemiesArr)
        {
            AddEnemyToInterval(enemy);
        }
    }

    private void SetCoordsMap()
    {
        leftSide = -(mapSizeX / 2);
        rightSide = mapSizeX / 2;
        upSide = mapSizeY / 2;
        downSide = -(mapSizeY / 2);
        intervalArrSizeX = mapSizeX - iterationStep;
        intervalArrSizeY = mapSizeY - iterationStep;
        globalIntervalsInfo = new bool[intervalArrSizeX, intervalArrSizeY];
        var currentCoord = leftSide;

        int i = 0;
        while(currentCoord <= rightSide)
        {
            mappedCoordsX.Add(currentCoord, i);
            currentCoord += iterationStep;
            i++;
        }

        currentCoord = downSide;

        i = intervalArrSizeY;
        while (currentCoord <= upSide)
        {
            mappedCoordsY.Add(currentCoord, i);
            currentCoord += iterationStep;
            i--;
        }
    }

    private bool IsEnemyInFront()
    {
        Vector2 rayStartPosition = new Vector2(transform.position.x, transform.position.y + GetComponent<Collider2D>().bounds.size.y);
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, Vector2.up);

        if (hit.collider != null && hit.collider.gameObject.tag == "Enemy")
        {
            return true;
        }
        return false;
    }

    private GameObject GetEnemy()
    {
        if (Target == null)
        {
            var TargetsArr = GameObject.FindGameObjectsWithTag("Enemy");

            if (TargetsArr.Length > 0)
            {
                return TargetsArr[0];
            }
            //else
            //{
            //    //spawn one enemy
            //    spawn_manager.SpawnEnemy();
            //    return GameObject.FindGameObjectWithTag("Enemy");
            //}

            return null;
        }
        else
            return Target;

    }
}
