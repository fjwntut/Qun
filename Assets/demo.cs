using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo : MonoBehaviour
{
    [Range(0f, 180f)]
    public float boxRotation;
    [Range(0f, 90f)]
    public float doorRotation;
    public Transform door;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, boxRotation);
        door.localEulerAngles = new Vector3(doorRotation, 0, 0);
        animator.SetBool("c2", doorRotation == 0);

    }
}
