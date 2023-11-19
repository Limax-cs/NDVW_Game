using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject entityPrefab;
    public int numEntities;
    public bool spawnPeriodically = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numEntities; i++)
        {
            Instantiate(entityPrefab, this.transform.position, Quaternion.identity);
        }

        if (spawnPeriodically)
            Invoke("SpawnEntity", 5);
    }

    void SpawnEntity()
    {
        Instantiate(entityPrefab, this.transform.position, Quaternion.identity);
        Invoke("SpawnEntity", Random.Range(2,10));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
