using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assuming your player GameObject is tagged as "Player"
        {
            Collect();
        }
    }

    void Collect()
    {
        // Here you can add logic to update the player's inventory or score

        // Destroy the crystal GameObject
        Destroy(gameObject);
    }
}
