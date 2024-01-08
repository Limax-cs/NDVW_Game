using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public static int RedCrystalCount = 0;
    public static int BlueCrystalCount = 0;
    public static int PurpleCrystalCount = 0;
    public static int GemCrystalCount = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CompareTag("RedCrystal"))
            {
                RedCrystalCount++;
                Debug.Log("Red Crystal collected. Total: " + RedCrystalCount);
            }
            else if (CompareTag("BlueCrystal"))
            {
                BlueCrystalCount++;
                Debug.Log("Blue Crystal collected. Total: " + BlueCrystalCount);
            }
            else if (CompareTag("PurpleCrystal"))
            {
                PurpleCrystalCount++;
                Debug.Log("Purple Crystal collected. Total: " + PurpleCrystalCount);
            }
            else if (CompareTag("GemCrystal"))
            {
                GemCrystalCount++;
                Debug.Log("Gem Crystal collected. Total: " + GemCrystalCount);
            }

            Debug.Log("Crystal touched by: " + other.name);
            Destroy(gameObject);
        }
    }
}
