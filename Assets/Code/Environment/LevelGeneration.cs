using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Unity.AI.Navigation;
using System.Diagnostics;
using System.Linq;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField]
    private int mapWidthInTiles, mapDepthInTiles;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    public int tileScale = 1;

    public GameObject base1Prefab;
    public GameObject base2Prefab;
    private bool instantiateBases = false;

    [SerializeField]
    public NavMeshSurface surface;

    [SerializeField]
    private List<GameObject> spaceship1Prefab;

    [SerializeField]
    private List<GameObject> spaceship2Prefab;

    [SerializeField]
    private int items2collect = 4;


    void Awake()
    {
        GenerateMap();
        //instantiateBases = true;
        spawnBases();
        surface.BuildNavMesh();
        spawnItems();
    }

    void Update()
    {
        /*
        if (!instantiateBases)
        {
            spawnBases();
            instantiateBases = true;
            surface.BuildNavMesh();
        }*/
    }

    void GenerateMap()
    {
        // get the tile dimensions from the tile Prefab
        //tilePrefab.transform.localScale *= 10.0f;
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size * tileScale;
        UnityEngine.Debug.Log(tileSize);
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        // for each Tile, instantiate a Tile in the correct position
        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
            {
                //Vector3 originalTileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;

                // calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new Vector3(this.gameObject.transform.position.x + xTileIndex * tileWidth,
                  this.gameObject.transform.position.y,
                  this.gameObject.transform.position.z + zTileIndex * tileDepth);
                UnityEngine.Debug.Log(tilePosition);
                // instantiate a new Tile
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                //if (xTileIndex == mapWidthInTiles - 1 && zTileIndex == mapDepthInTiles - 1)
                //{
                //    tile.GetComponent<NavMeshSurface>().BuildNavMesh();
                //}
            }
        }
        //Destroy(tilePrefab);
        
        //GameObject firstTile = GameObject.Find("Level Tile(Clone)");
        //firstTile.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    private void spawnBases()
    {
        // Extract tile size
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size * tileScale;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        Vector3 origin = new Vector3((float)tileWidth*mapWidthInTiles/2, 0.0f, (float)tileDepth*mapDepthInTiles/2);
        //Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);
        UnityEngine.Debug.Log("Origin: " + origin + " | Base Width: " + tileWidth*(mapWidthInTiles-1.5f) + " | Base Depth: " + (int)(tileDepth*(mapDepthInTiles-1.5f)));
        RandomLocationGenerator rlg = new RandomLocationGenerator(origin, (int)(tileWidth*(mapWidthInTiles-1.5f)), (int)(tileDepth*(mapWidthInTiles-1.5f)));
        Vector3 castDirection = Vector3.down;
        float raycastOffset = 50f;
        string[] biomeTags = { "Desert", "Forest", "Snowy", "Rocky" };

        // Spaceship positions
        Vector3 spawnLoc1 = rlg.getRandomLocation();
        Vector3 spawnLoc2 = rlg.getRandomLocation();
        if (Vector3.Distance(spawnLoc1, spawnLoc2) < 50)
        {
            Vector3 locDiff = spawnLoc1 - spawnLoc2;
            spawnLoc1 = spawnLoc1 - 25*locDiff.normalized; 
            spawnLoc2 = spawnLoc2 + 25*locDiff.normalized;
        }
        UnityEngine.Debug.Log("Base 1: " + spawnLoc1);
        UnityEngine.Debug.Log("Base 2: " + spawnLoc2);


        // Spaceship Location
        Vector3 raycastStart = new Vector3(spawnLoc1.x, raycastOffset, spawnLoc1.z);
        RaycastHit hit;
        if (Physics.Raycast(raycastStart, Vector3.down, out hit))
        {
            UnityEngine.Debug.Log("Base 1");
            spawnLoc1 = hit.point + new Vector3(0.0f, 0.0f, 0.0f); // Set the location to the point where the ray hits the surface
            Instantiate(base1Prefab, spawnLoc1, Quaternion.FromToRotation(Vector3.up, hit.normal));
            
        }

        raycastStart = new Vector3(spawnLoc2.x, raycastOffset, spawnLoc2.z);
        if (Physics.Raycast(raycastStart, Vector3.down, out hit))
        {
            UnityEngine.Debug.Log("Base 2");
            spawnLoc2 = hit.point + new Vector3(0.0f, 0.0f, 0.0f); // Set the location to the point where the ray hits the surface
            Instantiate(base2Prefab, spawnLoc2, Quaternion.FromToRotation(Vector3.up, hit.normal));
            
        }
    }

    private void spawnItems()
    {
        // Extract tile size
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size * tileScale;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        Vector3 origin = new Vector3((float)tileWidth*mapWidthInTiles/2, 0.0f, (float)tileDepth*mapDepthInTiles/2);
        RandomLocationGenerator rlg = new RandomLocationGenerator(origin, (int)(tileWidth*(mapWidthInTiles-1.5f)), (int)(tileDepth*(mapWidthInTiles-1.5f)));
        Vector3 castDirection = Vector3.down;
        float raycastOffset = 50f;
        string[] biomeTags = { "Desert", "Forest", "Snowy", "Rocky" };


        // Babo Team
        for (int i = 0; i < items2collect; i++)
        {
            Vector3 spawnLoc = rlg.getRandomLocation();
            Vector3 raycastStart = new Vector3(spawnLoc.x, raycastOffset, spawnLoc.z);
            RaycastHit hit;
            if (Physics.Raycast(raycastStart, Vector3.down, out hit))
            {
                spawnLoc = hit.point + new Vector3(0.0f, 10f, 0.0f); // Set the location to the point where the ray hits the surface
                if (spaceship1Prefab.Count > 0)
                {
                    int spaceshipIdx = UnityEngine.Random.Range(0, spaceship1Prefab.Count);
                    Instantiate(spaceship1Prefab[spaceshipIdx], spawnLoc, Quaternion.identity);
                } 
                
            }
        }

        // Mole Team
        for (int i = 0; i < items2collect; i++)
        {
            Vector3 spawnLoc = rlg.getRandomLocation();
            Vector3 raycastStart = new Vector3(spawnLoc.x, raycastOffset, spawnLoc.z);
            RaycastHit hit;
            if (Physics.Raycast(raycastStart, Vector3.down, out hit))
            {
                spawnLoc = hit.point + new Vector3(0.0f, 10f, 0.0f); // Set the location to the point where the ray hits the surface
                if (spaceship2Prefab.Count > 0)
                {
                    UnityEngine.Debug.Log("C");
                    int spaceshipIdx = UnityEngine.Random.Range(0, spaceship2Prefab.Count);
                    Instantiate(spaceship2Prefab[spaceshipIdx], spawnLoc, Quaternion.identity);
                } 
                
            }
        }
    }
}