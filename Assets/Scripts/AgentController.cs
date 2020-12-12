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
public class AgentController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    LightDetector lightDetector;
    CapsuleCollider capsuleCollider;

    [Header("Navigation")]
    public float Runspeed = 1; // RunSpeed Multiplier
    public float stepSize = 1;
    public float minDistance = 0;
    public float maxDistance = 5;
    Vector3 lastPos;
    public float nextRestTime;

    [Header("Grow")]

    [Range(0f, 100)]
    public float grow = 0; // state 0-100
    public float growSpeed = 0.2f; // state increased by frame when exposed to light 
    public List<int> cutPoints = new List<int> { 33, 66, 99, 101 }; // change states when grow reached these value
    public List<Transform> levelBirths;
    int stateIndex = 0;

    void Start()
    {
        // Get Component
        agent = GetComponent<NavMeshAgent>();
        lightDetector = GetComponent<LightDetector>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        lastPos = transform.position;
        nextRestTime = 0;
    }

    public void setWalking(bool walk) // false: pause, true: resume
    {
        agent.isStopped = !walk;
        animator.SetBool("Move", walk);
    }
    public void MoveToLevel()
    {
        // TODO: use fall
        transform.position = levelBirths[stateIndex].position;
        setWalking(true);
    }
    void OnExposed()
    {
        grow += growSpeed;

        // Change state if grow reach cutPoint
        if (grow > cutPoints[stateIndex])
        {
            stateIndex++;
            setWalking(false);
            animator.SetInteger("State", stateIndex);
            MoveToLevel();
        }
    }
    void ChangeDirection()
    {
        setWalking(false);
        agent.destination = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        nextRestTime = Time.time + Random.Range(minDistance, maxDistance);
        setWalking(true);
    }

    void OnMove()
    {
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
        animator.SetLookAtPosition(agent.steeringTarget + transform.forward);
    }

    void Update()
    {
        if (lightDetector.detected)
        {
            OnExposed();
        }

        if (Time.time >= nextRestTime || agent.remainingDistance <= 1)
        {
            ChangeDirection();
        }
        else if (animator.GetBool("Move"))
        {
            OnMove();
        }
    }




}
