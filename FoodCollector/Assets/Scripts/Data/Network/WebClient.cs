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

    public FoodManager foodManager;

    public AgentManager agentManager;

    public Dictionary<int, List<Food>> foodMap = new Dictionary<int, List<Food>>();

    // Agent map
    public Dictionary<int, List<Agent>> agentMap = new Dictionary<int, List<Agent>>();

    private DataModel resData;

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
                Debug.Log(jsonResponse);
                DataModel resModel = JsonUtility.FromJson<DataModel>(jsonResponse);
                resData = resModel;

                ProcessData();


            }
        }
    }


    // Start is called before the first frame update

    IEnumerator Start()
    {
        yield return StartCoroutine(GetData());
        ProcessData();
        foodManager.OnDataLoaded(foodMap);
        agentManager.OnDataLoaded(agentMap);
    }

    public void ProcessData()
    {
        foodMap.Clear();
        agentMap.Clear();

        for (int i = 0; i < resData.data.Count; i++)
        {
            ResModel resModel = resData.data[i];


            List<Food> foodList = new List<Food>();

            if (resModel.Food.Count != 0)
            {
                for (int j = 0; j < resModel.Food.Count; j++)
                {
                    Food food = resModel.Food[j];
                    foodList.Add(food);
                }
            }

            foodMap.Add(i, foodList);

            List<Agent> agentList = new List<Agent>();


            if (resModel.Agents.Count != 0)
            {
                for (int j = 0; j < resModel.Agents.Count; j++)
                {
                    Agent agent = resModel.Agents[j];
                    agentList.Add(agent);
                }
            }

            agentMap.Add(i, agentList);

        }

        foodManager.foodMap = foodMap;
        agentManager.agentMap = agentMap;



    }

    // Update is called once per frame
    void Update()
    {

    }
}