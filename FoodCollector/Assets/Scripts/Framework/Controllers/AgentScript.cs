// AgentScript.cs
using UnityEngine;
using System.Collections;

public class AgentScript : MonoBehaviour
{
    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        StartCoroutine(MoveAgent(transform, targetPosition, duration));
        yield return null;
    }

    IEnumerator MoveAgent(Transform agentTransform, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = agentTransform.position;

        while (elapsedTime < duration)
        {
            agentTransform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        agentTransform.position = targetPosition;
    }
}
