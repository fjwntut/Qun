using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LightDetector))]
[RequireComponent(typeof(Animator))]
public class AgentController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    LightDetector lightDetector;

    [Header("Navigation")]
    public Transform target;   // Navigation target
    public float speed; // Speed Multiplier
    public float stepSize; // Step size in unit per animation
    Vector3 lastPos;

    [Header("Light Detector")]
    public bool exposed;
    [Range(0f, 100)]
    public float grow; // state 0-100
    public float growSpeed; //
    public bool debugging;
    public Vector3 lightDirection = Vector3.back;
    public float detectorSize;


    void Start()
    {
        // Get Component
        agent = GetComponent<NavMeshAgent>();
        lightDetector = GetComponent<LightDetector>();
        animator = GetComponent<Animator>();

        // set light detector value
        lightDetector.set(debugging, lightDirection, detectorSize);
    }

    // Update is called once per frame
    void Update()
    {
        // Increase grow if exposed to light
        exposed = lightDetector.detected;
        if (exposed)
        {
            grow += growSpeed;
        }

        // Control Animation frame by frame
        animator.SetFloat("Grow", grow / 100);

        //  // Destroy when getting too big
        //  if (grow > 100 || grow < 0)
        //  {
        //      Destroy(gameObject);
        //  }

        // Set Nav target
        agent.destination = target.position;

        // Match step animation with Nav movement
        agent.speed = speed * transform.localScale.y;
        float realStepSize = stepSize * transform.localScale.y;
        if (agent.remainingDistance > realStepSize / 90 * 2)
        {
            animator.SetBool("Move", true);
            float rs = Vector3.Distance(lastPos, transform.position) * 1.5f / (realStepSize * Time.deltaTime);
            animator.SetFloat("RunSpeed", rs);
            lastPos = transform.position;
        }
        else
        {
            animator.SetBool("Move", false);
        }

        // Look at
        animator.SetLookAtPosition(agent.steeringTarget + transform.forward);
    }
}
