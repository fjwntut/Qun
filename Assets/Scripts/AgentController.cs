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
public class AgentController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    LightDetector lightDetector;
    CapsuleCollider capsuleCollider;
    Rigidbody rb;

    [Header("Navigation")]
    public float Runspeed = 1; // RunSpeed Multiplier
    public float stepSize = 1;
    public float minTime = 0;
    public float maxTime = 5;
    Vector3 lastPos;
    public float nextTime;
    public bool moving;

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
        nextTime = 0;
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
                setMoving(true);
            }
        }

        if (agent.remainingDistance <= 1f || agent.remainingDistance == float.PositiveInfinity)
        {
            if (moving)
            {
                setMoving(!moving);
                nextTime = Time.time + Random.Range(minTime, maxTime) * (moving ? 1f : 0.1f);
            }
            else
            {
                ChangeDirection(2);
                Debug.Log("finding destination");
            }
        }
        else if (Time.time >= nextTime)
        {
            setMoving(!moving);
            nextTime = Time.time + Random.Range(minTime, maxTime) * (moving ? 1f : 0.1f);
        }
        else if (moving)
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
        nextTime = Time.time;// Pause Walking
        Debug.Log($"Next time set to: {nextTime - Time.time} later");
        animator.SetInteger("State", stateIndex);// Start Animation
        // Call MoveToLevel in animation
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
        transform.Rotate(0, 90, 0);
    }

    void setMoving(bool move) // false: pause, true: resume
    {
        if (moving != move)
        {
            moving = move;
            agent.isStopped = !move;
            animator.SetBool("Move", move);
            Debug.Log(moving ? "Resume" : "Paused");
        }
    }

    public void MoveToLevel()
    {
        // TODO: use fall
        nextTime = Time.time;
        Debug.Log(nextTime);
        agent.enabled = false;
        transform.position = levelBirths[stateIndex - 1].position;
        agent.enabled = true;

    }
    void ChangeDirection(float offset)  // cannot use 
    {
        agent.destination = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        float rd = agent.remainingDistance;
    }

    void SetNextTime(float time)
    {

    }

}
