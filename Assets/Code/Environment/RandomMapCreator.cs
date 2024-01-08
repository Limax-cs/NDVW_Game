using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Diagnostics;
using System.Collections.Specialized;

public class RandomMapCreator : MonoBehaviour
{

    public int sideLength = 120;
    public int biomeLength = 40;
    private float[,] noiseMap;
    private Dictionary<BiomeType, float> biomeHeightMultipliers;
    public List<GameObject> tiles;

    // private RandomLocationGenerator rgl = null;

    // Start is called before the first frame update
    void Start()
    {
        //planes = new List<GameObject>();
        //StartCoroutine(SetRandomTextureOnTerrain());
        noiseMap = GenerateNoiseMap(sideLength);
        UnityEngine.Debug.Log(noiseMap.Length);
        //UnityEngine.Debug.Log(noiseMap[0].Length);
        biomeHeightMultipliers = GetBiomeHeightMultipliers();
        floorGeneratorWithBiomes(noiseMap, biomeHeightMultipliers);
        CombineTiles();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void CombineTiles()
    {
        MeshFilter[] meshFilters = new MeshFilter[tiles.Count];
        CombineInstance[] combine = new CombineInstance[tiles.Count];

        for (int i = 0; i < tiles.Count; i++)
        {
            meshFilters[i] = tiles[i].GetComponent<MeshFilter>();
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            Destroy(tiles[i]); // Optionally destroy individual tiles after combining
        }

        // Create a new GameObject to hold the combined mesh (your entire terrain)
        GameObject combinedTerrain = new GameObject("CombinedTerrain");
        combinedTerrain.AddComponent<MeshFilter>();
        combinedTerrain.AddComponent<MeshRenderer>();

        // Assign the combined mesh to the new GameObject
        combinedTerrain.GetComponent<MeshFilter>().mesh = new Mesh();
        combinedTerrain.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedTerrain.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

        // Optionally, add a MeshCollider if needed for collision detection
        combinedTerrain.AddComponent<MeshCollider>().sharedMesh = combinedTerrain.GetComponent<MeshFilter>().mesh;

        // Set position, rotation, and scale of the combined terrain as needed
        combinedTerrain.transform.position = Vector3.zero;
        combinedTerrain.transform.rotation = Quaternion.identity;
        combinedTerrain.transform.localScale = Vector3.one;
    }

    private Dictionary<BiomeType, float> GetBiomeHeightMultipliers()
    {
        Dictionary<BiomeType, float> multipliers = new Dictionary<BiomeType, float>();
        multipliers[BiomeType.Forest] = 4.5f;
        multipliers[BiomeType.Desert] = 0.8f;
        multipliers[BiomeType.Rocky] = 17.5f;
        multipliers[BiomeType.Snowy] = 3.5f;

        return multipliers;
    }

    private void tileGenerator(Vector3 location, TileType tileType, BiomeType biomeType, float globalHeight, float biomeMultiplier)
    {

        RandomLocationGenerator rlg = new RandomLocationGenerator(location);

        // Create the tileff
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        plane.transform.position = location;

        Texture2D myTexture;

        float heightMultiplier = 1.0f;

        // Assign textures based on biomeType
        switch (biomeType)
        {
            case BiomeType.Forest:
                heightMultiplier = 2.5f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Grass_Moss_BaseColor") as Texture2D;
                break;
            case BiomeType.Desert:
                heightMultiplier = 1.0f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Sand_BaseColor") as Texture2D;
                break;
            case BiomeType.Rocky:
                heightMultiplier = 3.0f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Rock_BaseColor") as Texture2D;
                break;
            case BiomeType.Snowy:
                heightMultiplier = 2.5f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Snow_BaseColor") as Texture2D;
                break;
            default:
                heightMultiplier = 1.0f;
                myTexture = Resources.Load("Textures/terrainFloor") as Texture2D;
                break;
        }

        plane.GetComponent<Renderer>().material.mainTexture = myTexture;


        // Get the planes vertices
        Mesh mesh = plane.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        //int sideLength = (int)Mathf.Sqrt(vertices.Length);
        float planeSize = 10f;

        for (int v = 0; v < vertices.Length; v++)
        {
            // generate the height for the current vertex
            Vector3 vertexPosition = plane.transform.position + vertices[v] * planeSize / 10f;
            //float height = SimplexNoise.Noise(vertexPosition.x, vertexPosition.z);
            // Get the position of the plane relative to the noise map
            int noiseMapX = Mathf.FloorToInt((location.x + sideLength / 2f) / 10f);
            int noiseMapZ = Mathf.FloorToInt((location.z + sideLength / 2f) / 10f);

            // Ensure the noise map indices are within bounds
            noiseMapX = Mathf.Clamp(noiseMapX, 0, sideLength - 1);
            noiseMapZ = Mathf.Clamp(noiseMapZ, 0, sideLength - 1);

            // Get the height from the noise map using the calculated indices
            float height = noiseMap[noiseMapZ, noiseMapX]; // Swapped Z and X here as noiseMap is [Z,X]

            vertices[v].y = height * heightMultiplier;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //for (int i = 0; i < sideLength; i++)
        //{
        //    for (int j = 0; j < sideLength; j++)
        //    {
        //        Vector3 vertexPosition = plane.transform.TransformPoint(vertices[i * sideLength + j]) * planeSize / 10f;
        //        float height = noiseMap[(int)(vertexPosition.z + sideLength / 2), (int)(vertexPosition.x + sideLength / 2)];
        //        height = globalHeight * biomeMultiplier;

        //        Vector3 vertex = vertices[i * sideLength + j];
        //        vertices[i * sideLength + j] = new Vector3(vertex.x, height, vertex.z);
        //    }
        //}

        //SmoothHeightMap(vertices, sideLength, sideLength, 2); // Adjust the number of iterations as needed

        //mesh.vertices = vertices;
        //mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
        //UpdateMeshVerticesHeights(heightMap, mesh, vertices, heightMultiplier);
        
        //UpdateMeshVerticesHeights(heightMap, mesh, vertices, heightMultiplier);

        plane.AddComponent<MeshCollider>();
        tiles.Add(plane);

        // Create the objects in the tile based on type parameter
        if (tileType == TileType.Empty)
        {
            // Empty tile, just texture added.
        }
        else if (tileType == TileType.Tree)
        {
            for (int item_index = 1; item_index <= 2; item_index++)
            {
                GameObject tmp_go = Resources.Load("Handpainted_Forest_pack/Models/Fir_v1_" + item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);
            }
        }
        else if (tileType == TileType.Pine)
        {
            for (int item_index = 1; item_index <= 2; item_index++)
            {
                GameObject tmp_go = Resources.Load("Handpainted_Forest_pack/Models/Pine_v1_" + item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);
            }
        }
        else if (tileType == TileType.Grass)
        {
            for (int item_index = 1; item_index <= 3; item_index++)
            {
                GameObject tmp_go = Resources.Load("Handpainted_Forest_pack/Models/Grass_v1_" + item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);
            }
        }
        else if (tileType == TileType.Rocks)
        {
            for (int item_index = 1; item_index <= 4; item_index++)
            {
                GameObject tmp_go = Resources.Load("Free_Rocks/_prefabs/rock" + item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);
            }
        }
    }

    private void UpdateMeshVerticesHeights(float[,] heightMap, Mesh mesh, Vector3[] meshVertices, float heightMultiplier)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];

                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coord, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, height * heightMultiplier, vertex.z);

