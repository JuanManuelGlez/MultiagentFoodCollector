using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public Dictionary<int, List<Agent>> agentMap = new Dictionary<int, List<Agent>>();
    public GameObject AgentExplorerPrefab;

    public GameObject AgentCollectorPrefab;


    private int step = 0;

    private Dictionary<int, GameObject> agentObjects = new Dictionary<int, GameObject>();

    void Start()
    {
        Debug.Log("AgentManager Start");
        StartCoroutine(WaitForData());
    }

    IEnumerator WaitForData()
    {
        while (agentMap.Count == 0)
        {
            yield return null;
        }

        StartCoroutine(GenerateAgent());
    }

    IEnumerator GenerateAgent()
    {
        while (step < agentMap.Count)
        {
            List<Agent> agentList = agentMap[step];

            foreach (Agent agent in agentList)
            {
                if (!agentObjects.ContainsKey(agent.id))
                {
                    // Create new agent if it doesn't exist
                    if (agent.type == 1)
                    {
                        // Create explorer
                        GameObject agentObject = Instantiate(AgentExplorerPrefab, new Vector3(agent.x, 0, agent.z), Quaternion.identity);
                        agentObject.name = "Agent" + agent.id;
                        agentObject.tag = "Agent";

                        agentObjects.Add(agent.id, agentObject);
                    }
                    else
                    {
                        // Create collector
                        GameObject agentObject = Instantiate(AgentCollectorPrefab, new Vector3(agent.x, 0, agent.z), Quaternion.identity);
                        agentObject.name = "Agent" + agent.id;
                        agentObject.tag = "Agent";

                        agentObjects.Add(agent.id, agentObject);
                    }
                }
                else
                {
                    // Update the position of existing agent
                    GameObject agentObject = agentObjects[agent.id];
                    StartCoroutine(MoveAgent(agentObject.transform, new Vector3(agent.x, 0, agent.z), 0.1f));
                }
            }

            step++;
            yield return new WaitForSeconds(0.1f);
        }
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

    public void OnDataLoaded(Dictionary<int, List<Agent>> loadedData)
    {
        agentMap = loadedData;
    }
}
