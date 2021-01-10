using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class ArduninoRead : MonoBehaviour
{
    Quaternion initialPos = new Quaternion();
    public bool doorClosed;
    public Transform door;
    Quaternion doorinitpos;
    public List<int> box = new List<int> { 1, 2, 3 };
    public List<int> doorRotation = new List<int> { 4, 5, 6 };
    bool[] closes = new bool[10];

    int closesIndex = 0;

    float gyro_normalizer_factor = 0.1f * 180 / Mathf.PI;

    private void Start()
    {
        initialPos = this.transform.rotation;
        doorinitpos = door.localRotation;
        UduinoManager.Instance.OnDataReceived += DataReceived;
    }

    private void DataReceived(string data, UduinoDevice board)
    {
        //Debug.Log(data);
        string[] values = data.Split('\\');

        Debug.Log(closesIndex);
        closes[closesIndex] = (values[0] == "1");
        closesIndex += (closesIndex < closes.Length - 1 ? 1 : 1 - closes.Length);
        doorClosed = true;
        foreach (bool close in closes)
        {
            if (!close)
            {
                doorClosed = false;
                break;
            }
        }

        if (doorClosed)
        {
            //this.transform.rotation = initialPos;
            door.localRotation = doorinitpos;
            transform.rotation *= Quaternion.Euler(parseVector(box));
        }
        else
        {
            transform.rotation *= Quaternion.Euler(parseVector(box));
            door.rotation *= Quaternion.Euler(parseVector(doorRotation) * -1);
            Debug.Log(door.rotation);

        }


        Vector3 parseVector(List<int> indexs)
        {
            float x, y, z;
            if (indexs.Count != 3)
            {
                Debug.Log("invalid list!");
                x = y = z = 0;
            }
            else
            {
                string[] vs = values;
                vs[0] = "0";
                x = float.Parse(vs[indexs[0]]) * gyro_normalizer_factor;
                y = float.Parse(vs[indexs[1]]) * gyro_normalizer_factor;
                z = float.Parse(vs[indexs[2]]) * gyro_normalizer_factor;
            }
            return new Vector3(x, y, z);
        }

    }
}
