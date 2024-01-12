// CREDIT FOR BASIS OF PCG SYSTEM: 
// https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System.Linq;
using System.IO;


public class TileGeneration : MonoBehaviour
{
    [SerializeField]
    public NoiseMapGeneration noiseMapGeneration;

    [SerializeField]
    public MeshRenderer tileRenderer;

    [SerializeField]
    public MeshFilter meshFilter;

    [SerializeField]
    public MeshCollider meshCollider;

    [SerializeField]
    public float mapScale;

    [SerializeField]
    private float heightMultiplier;

    [SerializeField]
    public int tileScale = 1;

    [SerializeField]
    public NavMeshSurface surface;

    [SerializeField]
    private GameObject explorePointPrefab;

    [SerializeField]
    private List<GameObject> weaponPrefab;

    [SerializeField]
    private int weaponPerTile = 4;

    [SerializeField]
    private NavMeshAgent navMeshAgent;
    //[SerializeField]
    //private AnimationCurve heightCurve;

    [SerializeField]
    private Wave[] waves;

    [SerializeField]
    private List<GameObject> forestNPCsPrefab;
    [SerializeField]
    private List<GameObject> desertNPCsPrefab;
    [SerializeField]
    private List<GameObject> rockNPCsPrefab;
    [SerializeField]
    private List<GameObject> snowNPCsPrefab;
    [SerializeField]
    private int entitiesPerTileMin = 0;
    [SerializeField]
    private int entitiesPerTileMax = 8;

    // Start is called before the first frame update
    void Awake()
    {
        GenerateTile();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateTile()
    {
        this.gameObject.transform.localScale = new Vector3(tileScale, tileScale, tileScale);
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        //UnityEngine.Debug.Log(tileDepth);
        int tileWidth = tileDepth;

        // calculate the offsets based on the tile position
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        // generate a heightMap using noise
        float[,] heightMap = this.noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, this.mapScale, offsetX, offsetZ, waves, tileScale);

        // build a Texture2D from the height map
        System.Random random = new System.Random();
        BiomeType currentBiome = (BiomeType)random.Next(Enum.GetNames(typeof(BiomeType)).Length);

        Texture2D myTexture;
        switch (currentBiome)
        {
            case BiomeType.Forest:
                //this.heightMultiplier = 3.0f;
                myTexture = Resources.Load("Textures/terrainFloor") as Texture2D;
                this.tag = "Forest";
                break;
            case BiomeType.Desert:
                //this.heightMultiplier = 1.0f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Sand_BaseColor") as Texture2D;
                this.tag = "Desert";
                break;
            case BiomeType.Rocky:
                //heightMultiplier = 3.0f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Rock_BaseColor") as Texture2D;
                this.tag = "Rocky";
                break;
            case BiomeType.Snowy:
                //heightMultiplier = 2.5f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Snow_BaseColor") as Texture2D;
                this.tag = "Snowy";
                break;
            default:
                //heightMultiplier = 1.0f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Grass_Moss_BaseColor") as Texture2D;
                this.tag = "Default";
                break;
        }

        this.tileRenderer.material.mainTexture = myTexture;

        UpdateMeshVertices(heightMap);

        // Spawn items
        SpawnTileObjects(this.gameObject, currentBiome);

        // Generate NavMesh for the tile
        //surface.BuildNavMesh();
        //navMeshAgent.NavMeshOwner = surface;

        //spawnMole();
    }

