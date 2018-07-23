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

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
    }

    public override void CollectObservations()
    {
        Target = GetEnemy();

        Vector3 relativePosition = new Vector3(-100, -100, -100);

        if (Target)
        {
            relativePosition = Target.transform.position - gameObject.transform.position;
            AddVectorObs(Target.transform.position.x);
            AddVectorObs(Target.transform.position.y);

            float distanceToTarget = Vector3.Distance(this.transform.position, Target.transform.position);
            //float distanceToTarget = (this.transform.position - Target.transform.position).magnitude;
            float distanceToTargetX = Mathf.Abs(this.transform.position.x - Target.transform.position.x);

            AddVectorObs(distanceToTargetX);
            AddVectorObs(distanceToTarget);
        }
        else
        {
            AddVectorObs(-100);
            AddVectorObs(-100);
            AddVectorObs(-100);
            AddVectorObs(-100);
        }

        AddVectorObs(relativePosition.x);
        AddVectorObs(relativePosition.y);
        AddVectorObs(gameObject.transform.position.x);
        AddVectorObs(gameObject.transform.position.y);
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.y);
    }

    public override void AgentReset()
    {
        Target = GetEnemy();
        if (Target)
            Destroy(Target);

        this.transform.position = new Vector3(0, 0, 0);
        this.rBody.velocity = Vector2.zero;
        this.rBody.angularVelocity = 0;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        Target = GetEnemy();

        if (Target)
        {
            //Collided
            if (CheckCollision(Target))
            {
                AddReward(-5.0f);
                Done();
            }
            else
            {
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
                /*
                //Getting further
                if (distanceToTarget > (previousDistance + 0.1f))
                {
                    //Debug.Log(distanceToTarget + " > " + (previousDistance + 0.1).ToString());
                    AddReward(0.1f);
                }
                previousDistance = distanceToTarget;*/
            }
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
        //rBody.AddForce(controlSignal * speed);
    }

    private bool CheckCollision(GameObject target)
    {
        var thisCollider = gameObject.GetComponent<Collider2D>();
        var contactFiler = new ContactFilter2D { useTriggers = true };

        return thisCollider.IsTouching(target.GetComponent<Collider2D>(), contactFiler);
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

            return null;
        }
        else
            return Target;

    }
}
