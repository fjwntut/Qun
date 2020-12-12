using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class ArduninoRead : MonoBehaviour
{
    Quaternion initialPos = new Quaternion();

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

        if (int.Parse(values[0]) == 0)
        {
            this.transform.rotation = initialPos;
        }
        else
        {
            float gx = float.Parse(values[1]) * gyro_normalizer_factor;
            float gy = float.Parse(values[2]) * gyro_normalizer_factor;
            float gz = float.Parse(values[3]) * gyro_normalizer_factor;

            //if (Mathf.Abs(gx) < 0.025f) gx = 0f;
            //if (Mathf.Abs(gy) < 0.025f) gy = 0f;
            //if (Mathf.Abs(gz) < 0.025f) gz = 0f;

            //Debug.Log(gx + "/" + gy + "/" + gz);

            this.transform.rotation *= Quaternion.Euler(new Vector3(0, -gx, 0));
        }

    }
}
