using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LightDetector))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshObstacle))]
public class newAgentController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    LightDetector lightDetector;
    CapsuleCollider capsuleCollider;
    Rigidbody rb;


    [Header("Resting")]

    public float minTime = 0;   // minimum walk time (rest Time is walk time /10)
    public float maxTime = 5;   // maximum walk time
    bool NextTimeEnded   // sync with animator parameter
    {
        get
        {
            // Check if next time ended
            return (animator.GetFloat("Next time") >= Time.time);
        }
        set
        {
            // Set nextTime to current time if true / random time if false
            float nextTime = Time.time + (value ? 0 : Random.Range(minTime, maxTime) * (Moving ? 1f : 0.1f));
            animator.SetFloat("Next time", nextTime);
            Debug.Log($"Next time set to: {nextTime - Time.time}s later");
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


    void Start()
    {
        // Get Component
        agent = GetComponent<NavMeshAgent>();
        lightDetector = GetComponent<LightDetector>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
        lastPos = transform.position;
        NextTimeEnded = true;
        grow = -growSpeed;
    }

    void Update()
    {
        // light Detected (stop in last state)
        if (stateIndex < cutPoints.Count && lightDetector.detected)
        {
            grow += growSpeed;

            // Change state if grow reach cutPoint
            if (grow > cutPoints[stateIndex])
            {
                ChangeState();
            }
            else if (grow == 0)
            {
                Moving = true;
            }
        }

        // if path come to end
        if (agent.remainingDistance <= 1f || agent.remainingDistance == float.PositiveInfinity)
        {
            if (Moving) // for the first frame
            {
                Moving = !Moving;
                NextTimeEnded = false;  // generate new nextTime
            }
            else // havent found valid path or path not yet
            {
                ChangeDirection(2);
            }
        }
        else if (NextTimeEnded)
        {
            Moving = !Moving;
            NextTimeEnded = false;  // generate new nextTime
        }
        else if (Moving)
        {
            OnMove();
        }
        else
        {
            // Do nothing
        }
    }

    void ChangeState()
    {
        stateIndex++;
        NextTimeEnded = true;// Pause Walking
        animator.SetInteger("State", stateIndex);// Start Animation
        // Call MoveToLevel in animation
    }

    public void MoveToLevel()
    {
        // TODO: use fall
        NextTimeEnded = true;
        agent.enabled = false;
        transform.position = levelBirths[stateIndex - 1].position;
        agent.enabled = true;
    }
    void ChangeDirection(float offset)  // cannot use 
    {
        Debug.Log("Finding new destination...");
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

        // Match step animation with Nav movement
        float agentSpeed = Vector3.Distance(lastPos, transform.position) / Time.deltaTime; // agent moved distance in second
        animator.SetFloat("RunSpeed", agentSpeed / realStepSize);
        lastPos = transform.position;

        // Look at
        transform.LookAt(agent.steeringTarget + transform.forward);
        transform.Rotate(0, 90, 0); // correct prefab rotation
    }



}
