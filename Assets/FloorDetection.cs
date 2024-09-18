using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDetection : MonoBehaviour
{
    private float floors = 0;
    public float Floors
    {
        get
        {
            return floors;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        floors++;
    }

    private void OnTriggerExit(Collider other)
    {
        floors--;
    }
}
