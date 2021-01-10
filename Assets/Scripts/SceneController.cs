using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SceneController : MonoBehaviour
{
    public Transform door;
    bool doorCloseDetected
    {
        get
        {
            return door.transform.eulerAngles.x == 0;
        }
    }
    public bool doorClosed
    {
        get
        {
            return animator.GetBool("Closed");
        }
        set
        {
            animator.SetBool("Closed", value);
        }
    }
    public float grow
    {
        get
        {
            return animator.GetFloat("Grow") * 100;
        }
        set
        {
            animator.SetFloat("Grow", value / 100);
        }

    }
    public int SceneID;
    public List<GameObject> Scene = new List<GameObject>(4);

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // sync animator with arduino
        if (doorClosed != doorCloseDetected)
        {
            doorClosed = !doorClosed;
            Debug.Log($"doorClosed = {doorClosed}");
        }

        // sync rotation
        grow = (gameObject.transform.eulerAngles.z / 360) * 100;


    }
    public void setPopcorn()
    {
        GameObject[] popcorns = GameObject.FindGameObjectsWithTag("popcorn");
        GameObject[] births = GameObject.FindGameObjectsWithTag("birth");

        for (int i = 0; i < popcorns.Length; i++)
        {
            AgentController agent = popcorns[i].GetComponent<AgentController>();
            Transform place = births[i].transform;
            agent.setPlace(place);
        }

    }
}

