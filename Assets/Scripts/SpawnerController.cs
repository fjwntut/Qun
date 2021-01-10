using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LightDetector))]
[RequireComponent(typeof(Animator))]

public class SpawnerController : MonoBehaviour
{

    public GameObject prefab;
    public Transform destination;
    float grow
    {
        get
        {
            return animator.GetFloat("Grow");
        }
        set
        {
            animator.SetFloat("Grow", value);
        }
    }
    bool startGrow = false;

    bool spawned = false;
    public float growSpeed = 0.02f; // state increased by frame when exposed to light 
    LightDetector lightDetector;
    Animator animator;
    Vector3 oriScale;
    // Start is called before the first frame update
    void Start()
    {
        lightDetector = gameObject.GetComponent<LightDetector>();
        animator = gameObject.GetComponent<Animator>();
        oriScale = transform.localScale;
        gameObject.tag = "Spawner";
    }

    // Update is called once per frame
    void Update()
    {
        if (grow >= 0)
        {
            if (lightDetector.detected)
            {
                grow += growSpeed;
            }
            else if (!spawned)
            {
                grow -= growSpeed;
            }
            if (grow > 1)
            {
                DestroyMe();
            }
        }
    }

    public void Spawn()
    {
        if (!spawned)
        {
            GameObject child = Instantiate(prefab, transform.position, Quaternion.identity);
            child.tag = "popcorn";
            Debug.Log("popcorn birth");
        }
        spawned = true;
    }
    public void DestroyMe()
    {
        Debug.Log("byebye");
        Destroy(this.gameObject);
    }


}
