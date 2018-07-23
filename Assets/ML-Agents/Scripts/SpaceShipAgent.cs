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
    private Spawn_Manager spawn_Manager;

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        spawn_Manager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
    }

    public override void CollectObservations()
    {
        var EnemiesArr = GameObject.FindGameObjectsWithTag("Enemy");
        float[] attrList = new float[498];
        attrList = Array.ConvertAll(attrList, x => -5000f);
        int i = 0;        

        if (EnemiesArr.Length > 0)
        {
            foreach(var enemy in EnemiesArr)
            {
                Target = enemy;
                Vector3 relativePosition = Target.transform.position - gameObject.transform.position;
                float distanceToTarget = Vector3.Distance(this.transform.position, Target.transform.position);
                //float distanceToTarget = (this.transform.position - Target.transform.position).magnitude;
                float distanceToTargetX = Mathf.Abs(this.transform.position.x - Target.transform.position.x);

                attrList[i++] = (Target.transform.position.x);
                attrList[i++] = (Target.transform.position.y);
                attrList[i++] = (distanceToTargetX);
                attrList[i++] = (distanceToTarget);
                attrList[i++] = (relativePosition.x);
                attrList[i++] = (relativePosition.y);
                attrList[i++] = (enemy.GetComponent<EnemyAI>().speed);
            }
        }

        AddVectorObs(attrList);
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
        bool collided = false;

        if (EnemiesArr.Length > 0)
        {
            foreach(var enemy in EnemiesArr)
            {
                //Collided
                if (CheckCollision(enemy))
                {
                    AddReward(-5.0f);
                    Done();
                    collided = true;
                }
            }
            
            if(collided == false)
            {
                foreach(var enemy in EnemiesArr)
                {
                    Target = enemy;
                    //Reward for surviving
                    //AddReward(0.01f);

                    float distanceToTarget = Vector3.Distance(this.transform.position, Target.transform.position);
                    //float distanceToTarget = (this.transform.position - Target.transform.position).magnitude;
                    float distanceToTargetX = Mathf.Abs(this.transform.position.x - Target.transform.position.x);

                    //X is alligned - you are on collide course
                    if (distanceToTargetX < 2f)
                    {
                        AddReward(-0.05f);
                    }
                    else
                    {
                        AddReward(0.05f);
                    }
                }
            }
        }

        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.y = vectorAction[1];

        transform.Translate(Vector3.right * controlSignal.x * (speed * 2) * Time.deltaTime);
        transform.Translate(Vector3.up * controlSignal.y * (speed * 2) * Time.deltaTime);
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
