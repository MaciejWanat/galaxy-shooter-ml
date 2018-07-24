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
    private float previousYPos = float.MaxValue;
    private Spawn_Manager spawn_Manager;

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        spawn_Manager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
    }

    public override void CollectObservations()
    {
        var EnemiesArr = GameObject.FindGameObjectsWithTag("Enemy");
        float[] singleEnemyAttr = new float[7];
        List<float> enemiesAttributes = new List<float>(7);
        int i = 0;

        if (EnemiesArr.Length == 0)
        {
            spawn_Manager.SpawnEnemy();
        }

        EnemiesArr = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in EnemiesArr)
        {
            Target = enemy;
            Vector3 relativePosition = Target.transform.position - gameObject.transform.position;
            float distanceToTarget = Vector3.Distance(this.transform.position, Target.transform.position);
            //float distanceToTarget = (this.transform.position - Target.transform.position).magnitude;
            float distanceToTargetX = Mathf.Abs(this.transform.position.x - Target.transform.position.x);

            singleEnemyAttr[0] = (Target.transform.position.x);
            singleEnemyAttr[1] = (Target.transform.position.y);
            singleEnemyAttr[2] = (distanceToTargetX);
            singleEnemyAttr[3] = (distanceToTarget);
            singleEnemyAttr[4] = (relativePosition.x);
            singleEnemyAttr[5] = (relativePosition.y);
            singleEnemyAttr[6] = (enemy.GetComponent<EnemyAI>().speed);

            foreach(var attr in singleEnemyAttr)
            {
                enemiesAttributes.Add(attr);
            }               
        }
        
        /*
        //if list isnt full, fill it with "enemies" observations out of game field
        while(enemiesAttributes.Count < 7)
        {
            var fakePos = new Vector3(0, 7, 0);

            Vector3 relativePosition = fakePos - gameObject.transform.position;
            float distanceToTarget = Vector3.Distance(this.transform.position, fakePos);
            //float distanceToTarget = (this.transform.position - Target.transform.position).magnitude;
            float distanceToTargetX = Mathf.Abs(this.transform.position.x - fakePos.x);

            singleEnemyAttr[0] = (fakePos.x);
            singleEnemyAttr[1] = (fakePos.y);
            singleEnemyAttr[2] = (distanceToTargetX);
            singleEnemyAttr[3] = (distanceToTarget);
            singleEnemyAttr[4] = (relativePosition.x);
            singleEnemyAttr[5] = (relativePosition.y);
            singleEnemyAttr[6] = (10); //enemy speed

            foreach (var attr in singleEnemyAttr)
            {
                enemiesAttributes.Add(attr);
            }
        }
        */
        AddVectorObs(enemiesAttributes);
        AddVectorObs(gameObject.transform.position.x);
        AddVectorObs(gameObject.transform.position.y);
    }

    public override void AgentReset()
    {
        var EnemiesArr = GameObject.FindGameObjectsWithTag("Enemy");

        if (EnemiesArr.Length > 0)
        {
            foreach (var enemy in EnemiesArr)
            {
                Destroy(enemy);
            }
        }
        
        spawn_Manager.ResetHardLvlVars();
        this.transform.position = new Vector3(0, 0, 0);
        this.rBody.velocity = Vector2.zero;
        this.rBody.angularVelocity = 0;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        var EnemiesArr = GameObject.FindGameObjectsWithTag("Enemy");

        if (EnemiesArr.Length > 0)
        {
            Target = EnemiesArr[0];

            //Collided
            if (CheckCollision(Target))
            {
                AddReward(-5.0f);
                Done();
            }

            //Reward for surviving
            //AddReward(0.01f);

            float distanceToTarget = Vector3.Distance(this.transform.position, Target.transform.position);
            //float distanceToTarget = (this.transform.position - Target.transform.position).magnitude;
            float distanceToTargetX = Mathf.Abs(this.transform.position.x - Target.transform.position.x);

            //X is alligned - you are on collide course
            if (distanceToTargetX < 2.4f)
            {
                AddReward(-0.05f);
            }
            else
            {
                AddReward(0.05f);
            }  
        }

        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.y = vectorAction[1];

        transform.Translate(controlSignal * (speed * 2) * Time.deltaTime);
        //rBody.AddForce(controlSignal * speed);
    }

    private bool CheckCollision(GameObject target)
    {
        var thisCollider = gameObject.GetComponent<Collider2D>();
        var contactFiler = new ContactFilter2D { useTriggers = true };

        return thisCollider.IsTouching(target.GetComponent<Collider2D>(), contactFiler);
    }

    private GameObject GetSingleEnemy()
    {
        if (Target == null)
        {
            var TargetsArr = GameObject.FindGameObjectsWithTag("Enemy");

            if (TargetsArr.Length > 0)
            {
                return TargetsArr[0];
            }

            return null;
        }
        else
            return Target;

    }
}
