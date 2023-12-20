using UnityEngine;

public class ScentSource : MonoBehaviour
{
    public string scentType; // Type of scent, can be used to trigger different behaviors in NPCs
    public float scentIntensity; // Intensity of the scent, could be used to affect detection range or NPC reaction

    // Optional: A method to change the scent properties dynamically, if needed
    public void SetScent(string type, float intensity)
    {
        scentType = type;
        scentIntensity = intensity;
    }

    // Additional methods can be added here to handle dynamic changes in scent properties
    // For example, reducing intensity over time, changing scent type, etc.
}
