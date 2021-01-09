using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]

public class TargetController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;

    void OnEnable()
    {
        Debug.Log("Go!");
        agent.SetDestination(target.transform.position);
    }


}
