using MLAgents;
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

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        spawn_manager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void CollectObservations()
    {
        Target = GetEnemy();

        Vector3 relativePosition = Target.transform.position - gameObject.transform.position;
        AddVectorObs(Target.transform.position.x);
        AddVectorObs(Target.transform.position.y);

        float distanceToTarget = Vector3.Distance(this.transform.position, Target.transform.position);
        //float distanceToTarget = (this.transform.position - Target.transform.position).magnitude;
        float distanceToTargetX = Mathf.Abs(this.transform.position.x - Target.transform.position.x);

        AddVectorObs(distanceToTargetX);
        AddVectorObs(distanceToTarget);
        AddVectorObs(relativePosition.x);
        AddVectorObs(relativePosition.y);
        AddVectorObs(gameObject.transform.position.x);
        AddVectorObs(gameObject.transform.position.y);
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.y);
    }

    public override void AgentReset()
    {
        //destroy enemies
        Target = GetEnemy();
        if (Target)
            Destroy(Target);
        
        //destroy lasers
        var Lasers = GameObject.FindGameObjectsWithTag("Laser");
        uiManager.ResetScore();

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
        Target = GetEnemy();

        if (Target)
        {
            var Lasers = GameObject.FindGameObjectsWithTag("Laser");

            foreach(var laser in Lasers)
            {
                if(CheckCollision(laser, Target))
                {
                    AddReward(1.0f);
                    uiManager.UpdateScore();
                    Target.GetComponent<EnemyAI>().PlayExplode();
                    Destroy(Target);
                    Destroy(laser);
                }
            }

            //Collided
            if (CheckCollision(Target))
            {
                AddReward(-1.0f);
                Done();
            }

            float distanceToTargetX = Mathf.Abs(this.transform.position.x - Target.transform.position.x);

            //X is alligned and you shoot = reward
            if (vectorAction[2] > 0 && distanceToTargetX < 2f)
            {
                AddReward(0.05f);
            }

            //Now point is to shoot enemy - not to get out of its way
            //else
            //{
            //    //Reward for surviving
            //    //AddReward(0.01f);

            //    float distanceToTarget = Vector3.Distance(this.transform.position, Target.transform.position);
            //    //float distanceToTarget = (this.transform.position - Target.transform.position).magnitude;
            //    float distanceToTargetX = Mathf.Abs(this.transform.position.x - Target.transform.position.x);

            //    //X is alligned - you are on collide course
            //    if (distanceToTargetX < 2f)
            //    {
            //        AddReward(-0.05f);
            //    }
            //    else
            //    {
            //        AddReward(0.05f);
            //    }
            //    /*
            //    //Getting further
            //    if (distanceToTarget > (previousDistance + 0.1f))
            //    {
            //        //Debug.Log(distanceToTarget + " > " + (previousDistance + 0.1).ToString());
            //        AddReward(0.1f);
            //    }
            //    previousDistance = distanceToTarget;*/
            //}
        }
        else
        {
            previousDistance = float.MinValue;
        }

        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.y = vectorAction[1];

        transform.Translate(Vector3.right * controlSignal.x * (speed * 2) * Time.deltaTime);
        transform.Translate(Vector3.up * controlSignal.y * (speed * 2) * Time.deltaTime);

        if(vectorAction[2] > 0)
        {
            player.GetComponent<Player>().Shoot();
        }

        //rBody.AddForce(controlSignal * speed);
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

    private GameObject GetEnemy()
    {
        if (Target == null)
        {
            var TargetsArr = GameObject.FindGameObjectsWithTag("Enemy");

            if (TargetsArr.Length > 0)
            {
                return TargetsArr[0];
            }
            else
            {
                //spawn one enemy
                spawn_manager.SpawnEnemy();
                return GameObject.FindGameObjectWithTag("Enemy");
            }

            return null;
        }
        else
            return Target;

    }
}
