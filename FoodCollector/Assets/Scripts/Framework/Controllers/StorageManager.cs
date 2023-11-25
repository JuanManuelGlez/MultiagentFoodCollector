using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class StorageManager : MonoBehaviour
{
    // Storage map
    public Dictionary<int, List<Storage>> storageMap = new Dictionary<int, List<Storage>>();

     // Steps and changeRoles map
    public Dictionary<int, List<bool>> stepChangeRolesMap = new Dictionary<int, List<bool>>();

    public GameObject foodPrefab;

    public GameObject blackHole;

    public float verticalOffsetCows = 3.5f;

    private int step;
    private bool isChangedRoles;

    private int numberCows;
    private bool instantiateBlackHole;

    private Vector3 depositPosition;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("StorageManager Start");
        depositPosition = Vector3.zero;
        numberCows = 0;
        instantiateBlackHole = false;
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
            // Instantiate at position the black hole
            if(stepChangeRolesMap[step][1] && !instantiateBlackHole)
            {
                Vector3 positionBlackHole;

                List<Storage> tempList = storageMap[step];
                positionBlackHole = new Vector3(tempList[0].x, 1.2f, tempList[0].z);

                depositPosition = new Vector3(tempList[0].x, 0.0f, tempList[0].z);
                StartCoroutine(generateBlackHole(positionBlackHole));
                instantiateBlackHole = true;
            }

            if(stepChangeRolesMap[step][0])
            {
                int cows = 0;
                List<Storage> storageList = storageMap[step];
                for (int i = 0; i < storageList.Count; i++)
                {
                    Storage storage = storageList[i];
                    if (storage.depositQuantity > 0)
                    {
                        cows = storage.depositQuantity;
                    }
                }
                if (numberCows < cows)
                {
                    StartCoroutine(stackCows(cows, numberCows));
                    numberCows = cows;
                }
            }
            step++;
            yield return new WaitForSeconds(ParamManager.speed);
        }
    }


    public void OnDataLoaded(Dictionary<int, List<Storage>> loadedData, Dictionary<int, List<bool>> stepsData)
    {
        storageMap = loadedData;
        stepChangeRolesMap = stepsData;
    }

    IEnumerator generateBlackHole(Vector3 positionBlackHole)
    {
        GameObject blackHoleObject = Instantiate(blackHole, positionBlackHole, Quaternion.identity);
        yield return new WaitForSeconds(ParamManager.speed);
    }

    IEnumerator stackCows(int cows, int startNumber)
    {
        for (int i = startNumber; i <= cows; i++)
        {
            Debug.Log("Number of cows: " + numberCows);
            Vector3 spawnPosition = depositPosition + new Vector3(0f, verticalOffsetCows, 0f);
            Quaternion rotation = foodPrefab.transform.rotation;
            rotation *= Quaternion.Euler(0f, 0f, 90f);
            GameObject foodObject = Instantiate(foodPrefab, spawnPosition, rotation);

            // Attach a Collider component
            foodObject.AddComponent<BoxCollider>();

            // Attach a SingularityPullable script
            foodObject.AddComponent<SingularityPullable>();

            // Wait for one frame to ensure that the collider is initialized
            yield return null;

            // Attach a Rigidbody component to enable physics
            Rigidbody foodRigidbody = foodObject.AddComponent<Rigidbody>();

            // Adjust Rigidbody settings (if needed)
            foodRigidbody.mass = 10.0f;
            foodRigidbody.drag = 1.5f;

            foodObject.name = "Food" + i;
            foodObject.tag = "Food";

            
            yield return new WaitForSeconds(ParamManager.speed);
        }
    }
}
