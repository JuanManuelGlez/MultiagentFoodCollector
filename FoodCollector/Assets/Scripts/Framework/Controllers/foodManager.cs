using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class FoodManager : MonoBehaviour
{
    public Dictionary<int, List<Food>> foodMap = new Dictionary<int, List<Food>>();
    public GameObject foodPrefab;
    private int step = 0;

    void Start()
    {
        Debug.Log("FoodManager Start");
        StartCoroutine(WaitForData());
    }

    IEnumerator WaitForData()
    {
        while (foodMap.Count == 0)
        {
            yield return null;
        }

        Debug.Log("FoodMap count: " + foodMap.Count);

        // Print the foodMap
        for (int i = 0; i < foodMap.Count; i++)
        {
            List<Food> foodList = foodMap[i];
            Debug.Log("FoodList count: " + foodList.Count);
            for (int j = 0; j < foodList.Count; j++)
            {
                Food food = foodList[j];
                Debug.Log("Food: " + food.x + ", " + food.z);
            }
        }

        // Generate food
        StartCoroutine(GenerateFood());
    }

    IEnumerator GenerateFood()
    {



        while (step < foodMap.Count)
        {
            GameObject[] existingFood = GameObject.FindGameObjectsWithTag("Food");
            foreach (GameObject foodObject in existingFood)
            {
                Destroy(foodObject);
            }
            List<Food> foodList = foodMap[step];
            for (int i = 0; i < foodList.Count; i++)
            {
                Food food = foodList[i];
                GameObject foodObject = Instantiate(foodPrefab, new Vector3(food.x, 0, food.z), Quaternion.identity);
                foodObject.name = "Food" + food.x + food.z;
                foodObject.tag = "Food";
            }
            step++;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void OnDataLoaded(Dictionary<int, List<Food>> loadedData)
    {
        foodMap = loadedData;
    }
}
