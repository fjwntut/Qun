using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LightDetector))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public class AgentController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    LightDetector lightDetector;
    CapsuleCollider capsuleCollider;

    [Header("Navigation")]
    public float Runspeed = 1; // RunSpeed Multiplier
    public float stepSize = 1;
    Vector3 lastPos;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (lightDetector.detected)
        {
            OnExposed();
        }

        // TODO: Random position

        // Match step animation with Nav movement
        agent.speed = Runspeed * transform.localScale.y;   // TODO: Update only changed
        float realStepSize = stepSize * transform.localScale.y; // TODO: Update only changed

        if (agent.remainingDistance < realStepSize / 10)
        {
            if (animator.GetBool("Move"))
            {
                animator.SetBool("Move", false);
            }
        }
        else
        {
            Debug.Log("oo");
            if (!animator.GetBool("Move"))
            {
                animator.SetBool("Move", true);
            }
            // Match step animation with Nav movement
            float agentSpeed = Vector3.Distance(lastPos, transform.position) / Time.deltaTime; // agent moved distance in second
            animator.SetFloat("RunSpeed", agentSpeed / realStepSize);
            lastPos = transform.position;
        }

        // Look at
        animator.SetLookAtPosition(agent.steeringTarget + transform.forward);
    }
    void OnExposed()
    {
        grow += growSpeed;

        // Change state if grow reach cutPoint
        if (grow > cutPoints[stateIndex])
        {
            stateIndex++;
            PauseResumeWalking(false);
            ChangeState(stateIndex);
        }
    }
    void ChangeState(int index)
    {
        animator.SetInteger("State", index);
        MoveToLevel(index);
    }
    public void MoveToLevel(int index)
    {
        // TODO: use fall
        transform.position = levelBirths[index].position;
    }
    public void PauseResumeWalking(bool mode) // false: pause, true: resume
    {
        agent.isStopped = !mode;
        animator.SetBool("Move", mode);
    }
}
