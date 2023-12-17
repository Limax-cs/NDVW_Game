using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class collectableItem : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Player GameObject is tagged as "Player"
        {
            Collect();
        }
    }

    void Collect()
    {
        //logic to update the player's inventory or score

        // Destroy the crystal GameObject
        Destroy(gameObject);
    }
}
