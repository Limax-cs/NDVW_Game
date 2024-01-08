using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Unity.AI.Navigation;
using System.Linq;

//[System.Serializable]
//public class TerrainType
//{
//    public string name;
//    public float height;
//    public Color color;
//}

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

    //[SerializeField]
    //private AnimationCurve heightCurve;

    [SerializeField]
    private Wave[] waves;

    // Start is called before the first frame update
    void Start()
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
        UnityEngine.Debug.Log(tileDepth);
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
                //heightMultiplier = 2.5f;
                myTexture = Resources.Load("Textures/terrainFloor") as Texture2D;
                this.tag = "Forest";
                break;
            case BiomeType.Desert:
                //heightMultiplier = 1.0f;
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

        //Texture2D tileTexture = BuildTexture(heightMap);
        this.tileRenderer.material.mainTexture = myTexture;

        UpdateMeshVertices(heightMap);

        // Spawn items
        SpawnTileObjects(this.gameObject, currentBiome);

        // Generate NavMesh for the tile
        surface.BuildNavMesh();
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
        if (biomeType == BiomeType.Forest)
        {
            for(int i = 0; i < 40; i++)
            {
                for (int item_index = 1; item_index <= 2; item_index++)
                {
                    GameObject tmp_go = Resources.Load("Handpainted_Forest_pack/Models/Fir_v1_" + item_index) as GameObject;
                    Vector3 loc = rlg.getRandomLocation();

                    // Adjust the raycast starting position above the tile
                    Vector3 raycastStart = new Vector3(loc.x, tileGameObject.transform.position.y + raycastOffset, loc.z);

                    RaycastHit hit;
                    if (Physics.Raycast(raycastStart, castDirection, out hit))
                    {
                        loc = hit.point; // Set the location to the point where the ray hits the surface
                        if (biomeTags.Contains(hit.collider.gameObject.tag))
                        {
                            Instantiate(tmp_go, loc, Quaternion.identity);
                        }
                    }
                }
            }
            //BatchSpawnTileObjects(tileGameObject, biomeType, 40, "Handpainted_Forest_pack/Models/Fir_v1_");
        }
        else if (biomeType == BiomeType.Desert)
        {
            for (int i = 0; i < 40; i++)
            {
                for (int item_index = 1; item_index <= 4; item_index++)
                {
                    GameObject tmp_go = Resources.Load("Free_Rocks/_prefabs/rock" + item_index) as GameObject;
                    Vector3 loc = rlg.getRandomLocation();

                    // Adjust the raycast starting position above the tile
                    Vector3 raycastStart = new Vector3(loc.x, tileGameObject.transform.position.y + raycastOffset, loc.z);

                    RaycastHit hit;
                    if (Physics.Raycast(raycastStart, castDirection, out hit))
                    {
                        loc = hit.point; // Set the location to the point where the ray hits the surface
                        if (biomeTags.Contains(hit.collider.gameObject.tag))
                        {
                            Instantiate(tmp_go, loc, Quaternion.identity);
                        }
                    }
                }
            }
            
            //BatchSpawnTileObjects(tileGameObject, biomeType, 30, "Free_Rocks/_prefabs/rock");
        }
        else if (biomeType == BiomeType.Snowy)
        {
            for(int i = 0; i < 60; i++)
            {
                for (int item_index = 1; item_index <= 3; item_index++)
                {
                    GameObject tmp_go = Resources.Load("Handpainted_Forest_pack/Models/Grass_v1_" + item_index) as GameObject;
                    Vector3 loc = rlg.getRandomLocation();

                    // Adjust the raycast starting position above the tile
                    Vector3 raycastStart = new Vector3(loc.x, tileGameObject.transform.position.y + raycastOffset, loc.z);

                    RaycastHit hit;
                    if (Physics.Raycast(raycastStart, castDirection, out hit))
                    {
                        loc = hit.point; // Set the location to the point where the ray hits the surface
                        if (biomeTags.Contains(hit.collider.gameObject.tag))
                        {
                            Instantiate(tmp_go, loc, Quaternion.identity);
                        }
                    }
                }
            }
            
            //BatchSpawnTileObjects(tileGameObject, biomeType, 20, "Free_Rocks/_prefabs/rock");
        }
        else if (biomeType == BiomeType.Rocky)
        {
            for (int i = 0; i < 80; i++)
            {
                for (int item_index = 1; item_index <= 4; item_index++)
                {
                    GameObject tmp_go = Resources.Load("Free_Rocks/_prefabs/rock" + item_index) as GameObject;
                    Vector3 loc = rlg.getRandomLocation();

                    // Adjust the raycast starting position above the tile
                    Vector3 raycastStart = new Vector3(loc.x, tileGameObject.transform.position.y + raycastOffset, loc.z);

                    RaycastHit hit;
                    if (Physics.Raycast(raycastStart, castDirection, out hit))
                    {
                        loc = hit.point; // Set the location to the point where the ray hits the surface
                        if (biomeTags.Contains(hit.collider.gameObject.tag))
                        {
                            Instantiate(tmp_go, loc, Quaternion.identity);
                        }
                    }
                }
            }
            //BatchSpawnTileObjects(tileGameObject, biomeType, 40, "Free_Rocks/_prefabs/rock");
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