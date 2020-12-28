using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class ArduninoRead : MonoBehaviour
{
    Quaternion initialPos = new Quaternion();
    public Transform door;

    public List<int> box = new List<int> { 1, 2, 3 };
    public List<int> doorRotation = new List<int> { 4, 5, 6 };


    float gyro_normalizer_factor = 0.1f * 180 / Mathf.PI;


    private void Start()
    {
        initialPos = this.transform.rotation;
        UduinoManager.Instance.OnDataReceived += DataReceived;
    }

    private void DataReceived(string data, UduinoDevice board)
    {
        Debug.Log(data);
        string[] values = data.Split('\\');

        if (int.Parse(values[0]) == 1)
        {
            this.transform.rotation = initialPos;
        }
        else
        {
            transform.rotation *= Quaternion.Euler(parseVector(box));

            door.rotation *= Quaternion.Euler(parseVector(doorRotation));

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
                x = float.Parse(values[indexs[0]]) * gyro_normalizer_factor;
                y = float.Parse(values[indexs[1]]) * gyro_normalizer_factor;
                z = float.Parse(values[indexs[2]]) * gyro_normalizer_factor;
            }
            return new Vector3(x, y, z);
        }

    }
}
