using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour
{
    public NavMeshAgent agent;
    public bool TargetAtMe;
    public bool ClickToMove;
    RaycastHit hitInfo = new RaycastHit();
    void Update()
    {
        if (TargetAtMe && transform.hasChanged)
        {
            agent.SetDestination(transform.position);
            transform.hasChanged = false;
        }
        if (ClickToMove && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                agent.destination = hitInfo.point;
        }
    }
}
