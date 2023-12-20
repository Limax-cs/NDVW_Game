using UnityEngine;
using UnityEngine.AI; // For navigation

public class ScentDetector : MonoBehaviour
{
    public float detectionRadius = 5f;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        Debug.Log("Scent agent: "+ navMeshAgent);
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            ScentSource scent = hitCollider.GetComponent<ScentSource>();
            Debug.Log("Scent: "+ scent);
            if (scent != null)
            {
                MoveTowardsScent(scent.transform.position);
                break; // Assuming you want to react to the first scent detected
            }
        }
    }

    void MoveTowardsScent(Vector3 scentPosition)
    {
        // Set the destination for the NPC to move towards the scent
        navMeshAgent.SetDestination(scentPosition);
    }
}
