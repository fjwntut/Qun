﻿using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LightDetector))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class AgentController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    LightDetector lightDetector;

    Rigidbody rigidbody;

    [Header("Resting")]

    public float minTime = 0;   // minimum walk time (rest Time is walk time /10)
    public float maxTime = 5;   // maximum walk time (rest Time is walk time /10)
    bool NextTimeEnded   // sync with animator parameter
    {
        get
        {
            // Check if next time ended
            return (Time.time >= animator.GetFloat("Next time"));
        }
        set
        {
            // Set nextTime to current time if true / random time if false
            float nextTime = Time.time + (value ? 0 : Random.Range(minTime, maxTime) * (Moving ? 1f : 0.1f));
            animator.SetFloat("Next time", nextTime);
            // Debug.Log($"Next time set to: {nextTime - Time.time}s later");
        }
    }
    bool Moving  // sync with animator parameter
    {
        get
        {
            return animator.GetBool("Move");
        }
        set
        {
            agent.isStopped = !value;
            animator.SetBool("Move", value);
            Debug.Log(value ? "Resume" : "Paused");
            animator.SetInteger("State", stateIndex);
        }
    }

    bool falling
    {
        get
        {
            return animator.GetBool("Falling");
        }
        set
        {
            animator.SetBool("Falling", value);
        }
    }

    [Header("Locomotion")]
    public float Runspeed = 1; // RunSpeed Multiplier
    public float stepSize = 1;  // adjust freely
    Vector3 lastPos;

    [Header("Grow")]

    [Range(0f, 100)]
    public float grow = 0; // state 0-100
    public float growSpeed = 0.2f; // state increased by frame when exposed to light 
    public List<int> cutPoints = new List<int> { 33, 66, 99 }; // change states when grow reached these value
    public List<Transform> levelBirths = new List<Transform>(3);
    int stateIndex = 0;

    public Transform box;

    void Start()
    {
        // Get Component
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        lightDetector = GetComponent<LightDetector>();
        lightDetector.enabled = false;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        lastPos = transform.position;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        // stop movings
        grow = -growSpeed;
        reachlevel = false;

    }

    public void setPlace(Transform place)
    {
        transform.position = place.position;
        lightDetector.enabled = true;
        Debug.Log("setPlace");
    }

    void Update()
    {
        // light Detected (stop in last state)
        if (lightDetector.enabled && stateIndex < cutPoints.Count && lightDetector.detected)
        {
            grow += growSpeed;

            // Change state if grow reach cutPoint
            if (grow > cutPoints[stateIndex])
            {
                ChangeState();
            }
            // Start to move when exposed first time
            else if (grow == 0)
            {
                Debug.Log("startToMove");
                agent.enabled = true;
                Moving = true;
                reachlevel = true;
                NextTimeEnded = true;
                rigidbody.constraints = RigidbodyConstraints.None;

            }
        }
        if (agent.enabled)
        {
            if (reachlevel && (agent.remainingDistance <= 1f || agent.remainingDistance == float.PositiveInfinity)) // No valid path
            {
                if (Moving) // for the first frame when path ended
                {
                    Moving = !Moving; // stop moving
                    NextTimeEnded = false;  // generate new nextTime
                }
                else // havent found valid path or path not yet
                {
                    ChangeDirection(2);
                }
            }
            else if (NextTimeEnded)
            {
                Moving = !Moving;   // change moving state
                NextTimeEnded = false;  // generate new nextTime
            }
            else if (Moving)
            {
                if (!reachlevel && stateIndex > 0 && Vector2.Distance(levelBirths[stateIndex - 1].position, transform.position) < 0.1)
                {
                    reachlevel = true;
                }
                OnMove();
            }
            else
            {
                // Do nothing
            }
        }
        else if (stateIndex == 3 && !reachlevel)
        {

            rigidbody.AddForce(new Vector3(0, 100, 0));
            if (transform.localPosition.x > 0.91)
            {
                reachlevel = true;
                agent.enabled = true;
                Debug.Log("reachsky");
            }
        }

    }
    public bool reachlevel = true;
    void ChangeState()
    {
        stateIndex++;
        Moving = false;// Pause Walking
        reachlevel = false;
        animator.SetInteger("State", stateIndex);// Start Animation
        // Call MoveToLevel in animation
    }

    [ContextMenu("MoveLevel")]
    public void MoveToLevel()
    {

        Debug.Log($"Move to {levelBirths[stateIndex - 1].gameObject.name}.....");
        if (stateIndex == 3)
        {
            agent.enabled = false;
            animator.SetBool("Rising", true);
        }
        agent.destination = levelBirths[stateIndex - 1].position;
        Moving = true;

    }
    void ChangeDirection(float offset)  // cannot use 
    {
        // Debug.Log("Finding new destination...");
        agent.destination = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        float rd = agent.remainingDistance;
    }

    void OnMove()
    {
        // Sync with Transform
        float realStepSize = stepSize;
        if (transform.hasChanged)
        {
            agent.speed = Runspeed * transform.localScale.y;
            realStepSize = stepSize * transform.localScale.y;
            transform.hasChanged = false;
        }
        // Match jump
        falling = (((lastPos.y - transform.position.y) / Time.deltaTime) > 0);

        // Match step animation with Nav movement
        float agentSpeed = Vector3.Distance(lastPos, transform.position) / Time.deltaTime; // agent moved distance in second
        animator.SetFloat("RunSpeed", agentSpeed / realStepSize);
        lastPos = transform.position;

        // Look at
        transform.LookAt(agent.steeringTarget + transform.forward);
        transform.Rotate(0, 90, 0); // correct prefab rotation
    }
}
