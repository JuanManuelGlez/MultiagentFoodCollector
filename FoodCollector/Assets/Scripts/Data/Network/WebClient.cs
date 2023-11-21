// TC2008B Modelación de Sistemas Multiagentes con gráficas computacionales
// C# client to interact with Python server via POST
// Sergio Ruiz-Loza, Ph.D. March 2021

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{

    // public FoodManager foodManager;

    private ResModel resData;

    IEnumerator GetData()
    {
        string url = "http://localhost:8585";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                ResModel resModel = JsonUtility.FromJson<ResModel>(jsonResponse);
                resData = resModel;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetData());

        Debug.Log(resData);


    }

    // Update is called once per frame
    void Update()
    {

    }
}