using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphereManager : MonoBehaviour
{
    public GameObject sphereEnv;
    public float velocity = 2f;

    void Start()
    {
        StartCoroutine(RotateSphereGradually());
    }

    IEnumerator RotateSphereGradually()
    {
        while(true)
        {
            sphereEnv.transform.Rotate(0,1.0f*velocity*Time.deltaTime,0.2f*velocity*Time.deltaTime);
            yield return null;
        }
    }
}
