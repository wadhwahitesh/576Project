using UnityEngine;
using UnityEngine.AI;

public class BouncBall : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Check if the agent is currently on the NavMesh
        GameObject claire = GameObject.Find("Claire");
        

        Vector3 claire_position = claire.transform.position;

        agent.SetDestination(claire_position);
    }
}
