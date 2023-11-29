using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphereManager : MonoBehaviour
{
    public GameObject sphereEnv;

    public Light directionalLight;
    public float velocity = 2f;

    void Start()
    {
        StartCoroutine(RotateSphereGradually());
    }

    IEnumerator RotateSphereGradually()
    {
        while (true)
        {
            sphereEnv.transform.Rotate(0, 1.0f * velocity * Time.deltaTime, 0.2f * velocity * Time.deltaTime);
            Vector3 lightRotation = directionalLight.transform.rotation.eulerAngles;
            directionalLight.transform.rotation = Quaternion.Euler(50, lightRotation.y, 0);

            // Rotate the light only around the Y-axis
            directionalLight.transform.Rotate(0, 1.0f * velocity * Time.deltaTime, 0.2f * velocity * Time.deltaTime);

            yield return null;
        }
    }
}
