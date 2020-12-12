using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetector : MonoBehaviour
{
    public bool drawDectector = false;
    public Vector3 lightDirection = Vector3.back;
    public float detectorSize = 0.5f;
    public float maxDistance = 300.0f;
    public bool hitDetect;
    Collider m_collider;
    RaycastHit hit;

    public bool detected
    {
        get { return !hitDetect; }
    }

    void Start()
    {
        //Choose the distance the Box can reach to
        maxDistance = 300.0f;
        m_collider = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        //Test to see if there is a hit using a BoxCast
        //Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        //Also fetch the hit data
        hitDetect = Physics.BoxCast(m_collider.bounds.center, transform.localScale * detectorSize, lightDirection, out hit, transform.rotation, maxDistance);
        if (hitDetect)
        {
            if (drawDectector)
            {
                //Output the name of the Collider your Box hit
                Debug.Log("Hit : " + hit.collider.name);
            }
        }
    }

    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (drawDectector)
        {
            //Check if there has been a hit yet
            if (hitDetect)
            {
                //Draw a Ray forward from GameObject toward the hit
                Gizmos.DrawRay(transform.position, lightDirection * hit.distance);
                //Draw a cube that extends to where the hit exists
                Gizmos.DrawWireCube(transform.position + lightDirection * hit.distance, transform.localScale);
            }
            //If there hasn't been a hit yet, draw the ray at the maximum distance
            else
            {
                //Draw a Ray forward from GameObject toward the maximum distance
                Gizmos.DrawRay(transform.position, lightDirection * maxDistance);
                //Draw a cube at the maximum distance
                Gizmos.DrawWireCube(transform.position + lightDirection * maxDistance, transform.localScale);
            }
        }
    }
}
