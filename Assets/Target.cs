using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour
{
    public NavMeshAgent agent;

    void Update()
    {
        if (agent.destination != transform.position)
        {
            agent.SetDestination(transform.position);
        }
    }
}
