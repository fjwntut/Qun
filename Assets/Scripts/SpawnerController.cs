using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LightDetector))]
[RequireComponent(typeof(Animator))]

public class SpawnerController : MonoBehaviour
{

    public GameObject prefab;
    public Transform parent;
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
        spawned = false;

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"grow {grow}");

        Debug.Log($"lightdetected! {lightDetector.detected}");
        if (lightDetector.detected)
        {
            grow += growSpeed;
        }
        else if (!spawned && grow > 0)
        {
            grow -= growSpeed;
        }
        if (grow > 1)
        {
            DestroyMe();
        }

    }

    public void Spawn()
    {
        if (!spawned)
        {
            GameObject child = Instantiate(prefab, transform.position, Quaternion.identity, parent);
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
