using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class FoodManager : MonoBehaviour
{
    public Dictionary<int, List<Food>> foodMap = new Dictionary<int, List<Food>>();

    // Steps and changeRoles map
    public Dictionary<int, List<bool>> stepChangeRolesMap = new Dictionary<int, List<bool>>();

    public GameObject foodPrefab;

    private int step = 0;

    private float rotation = 0.0f;


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

        // Generate food
        StartCoroutine(GenerateFood());
    }

    IEnumerator GenerateFood()
    {
        while (step < foodMap.Count)
        {
            if (stepChangeRolesMap[step][0])
            {
                GameObject[] existingFood = GameObject.FindGameObjectsWithTag("Food");
                foreach (GameObject foodObject in existingFood)
                {
                    // Get the name of the food object
                    string foodObjectName = foodObject.name;

                    // Use regular expression to extract the number from the name
                    Match match = Regex.Match(foodObjectName, @"\d+");
                    if (match.Success)
                    {
                        // Convert the matched value to an integer
                        int numberInName = int.Parse(match.Value);

                        // Check if the number is greater than 50
                        if (numberInName > 50)
                        {
                            // Destroy the food object
                            Destroy(foodObject);
                        }
                    }
                }

                List<Food> foodList = foodMap[step];
                for (int i = 0; i < foodList.Count; i++)
                {
                    Food food = foodList[i];



                    GameObject foodObject = Instantiate(foodPrefab, new Vector3(food.x, 0, food.z), Quaternion.identity);
                    if (food.x == 0 && !(food.z == 0))
                    {
                        foodObject.name = "Food" + 100 * food.z;
                    }
                    else if (food.z == 0 && !(food.x < 0))
                    {
                        foodObject.name = "Food" + food.x * 100;
                    }
                    else if (food.z == 0 && (food.x == 0))
                    {
                        foodObject.name = "Food" + 100 * i;
                    }
                    else
                    {
                        foodObject.name = "Food" + (food.x * food.z) * 50;
                    }
                    foodObject.tag = "Food";
                }
            }
            else
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
                    if (food.x == 0 && !(food.z == 0))
                    {
                        foodObject.name = "Food" + 100 * food.z;
                    }
                    else if (food.z == 0 && !(food.x < 0))
                    {
                        foodObject.name = "Food" + food.x * 100;
                    }
                    else if (food.z == 0 && (food.x == 0))
                    {
                        foodObject.name = "Food" + 100 * i;
                    }
                    else
                    {
                        foodObject.name = "Food" + (food.x * food.z) * 50;
                    }
                    foodObject.tag = "Food";
                }
            }
            step++;
            yield return new WaitForSeconds(ParamManager.speed);
        }
    }

    public void OnDataLoaded(Dictionary<int, List<Food>> loadedData, Dictionary<int, List<bool>> stepsData)
    {
        stepChangeRolesMap = stepsData;
        foodMap = loadedData;
    }
}
