using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class ArduninoRead : MonoBehaviour
{

    public List<int> boxRotation = new List<int> { 1, 2, 3 };
    public float boxOffset;
    public bool boxCalibration;
    Quaternion boxInitPos;
    public Transform door;
    public List<int> doorRotation = new List<int> { 4, 5, 6 };
    public float doorOffset;
    public bool doorClosed;
    Quaternion doorinitpos;
    bool[] closes = new bool[1];

    int closesIndex = 0;

    float gyro_normalizer_factor = 0.1f * 180 / Mathf.PI;

    private void Start()
    {
        boxInitPos = transform.rotation;
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
            if (!boxCalibration)
            {
                transform.rotation *= Quaternion.Euler(parseVector(boxRotation) * boxOffset);
            }
        }
        else
        {
            door.rotation *= Quaternion.Euler(parseVector(doorRotation) * doorOffset);
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
