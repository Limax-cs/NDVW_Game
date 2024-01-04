using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Crystal touched by: " + other.name);
        //Destroy(gameObject);
        if (other.CompareTag("Player")) // Assuming your player GameObject is tagged as "Player"
        {
            Debug.Log("Crystal touched by: " + other.name);
            Destroy(gameObject);
        }
    }
}
