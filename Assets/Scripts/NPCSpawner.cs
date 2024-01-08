using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] npcPrefabs; // Array of NPC prefabs
    public int numberOfNPCs = 5;
    public float terrainWidth = 50f;
    public float terrainLength = 50f;
    public float yOffset = 1f;

    void Start()
    {
        for (int i = 0; i < numberOfNPCs; i++)
        {
            SpawnNPC();
        }
    }

    void SpawnNPC()
    {
        float xPos = Random.Range(-terrainWidth / 2, terrainWidth / 2);
        float zPos = Random.Range(-terrainLength / 2, terrainLength / 2);
        Vector3 npcPosition = new Vector3(xPos, yOffset, zPos);

        // Choose a random NPC prefab from the array
        GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        // Instantiate the chosen NPC prefab
        Instantiate(npcPrefab, npcPosition, Quaternion.identity);
    }
}
