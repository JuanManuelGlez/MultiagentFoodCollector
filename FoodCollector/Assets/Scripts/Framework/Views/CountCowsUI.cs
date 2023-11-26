using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountCowsUI : MonoBehaviour
{
    public Text CountCows;
    public StorageManager storageManager;

    void Start()
    {
        StartCoroutine(WaitForDataAndSetText());
    }

    IEnumerator WaitForDataAndSetText()
    {
        yield return StartCoroutine(WaitForData());

        // Now that WaitForData has completed, start the updateText coroutine repeatedly
        while (true)
        {
            yield return StartCoroutine(updateText());
            yield return new WaitForSeconds(ParamManager.speed);
        }
    }

    IEnumerator WaitForData()
    {
        while (storageManager.storageMap.Count == 0)
        {
            yield return null;
        }
    }

    
    /// <summary>
    /// Despliegue en UI de mensaje de contador de vacas
    /// </summary>
    IEnumerator updateText()
    {
        gameObject.SetActive(true);
        int countCows = storageManager.getNumberCows();
        CountCows.text = "Cows pulled: "+ countCows.ToString();
        yield return null;
    }
}
