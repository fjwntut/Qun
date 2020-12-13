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
    public float minDistance = 5;
    public float maxDistance = 20;
    Vector3 lastPos;
    public float nextTime;
    bool move;

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
        grow = 0;
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
                stateIndex++;

                // Pause Walking
                move = true;
                ChangeWalking();

                // Start Animation
                animator.SetInteger("State", stateIndex);

                // Call MoveToLevel in animation
            }

        }

        if (move)
        {
            if (agent.remainingDistance == 0)
            {
                nextTime = Time.time;
                Debug.Log("Dead End!");
            }
            else
            {
                OnMove();
            }
        }

        if (Time.time >= nextTime && grow > 0)
        {
            Debug.Log("Times UP");
            ChangeWalking();
        }
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

    void ChangeWalking() // will pause or resume walking
    {
        move = !move;

        // Change destination on Resume
        if (move)
        {
            agent.Move(new Vector3(Random.Range(-10, 10), transform.position.y + 10, Random.Range(-10, 10)));
            //agent.destination = new Vector3(Random.Range(-100, 100), transform.position.y + 100, Random.Range(-100, 100));
        }
        agent.isStopped = !move;
        animator.SetBool("Move", move);
        nextTime = Time.time + Random.Range(minDistance, maxDistance);
        Debug.Log((move ? $"Resume" : "Paused") + $", Next Time = {nextTime}");
    }

    public void MoveToLevel()
    {
        // TODO: use fall
        nextTime = Time.time;
        agent.enabled = false;  // Disable agent to move
        transform.position = levelBirths[stateIndex - 1].position;
        agent.enabled = true;
        Debug.Log($"{name} moved to {levelBirths[stateIndex - 1].name}");
    }

}
