using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Debug.Log("Crystal touched by: " + other.name);
            Destroy(gameObject);
        }
    }
}
