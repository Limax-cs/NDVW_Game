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
                GWorld.Instance.GetWorld().ModifyState("Player Red Crystal Count", 1);
                //Debug.Log("Red Crystal collected. Total: " + RedCrystalCount);
            }
            else if (CompareTag("BlueCrystal"))
            {
                BlueCrystalCount++;
                GWorld.Instance.GetWorld().ModifyState("Player Blue Crystal Count", 1);
                //Debug.Log("Blue Crystal collected. Total: " + BlueCrystalCount);
            }
            else if (CompareTag("PurpleCrystal"))
            {
                PurpleCrystalCount++;
                GWorld.Instance.GetWorld().ModifyState("Player Purple Crystal Count", 1);
                //Debug.Log("Purple Crystal collected. Total: " + PurpleCrystalCount);
            }
            else if (CompareTag("GemCrystal"))
            {
                GemCrystalCount++;
                GWorld.Instance.GetWorld().ModifyState("Player Gem Crystal Count", 1);
                //Debug.Log("Gem Crystal collected. Total: " + GemCrystalCount);
            }

            //Debug.Log("Crystal touched by: " + other.name);
            GWorld.Instance.GetWorld().ModifyState("Player Crystal Count", 1);
            Destroy(gameObject);
        }
        else if (other.CompareTag("mole"))
        {
            if (CompareTag("RedCrystal"))
            {
                RedCrystalCount++;
                GWorld.Instance.GetWorld().ModifyState("Rival Red Crystal Count", 1);
                //Debug.Log("Red Crystal collected. Total: " + RedCrystalCount);
            }
            else if (CompareTag("BlueCrystal"))
            {
                BlueCrystalCount++;
                GWorld.Instance.GetWorld().ModifyState("Rival Blue Crystal Count", 1);
                //Debug.Log("Blue Crystal collected. Total: " + BlueCrystalCount);
            }
            else if (CompareTag("PurpleCrystal"))
            {
                PurpleCrystalCount++;
                GWorld.Instance.GetWorld().ModifyState("Rival Purple Crystal Count", 1);
                //Debug.Log("Purple Crystal collected. Total: " + PurpleCrystalCount);
            }
            else if (CompareTag("GemCrystal"))
            {
                GemCrystalCount++;
                GWorld.Instance.GetWorld().ModifyState("Rival Gem Crystal Count", 1);
                //Debug.Log("Gem Crystal collected. Total: " + GemCrystalCount);
            }

            //Debug.Log("Crystal touched by: " + other.name);
            GWorld.Instance.GetWorld().ModifyState("Rival Crystal Count", 1);
            Destroy(gameObject);
        }
    }
}