                vertexIndex++;
            }
        }

        //update the vertices in the mesh and update its properties
        mesh.vertices = meshVertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private float[,] GenerateNoiseMap(int sideLength)
    {
        float[,] noiseMap = new float[sideLength, sideLength];
        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                noiseMap[i, j] = SimplexNoise.Noise(i * 0.1f, j * 0.1f);
            }
        }

        return noiseMap;
    }

    public float[,] GenerateNoiseMap2(int mapDepth, int mapWidth, float scale)
    {
        float[,] noiseMap = new float[mapDepth, mapWidth];
        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = xIndex / scale;
                float sampleZ = zIndex / scale;
                // generate noise value using PerlinNoise
                float noise = Mathf.PerlinNoise(sampleX, sampleZ);
                noiseMap[zIndex, xIndex] = noise;
            }
        }
        return noiseMap;
    }

    private BiomeType[,] GenerateRandomBiomeMap(int sideLength, int biomeLength)
    {
        BiomeType[,] biomeMap = new BiomeType[sideLength, sideLength];
        System.Random random = new System.Random();

        for (int z = 0; z < sideLength; z += biomeLength)
        {
            for (int x = 0; x < sideLength; x += biomeLength)
            {
                BiomeType randomBiome = (BiomeType)random.Next(Enum.GetNames(typeof(BiomeType)).Length);
                for (int i = z; i < z + biomeLength && i < sideLength; i++)
                {
                    for (int j = x; j < x + biomeLength && j < sideLength; j++)
                    {
                        biomeMap[i, j] = randomBiome;
                    }
                }
            }
        }

        return biomeMap;
    }

    private void floorGeneratorWithBiomes(float[,] noiseMap, Dictionary<BiomeType, float> biomeMultipliers)
    {
        System.Random random = new System.Random();

        int halfPositiveSideLenght = (int)(this.sideLength / 2);
        int halfNegativeSideLenght = (int)(this.sideLength / -2);

        BiomeType[,] biomeMap = GenerateRandomBiomeMap(this.sideLength, biomeLength);

        for (int z_i = 0; z_i < sideLength; z_i++)
        {
            for (int x_i = 0; x_i < sideLength; x_i++)
            {
                BiomeType currentBiome = biomeMap[z_i, x_i];
                tileGenerator(
                    new Vector3(10f * (x_i - sideLength / 2), 0, 10f * (z_i - sideLength / 2)),
                    (TileType)random.Next(Enum.GetNames(typeof(TileType)).Length),
                    currentBiome,
                    noiseMap[z_i, x_i],
                    biomeMultipliers[currentBiome]);
            }
        }
    }

    //private float BilinearInterpolation(float[,] heightMap, float x, float z, int sideLength)
    //{
    //    int x0 = Mathf.Clamp((int)x, 0, sideLength - 1);
    //    int x1 = Mathf.Clamp(x0 + 1, 0, sideLength - 1);
    //    int z0 = Mathf.Clamp((int)z, 0, sideLength - 1);
    //    int z1 = Mathf.Clamp(z0 + 1, 0, sideLength - 1);

    //    float sx = x - x0;
    //    float sz = z - z0;

    //    float h00 = heightMap[x0, z0];
    //    float h01 = heightMap[x0, z1];
    //    float h10 = heightMap[x1, z0];
    //    float h11 = heightMap[x1, z1];

    //    float height = Mathf.Lerp(Mathf.Lerp(h00, h10, sx), Mathf.Lerp(h01, h11, sx), sz);
    //    return height;
    //}

    //private float CalculateHeight(float[,] heightMap, int x, int z, int sideLength, float heightScale)
    //{
    //    float height = BilinearInterpolation(heightMap, x, z, sideLength);
    //    return height * heightScale;
    //}

    private void SmoothHeightMap(Vector3[] vertices, int width, int height, int iterations)
    {
        for (int iter = 0; iter < iterations; iter++)
        {
            Vector3[] smoothedVertices = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                int adjacentVertices = 0;
                Vector3 averageVertex = Vector3.zero;

                int x = i % width;
                int y = i / width;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                        {
                            adjacentVertices++;
                            averageVertex += vertices[ny * width + nx];
                        }
                    }
                }

                smoothedVertices[i] = averageVertex / adjacentVertices;
            }

            // Update the original vertices with the smoothed values
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = smoothedVertices[i];
            }
        }
    }
}