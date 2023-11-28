using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class stepManager : MonoBehaviour
{
    public Text stepCounterText;

    private int stepCounter = 0;

    private int currStep = 0;

    void Start()
    {
        StartCoroutine(WaitForDataAndSetText());
    }

    IEnumerator WaitForDataAndSetText()
    {
        yield return StartCoroutine(WaitForData());


        while (true)
        {
            yield return StartCoroutine(updateText());
            yield return new WaitForSeconds(ParamManager.speed);
        }
    }

    IEnumerator WaitForData()
    {
        while (stepCounter == 0)
        {
            stepCounter = WebClient.stepCounter;
            Debug.Log("stepCounter: " + stepCounter);
            yield return null;
        }
    }


    /// <summary>
    /// Despliegue en UI de mensaje de contador de vacas
    /// </summary>
    IEnumerator updateText()
    {
        if (stepCounterText != null)
        {
            if (currStep < stepCounter)
            {
                stepCounterText.text = "Step: " + currStep.ToString();
                currStep = currStep + 1;
                yield return null;
            }
            else
            {
                stepCounterText.text = "Step: " + currStep.ToString();
                yield return null;
            }
        }
    }
}
