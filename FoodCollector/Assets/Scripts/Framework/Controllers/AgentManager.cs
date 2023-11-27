using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public Dictionary<int, List<Agent>> agentMap = new Dictionary<int, List<Agent>>();

    public GameObject AgentExplorerPrefab;

    public GameObject AgentCollectorPrefab;

    [SerializeField]
    public GameObject paramManagerHolder;

    ParamManager paramManager;

    private int step = 0;

    private float levitate = 4.0f;

    private Dictionary<int, GameObject> agentObjects = new Dictionary<int, GameObject>();

    void Awake()
    {
        paramManager = paramManagerHolder.GetComponent<ParamManager>();
    }

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
                        GameObject agentObject = Instantiate(AgentExplorerPrefab, new Vector3(agent.x * ParamManager.distanceMultiplier, levitate, agent.z * ParamManager.distanceMultiplier), Quaternion.identity);
                        agentObject.transform.localScale = new Vector3(7, 7, 7);
                        agentObject.name = "Agent" + agent.id;
                        agentObject.tag = "AgentExplorer";

                        agentObjects.Add(agent.id, agentObject);
                    }
                    else
                    {

                        // delete the Explorer agent
                        GameObject[] explorerAgents = GameObject.FindGameObjectsWithTag("AgentExplorer");

                        foreach (GameObject explorerAgent in explorerAgents)
                        {
                            Destroy(explorerAgent);
                        }

                        // Create collector
                        GameObject agentObject = Instantiate(AgentCollectorPrefab, new Vector3(agent.x * ParamManager.distanceMultiplier, levitate, agent.z * ParamManager.distanceMultiplier), Quaternion.identity);
                        agentObject.transform.localScale = new Vector3(5, 5, 5);
                        agentObject.name = "Agent" + agent.id;
                        agentObject.tag = "AgentCollector";

                        agentObjects.Add(agent.id, agentObject);
                    }
                }
                else
                {
                    // Update the position of existing agent
                    GameObject agentObject = agentObjects[agent.id];
                    StartCoroutine(MoveAgent(agentObject.transform, new Vector3(agent.x * ParamManager.distanceMultiplier, 1, agent.z * ParamManager.distanceMultiplier), ParamManager.speed));
                }
            }

            step++;
            yield return new WaitForSeconds(ParamManager.speed);
        }
    }

    IEnumerator MoveAgent(Transform agentTransform, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = agentTransform.position;

        // Interpolation with value of 1
        Vector3 targetPositionWithConstantY = new Vector3(targetPosition.x, 1, targetPosition.z);

        while (elapsedTime < duration)
        {
            Vector3 intermediatePos = Vector3.Lerp(startingPos, targetPositionWithConstantY, elapsedTime / duration);
            intermediatePos.y = levitate;
            agentTransform.position = intermediatePos;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Get the movement in y again
        targetPositionWithConstantY.y = levitate;
        agentTransform.position = targetPositionWithConstantY;
    }

    public void OnDataLoaded(Dictionary<int, List<Agent>> loadedData)
    {
        agentMap = loadedData;
    }
}
