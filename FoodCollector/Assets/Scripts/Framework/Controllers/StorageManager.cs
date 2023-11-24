using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class StorageManager : MonoBehaviour
{
    // Storage map
    public Dictionary<int, List<Storage>> storageMap = new Dictionary<int, List<Storage>>();

     // Steps and changeRoles map
    public Dictionary<int, bool> stepChangeRolesMap = new Dictionary<int, bool>();

    public GameObject foodPrefab;

    private int step;
    private bool isChangedRoles;

    private int numberCows;

    private Vector3 depositPosition;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("StorageManager Start");
        depositPosition = Vector3.zero;
        numberCows = 0;
        StartCoroutine(WaitForData());
    }

    IEnumerator WaitForData()
    {
        while (storageMap.Count == 0)
        {
            yield return null;
        }
        // Only stack cows if isChangedRoles
        StartCoroutine(getCows());
    }

    IEnumerator getCows()
    {
        while (step < storageMap.Count)
        {
            if(stepChangeRolesMap[step])
            {
                int cows = 0;
                List<Storage> storageList = storageMap[step];
                for (int i = 0; i < storageList.Count; i++)
                {
                    Storage storage = storageList[i];
                    if(storage.x != null)
                    {
                        depositPosition.x = storage.x;
                    }
                    if(storage.z != null)
                    {
                        depositPosition.z = storage.z;
                    }
                    if (storage.depositQuantity > 0)
                    {
                        cows += storage.depositQuantity;
                    }
                }
                if (numberCows <= cows)
                {
                    StartCoroutine(stackCows(cows, numberCows));
                    numberCows += cows;
                }
            }
            step++;
            yield return new WaitForSeconds(ParamManager.speed);
        }
    }


    public void OnDataLoaded(Dictionary<int, List<Storage>> loadedData, Dictionary<int, bool> stepsData)
    {
        storageMap = loadedData;
        stepChangeRolesMap = stepsData;
    }

    IEnumerator stackCows(int cows, int startNumber)
    {
        float verticalOffset = 0.5f;
        for (int i = startNumber; i <= cows; i++)
        {
            Debug.Log("Number of cows: " + numberCows);
            Vector3 spawnPosition = depositPosition + new Vector3(0f, i * verticalOffset, 0f);
            Quaternion rotation = foodPrefab.transform.rotation;
            GameObject foodObject = Instantiate(foodPrefab, spawnPosition, rotation);
            foodObject.name = "Food" + i;
            foodObject.tag = "Food";
            yield return new WaitForSeconds(ParamManager.speed);
        }
    }
}
