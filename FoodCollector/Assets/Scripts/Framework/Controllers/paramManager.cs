using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ParamManager : MonoBehaviour
{

    [SerializeField]
    private float initialSpeed = 5.0f;

    [SerializeField]
    private float initialDistanceMultiplier = 1.0f;

    public static float speed { get; private set; }
    public static float distanceMultiplier { get; private set; }

    // Awake is called before Start. Use it to initialize values.
    private void Awake()
    {
        speed = initialSpeed;
        distanceMultiplier = initialDistanceMultiplier;
    }
}
