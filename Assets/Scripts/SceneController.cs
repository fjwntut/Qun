using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [Range(0, 360)]
    public float boxRotationZ, doorRotationY;
    public bool UduinoDoor;
    bool doorClosed;
    public Transform door;
    public int StateIndex;
    public List<int> angles;
    public List<Transform> scenePrefabs;
    Transform scenePrefab;
    public float microwaveRotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        // set up angles
        for (int i = 0; i < 360; i += 90)
        {
            angles.Add(i);
            angles.Add(i + 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // sync door and box rotation with gameObjects
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, boxRotationZ));
        door.rotation = Quaternion.Euler(new Vector3(0, doorRotationY, 0));

        for (int i = 0; i < 360; i += 90)
        {
            if (boxRotationZ >= i)
            {
                int State = i / 90;

            }
        }
        // detect door opened and send to Animator
        if (doorClosed != UduinoDoor)
        {
            scenePrefab.GetComponent<Animator>().SetBool("Closed", doorClosed);
            doorClosed = UduinoDoor;
        }

        if (!doorClosed)
        {
            scenePrefab.GetComponent<Animator>().speed = Mathf.Clamp(doorRotationY, 0, 90) / 90;
        }

        //foreach(int index in indexes){
        //
        //}

    }

    void OnEnterState(int stateIndex)
    {
        Destroy(scenePrefab);
        scenePrefab = Instantiate(scenePrefabs[stateIndex]);
    }
}