    private void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = this.meshFilter.mesh.vertices;

        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];

                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coord, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, height * this.heightMultiplier, vertex.z);

                vertexIndex++;
            }
        }

        //update the vertices in the mesh and update its properties
        this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        this.meshCollider.sharedMesh = this.meshFilter.mesh;
    }

    private void SpawnTileObjects(GameObject tileGameObject, BiomeType biomeType)
    {
        MeshRenderer renderer = tileGameObject.GetComponent<MeshRenderer>();
        Vector3 totalSize = renderer.bounds.size;
        RandomLocationGenerator rlg = new RandomLocationGenerator(tileGameObject.transform.position, (int)totalSize.x, (int)totalSize.z);

        Vector3 castDirection = Vector3.down;

        float raycastOffset = 50f;

        string[] biomeTags = { "Desert", "Forest", "Snowy", "Rocky" };
        int explorePointsPerBiome = 30;
        Vector3 spawnLoc;
        Vector3 raycastStart;
        RaycastHit hit;

        GameObject crystalPrefab = null;
        if (biomeType == BiomeType.Forest)
        {
            spawnExplorePoints(explorePointsPerBiome, rlg, 50.0f, biomeTags);
            crystalPrefab = Resources.Load("Toon Crystals pack/Prefabs/BlueCrystal00") as GameObject; 
            spawnCrystals(crystalPrefab, rlg);

            for(int i = 0; i < 40; i++)
            {
                for (int item_index = 1; item_index <= 2; item_index++)
                {
                    GameObject tmp_go = Resources.Load("Handpainted_Forest_pack/Models/Fir_v1_" + item_index) as GameObject;
                    spawnLoc = rlg.getRandomLocation();

                    // Adjust the raycast starting position above the tile
                    raycastStart = new Vector3(spawnLoc.x, tileGameObject.transform.position.y + raycastOffset, spawnLoc.z);

                    if (Physics.Raycast(raycastStart, castDirection, out hit))
                    {
                        spawnLoc = hit.point; // Set the location to the point where the ray hits the surface
                        if (biomeTags.Contains(hit.collider.gameObject.tag))
                        {
                            Instantiate(tmp_go, spawnLoc, Quaternion.identity);
                        }
                    }
                }
            }
            
            spawnWeapons(weaponPerTile, rlg, 50.0f, biomeTags);
            spawnForestEntity(rlg, 50.0f, biomeTags);
            //BatchSpawnTileObjects(tileGameObject, biomeType, 40, "Handpainted_Forest_pack/Models/Fir_v1_");
        }
        else if (biomeType == BiomeType.Desert)
        {
            spawnExplorePoints(explorePointsPerBiome, rlg, 50.0f, biomeTags);
            crystalPrefab = Resources.Load("Toon Crystals pack/Prefabs/RedCrystal08") as GameObject;
            spawnCrystals(crystalPrefab, rlg);

            for (int i = 0; i < 40; i++)
            {
                for (int item_index = 1; item_index <= 4; item_index++)
                {
                    GameObject tmp_go = Resources.Load("Free_Rocks/_prefabs/rock" + item_index) as GameObject;
                    spawnLoc = rlg.getRandomLocation();

                    // Adjust the raycast starting position above the tile
                    raycastStart = new Vector3(spawnLoc.x, tileGameObject.transform.position.y + raycastOffset, spawnLoc.z);

                    if (Physics.Raycast(raycastStart, castDirection, out hit))
                    {
                        spawnLoc = hit.point; // Set the location to the point where the ray hits the surface
                        if (biomeTags.Contains(hit.collider.gameObject.tag))
                        {
                            Instantiate(tmp_go, spawnLoc, Quaternion.identity);
                        }
                    }
                }
            }
            spawnWeapons(weaponPerTile, rlg, 50.0f, biomeTags);
            spawnDesertEntity(rlg, 50.0f, biomeTags);
            //BatchSpawnTileObjects(tileGameObject, biomeType, 30, "Free_Rocks/_prefabs/rock");
        }
        else if (biomeType == BiomeType.Snowy)
        {
            spawnExplorePoints(explorePointsPerBiome, rlg, 50.0f, biomeTags);
            crystalPrefab = Resources.Load("Toon Crystals pack/Prefabs/GemStone00") as GameObject; 
            spawnCrystals(crystalPrefab, rlg);
            for(int i = 0; i < 60; i++)
            {
                for (int item_index = 1; item_index <= 3; item_index++)
                {
                    GameObject tmp_go = Resources.Load("Handpainted_Forest_pack/Models/Grass_v1_" + item_index) as GameObject;
                    spawnLoc = rlg.getRandomLocation();

                    // Adjust the raycast starting position above the tile
                    raycastStart = new Vector3(spawnLoc.x, tileGameObject.transform.position.y + raycastOffset, spawnLoc.z);

                    if (Physics.Raycast(raycastStart, castDirection, out hit))
                    {
                        spawnLoc = hit.point; // Set the location to the point where the ray hits the surface
                        if (biomeTags.Contains(hit.collider.gameObject.tag))
                        {
                            Instantiate(tmp_go, spawnLoc, Quaternion.identity);
                        }
                    }
                }
            }
            spawnWeapons(weaponPerTile, rlg, 50.0f, biomeTags);
            spawnSnowEntity(rlg, 50.0f, biomeTags);
            //BatchSpawnTileObjects(tileGameObject, biomeType, 20, "Free_Rocks/_prefabs/rock");
        }
        else if (biomeType == BiomeType.Rocky)
        {
            spawnExplorePoints(explorePointsPerBiome, rlg, 50.0f, biomeTags);
            crystalPrefab = Resources.Load("Toon Crystals pack/Prefabs/PurpCrystal00") as GameObject;
            spawnCrystals(crystalPrefab, rlg);
            
            for (int i = 0; i < 80; i++)
            {
                for (int item_index = 1; item_index <= 4; item_index++)
                {
                    GameObject tmp_go = Resources.Load("Free_Rocks/_prefabs/rock" + item_index) as GameObject;
                    spawnLoc = rlg.getRandomLocation();

                    // Adjust the raycast starting position above the tile
                    raycastStart = new Vector3(spawnLoc.x, tileGameObject.transform.position.y + raycastOffset, spawnLoc.z);

                    if (Physics.Raycast(raycastStart, castDirection, out hit))
                    {
                        spawnLoc = hit.point; // Set the location to the point where the ray hits the surface
                        if (biomeTags.Contains(hit.collider.gameObject.tag))
                        {
                            Instantiate(tmp_go, spawnLoc, Quaternion.identity);
                        }
                    }
                }
            }
            spawnWeapons(weaponPerTile, rlg, 50.0f, biomeTags);
            spawnRockyEntity(rlg, 50.0f, biomeTags);
            //BatchSpawnTileObjects(tileGameObject, biomeType, 40, "Free_Rocks/_prefabs/rock");
        }
        
    }

    private void spawnExplorePoints(int desiredAmount, RandomLocationGenerator rlg, float raycastOffset, string[] biomeTags)
    {
        for (int i = 0; i < desiredAmount; i++)
        {
            Vector3 spawnLoc = rlg.getRandomLocation();
            Vector3 raycastStart = new Vector3(spawnLoc.x, this.gameObject.transform.position.y + raycastOffset, spawnLoc.z);
            RaycastHit hit;
            if (Physics.Raycast(raycastStart, Vector3.down, out hit))
            {
                spawnLoc = hit.point + new Vector3(0.0f, 0.5f, 0.0f); // Set the location to the point where the ray hits the surface
                if (biomeTags.Contains(hit.collider.gameObject.tag))
                {
                    Instantiate(explorePointPrefab, spawnLoc, Quaternion.identity);
                }
            }
        }
    }

    private void spawnCrystals(GameObject crystalPrefab, RandomLocationGenerator rlg)
    {
        int numberOfCrystals = 40;
        for (int i = 0; i < numberOfCrystals; i++)
        {
            GameObject crystalInstance = Instantiate(crystalPrefab, rlg.getRandomLocation(), Quaternion.identity);

            // Randomize scale
            float randomScaleFactor = UnityEngine.Random.Range(3.0f, 4.0f); // Adjust the range as needed
            crystalInstance.transform.localScale = new Vector3(randomScaleFactor, randomScaleFactor, randomScaleFactor);

            // Randomize rotation
            float randomRotationX = UnityEngine.Random.Range(0, 360);
            float randomRotationY = UnityEngine.Random.Range(0, 360);
            float randomRotationZ = UnityEngine.Random.Range(0, 360);
            crystalInstance.transform.rotation = Quaternion.Euler(randomRotationX, randomRotationY, randomRotationZ);

            float liftHeight = 0.02f * randomScaleFactor; // Adjust this value as needed

            RaycastHit hit;
            if (Physics.Raycast(crystalInstance.transform.position, Vector3.down, out hit))
            {
                crystalInstance.transform.position = hit.point + new Vector3(0, liftHeight, 0); // Set the location to the point where the ray hits the surface

            }
        } 
    }

    private void spawnWeapons(int desiredWeapons, RandomLocationGenerator rlg, float raycastOffset, string[] biomeTags)
    {
        for (int i = 0; i < desiredWeapons; i++)
        {
            Vector3 spawnLoc = rlg.getRandomLocation();
            Vector3 raycastStart = new Vector3(spawnLoc.x, this.gameObject.transform.position.y + raycastOffset, spawnLoc.z);
            RaycastHit hit;
            if (Physics.Raycast(raycastStart, Vector3.down, out hit))
            {
                spawnLoc = hit.point + new Vector3(0.0f, 0.5f, 0.0f); // Set the location to the point where the ray hits the surface
                if (biomeTags.Contains(hit.collider.gameObject.tag))
                {
                    if (weaponPrefab.Count > 0)
                    {
                        int weaponIdx = UnityEngine.Random.Range(0, weaponPrefab.Count);
                        Instantiate(weaponPrefab[weaponIdx], spawnLoc, Quaternion.identity);
                    } 
                }
            }
        }
    }

    // Entities
    private void spawnForestEntity(RandomLocationGenerator rlg, float raycastOffset, string[] biomeTags)
    {
        int entities = UnityEngine.Random.Range(entitiesPerTileMin, entitiesPerTileMax);
        for (int i = 0; i < entities; i++)
        {
            Vector3 spawnLoc = rlg.getRandomLocation();
            Vector3 raycastStart = new Vector3(spawnLoc.x, this.gameObject.transform.position.y + raycastOffset, spawnLoc.z);
            RaycastHit hit;
            if (Physics.Raycast(raycastStart, Vector3.down, out hit))
            {
                spawnLoc = hit.point + new Vector3(0.0f, 1.0f, 0.0f); // Set the location to the point where the ray hits the surface
                if ((forestNPCsPrefab.Count > 0)  && biomeTags.Contains(hit.collider.gameObject.tag))
                {
                    int entityIdx = UnityEngine.Random.Range(0, forestNPCsPrefab.Count);
                    Instantiate(forestNPCsPrefab[entityIdx], spawnLoc, Quaternion.identity);
                } 
            }
        }
    }

    private void spawnDesertEntity(RandomLocationGenerator rlg, float raycastOffset, string[] biomeTags)
    {
        int entities = UnityEngine.Random.Range(entitiesPerTileMin, entitiesPerTileMax);
        for (int i = 0; i < entities; i++)
        {
            Vector3 spawnLoc = rlg.getRandomLocation();
            Vector3 raycastStart = new Vector3(spawnLoc.x, this.gameObject.transform.position.y + raycastOffset, spawnLoc.z);
            RaycastHit hit;
            if (Physics.Raycast(raycastStart, Vector3.down, out hit))
            {
                spawnLoc = hit.point + new Vector3(0.0f, 1.0f, 0.0f); // Set the location to the point where the ray hits the surface
                if ((desertNPCsPrefab.Count > 0) && biomeTags.Contains(hit.collider.gameObject.tag))
                {
                    int entityIdx = UnityEngine.Random.Range(0, desertNPCsPrefab.Count);
                    Instantiate(desertNPCsPrefab[entityIdx], spawnLoc, Quaternion.identity);
                } 
            }
        }
    }

    private void spawnRockyEntity(RandomLocationGenerator rlg, float raycastOffset, string[] biomeTags)
    {
        int entities = UnityEngine.Random.Range(entitiesPerTileMin, entitiesPerTileMax);
        for (int i = 0; i < entities; i++)
        {
            Vector3 spawnLoc = rlg.getRandomLocation();
            Vector3 raycastStart = new Vector3(spawnLoc.x, this.gameObject.transform.position.y + raycastOffset, spawnLoc.z);
            RaycastHit hit;
            if (Physics.Raycast(raycastStart, Vector3.down, out hit))
            {
                spawnLoc = hit.point + new Vector3(0.0f, 1.0f, 0.0f); // Set the location to the point where the ray hits the surface
                if ((rockNPCsPrefab.Count > 0) && biomeTags.Contains(hit.collider.gameObject.tag))
                {
                    int entityIdx = UnityEngine.Random.Range(0, rockNPCsPrefab.Count);
                    Instantiate(rockNPCsPrefab[entityIdx], spawnLoc, Quaternion.identity);
                } 
            }
        }
    }

    private void spawnSnowEntity(RandomLocationGenerator rlg, float raycastOffset, string[] biomeTags)
    {
        int entities = UnityEngine.Random.Range(entitiesPerTileMin, entitiesPerTileMax);
        for (int i = 0; i < entities; i++)
        {
            Vector3 spawnLoc = rlg.getRandomLocation();
            Vector3 raycastStart = new Vector3(spawnLoc.x, this.gameObject.transform.position.y + raycastOffset, spawnLoc.z);
            RaycastHit hit;
            if (Physics.Raycast(raycastStart, Vector3.down, out hit))
            {
                spawnLoc = hit.point + new Vector3(0.0f, 1.0f, 0.0f); // Set the location to the point where the ray hits the surface
                if ((snowNPCsPrefab.Count > 0) && biomeTags.Contains(hit.collider.gameObject.tag))
                {
                    int entityIdx = UnityEngine.Random.Range(0, snowNPCsPrefab.Count);
                    Instantiate(snowNPCsPrefab[entityIdx], spawnLoc, Quaternion.identity);
                } 
            }
        }
    }

    //private void BatchSpawnTileObjects(GameObject tileGameObject, BiomeType biomeType, int itemCount, string prefabPath)
    //{
    //    RandomLocationGenerator rlg = new RandomLocationGenerator(tileGameObject.transform.position, (int)tileGameObject.transform.localScale.x, (int)tileGameObject.transform.localScale.z);

    //    Vector3 castDirection = Vector3.down;
    //    float raycastOffset = 50f;
    //    string[] biomeTags = { "Desert", "Forest", "Snowy", "Rocky" };

    //    GameObject[] prefabs = new GameObject[itemCount];
    //    for (int i = 1; i <= itemCount; i++)
    //    {
    //        prefabs[i - 1] = Resources.Load(prefabPath) as GameObject;
    //    }

    //    Vector3[] positions = new Vector3[itemCount];
    //    for (int i = 0; i < itemCount; i++)
    //    {
    //        Vector3 loc = rlg.getRandomLocation() * tileScale;
    //        Vector3 raycastStart = new Vector3(loc.x, tileGameObject.transform.position.y + raycastOffset, loc.z);

    //        RaycastHit hit;
    //        if (Physics.Raycast(raycastStart, castDirection, out hit))
    //        {
    //            loc = hit.point;
    //            if (biomeTags.Contains(hit.collider.gameObject.tag))
    //            {
    //                positions[i] = loc;
    //            }
    //        }
    //    }

    //    // Instantiate prefabs at the generated positions
    //    for (int i = 0; i < itemCount; i++)
    //    {
    //        if (positions[i] != Vector3.zero)
    //        {
    //            int randomPrefabIndex = UnityEngine.Random.Range(0, prefabs.Length);
    //            Instantiate(prefabs[randomPrefabIndex], positions[i], Quaternion.identity);
    //        }
    //    }
    //}

    //void GenerateNavMesh()
    //{
    //    NavMeshSurface navMeshSurface = this.gameObject.AddComponent<NavMeshSurface>();
    //    navMeshSurface.collectObjects = CollectObjects.Children;

    //    // Attach the NavMeshSurface to the GameObject and bake the NavMesh
    //    navMeshSurface.BuildNavMesh();
    //}
}