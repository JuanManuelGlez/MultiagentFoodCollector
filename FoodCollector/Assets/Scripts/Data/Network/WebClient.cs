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

    public StorageManager storageManager;

    public Dictionary<int, List<Food>> foodMap = new Dictionary<int, List<Food>>();

    // Agent map
    public Dictionary<int, List<Agent>> agentMap = new Dictionary<int, List<Agent>>();

    // Storage map
    public Dictionary<int, List<Storage>> storageMap = new Dictionary<int, List<Storage>>();

    // Steps and changeRoles map
    // [0] = CHANGE ROLES
    // [1] = FOUND DEPOSIT
    public Dictionary<int, List<bool>> stepChangeRolesMap = new Dictionary<int, List<bool>>();
    public int step = 0;

    public static int stepCounter { get; private set; }

    public bool isChangedRoles = false;

    private DataModel resData;

    void Awake()
    {
        stepCounter = 0;
    }

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
            }
        }
    }


    // Start is called before the first frame update

    IEnumerator Start()
    {
        yield return StartCoroutine(GetData());
        ProcessData();
        foodManager.OnDataLoaded(foodMap, stepChangeRolesMap);
        agentManager.OnDataLoaded(agentMap);
        storageManager.OnDataLoaded(storageMap, stepChangeRolesMap);
    }

    public void ProcessData()
    {
        foodMap.Clear();
        agentMap.Clear();
        storageMap.Clear();
        stepChangeRolesMap.Clear();

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

            List<Storage> storageData = new List<Storage>();

            if (resModel.Storage.Count != 0)
            {
                // Iterate every element inside the storage JSON list storage
                for (int j = 0; j < resModel.Storage.Count; j++)
                {
                    Storage storage = resModel.Storage[j];
                    storageData.Add(storage);
                }
            }
            List<bool> stepChangeRolesList = new List<bool>();
            stepChangeRolesList.Add(resModel.isChangedRoles);
            stepChangeRolesList.Add(resModel.foundDeposit);
            stepChangeRolesMap[--resModel.step] = stepChangeRolesList;
            // Add the step as the key and JSON list object as values
            storageMap.Add(i, storageData);

        }

        stepCounter = resData.data.Count;
        foodManager.foodMap = foodMap;
        agentManager.agentMap = agentMap;
        storageManager.storageMap = storageMap;
        storageManager.stepChangeRolesMap = stepChangeRolesMap;
    }
}