using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Diagnostics;

public class RandomMapCreator : MonoBehaviour
{

    public int sideLength = 120;
    public int biomeLength = 40;

    // private RandomLocationGenerator rgl = null;

    // Start is called before the first frame update
    void Start()
    {
        //planes = new List<GameObject>();
        //StartCoroutine(SetRandomTextureOnTerrain());
        floorGeneratorWithBiomes();
    }

    // Update is called once per frame
    void Update()
    {
    }

    //IEnumerator SetRandomTextureOnTerrain() {

    //    string[] biome = { "Black_Sand", "Grass_A", "Grass_B", "Grass_Dry", "Grass_Moss", "Grass_Soil", "Heather", "Muddy", "Pebbles_A", "Pebbles_B", "Pebbles_C", "Rock", "Sand", "Snow", "Soil_Rocks", "Tidal_Pools"};
    //    Texture2D myTexture = Resources.Load ("TerrainSampleAssets/Textures/Terrain/"+biome[Random.Range(0, 16)]+"_BaseColor") as Texture2D;
    //    foreach (GameObject plane in planes){ plane.GetComponent<Renderer>().material.mainTexture = myTexture; }

    //    yield return new WaitForSeconds(5);
    //    StartCoroutine(SetRandomTextureOnTerrain());
    //}

    private void tileGenerator(Vector3 location, TileType tileType, BiomeType biomeType)
    {

        RandomLocationGenerator rlg = new RandomLocationGenerator(location);

        // Create the tile
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        plane.transform.position = location;

        Texture2D myTexture;

        float detailScale = 0.0f;
        float heightScale = 0.0f;

        // Assign textures based on biomeType
        switch (biomeType)
        {
            case BiomeType.Forest:
                detailScale = 1.0f;
                heightScale = 4.0f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Grass_Moss_BaseColor") as Texture2D;
                break;
            case BiomeType.Desert:
                detailScale = 2.0f;
                heightScale = 0.8f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Sand_BaseColor") as Texture2D;
                break;
            case BiomeType.Rocky:
                detailScale = 3.0f;
                heightScale = 1.5f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Rock_BaseColor") as Texture2D;
                break;
            case BiomeType.Snowy:
                detailScale = 1.0f;
                heightScale = 1.5f;
                myTexture = Resources.Load("TerrainSampleAssets/Textures/Terrain/Snow_BaseColor") as Texture2D;
                break;
            case BiomeType.Empty:
                detailScale = 1.0f;
                heightScale = 1.0f;
                myTexture = Resources.Load("Textures/terrainFloor") as Texture2D;
                break;
            default:
                detailScale = 1.0f;
                heightScale = 1.0f;
                myTexture = Resources.Load("Textures/terrainFloor") as Texture2D;
                break;
        }

        plane.GetComponent<Renderer>().material.mainTexture = myTexture;


        // Get the planes vertices
        //float detailScale = 3.0f;
        //float heightScale = 0.5f;

        Mesh mesh = plane.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int sideLength = (int)Mathf.Sqrt(vertices.Length);
        float planeSize = 10f;

        float[,] heightMap = new float[sideLength, sideLength];

        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                Vector3 vertexPosition = plane.transform.TransformPoint(vertices[i * sideLength + j]) * planeSize / 10f;
                heightMap[i, j] = SimplexNoise.Noise(vertexPosition.x, vertexPosition.z);
            }
        }

        SmoothHeightMap(heightMap, 5); // Adjust the number of iterations as needed

        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                vertices[i * sideLength + j].y = CalculateHeight(heightMap, i, j, sideLength, heightScale);
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        plane.AddComponent<MeshCollider>();

        // Create the objects in the tile based on type parameter
        if (tileType == TileType.Empty)
        { // Empty tile, just texture added.

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
        else if (tileType == TileType.Crystal)
        {
            GameObject crystalPrefab = null;
            switch (biomeType)
            {
                case BiomeType.Forest:
                    crystalPrefab = Resources.Load("Toon Crystals pack/Prefabs/BlueCrystal00") as GameObject; 
                    break;
                case BiomeType.Desert:
                    crystalPrefab = Resources.Load("Toon Crystals pack/Prefabs/RedCrystal08") as GameObject;
                    break;
                case BiomeType.Rocky:
                    crystalPrefab = Resources.Load("Toon Crystals pack/Prefabs/GemStone00") as GameObject; 
                    break;
                case BiomeType.Snowy:
                    crystalPrefab = Resources.Load("Toon Crystals pack/Prefabs/PurpCrystal00") as GameObject;
                    break;
            }

            if (crystalPrefab != null)
            {
                int numberOfCrystals = 4; 
                for (int i = 0; i < numberOfCrystals; i++)
                {
                    Instantiate(crystalPrefab, rlg.getRandomLocation(), Quaternion.identity);
                }
            }
        }


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
    private void floorGeneratorWithBiomes()
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
                    new Vector3(10f * (x_i - sideLength / 2), 0,
                    10f * (z_i - sideLength / 2)),
                    (TileType)random.Next(Enum.GetNames(typeof(TileType)).Length),
                    currentBiome);
            }
        }
    }

    private float BilinearInterpolation(float[,] heightMap, float x, float z, int sideLength)
    {
        int x0 = Mathf.Clamp((int)x, 0, sideLength - 1);
        int x1 = Mathf.Clamp(x0 + 1, 0, sideLength - 1);
        int z0 = Mathf.Clamp((int)z, 0, sideLength - 1);
        int z1 = Mathf.Clamp(z0 + 1, 0, sideLength - 1);

        float sx = x - x0;
        float sz = z - z0;

        float h00 = heightMap[x0, z0];
        float h01 = heightMap[x0, z1];
        float h10 = heightMap[x1, z0];
        float h11 = heightMap[x1, z1];

        float height = Mathf.Lerp(Mathf.Lerp(h00, h10, sx), Mathf.Lerp(h01, h11, sx), sz);
        return height;
    }

    private float CalculateHeight(float[,] heightMap, int x, int z, int sideLength, float heightScale)
    {
        float height = BilinearInterpolation(heightMap, x, z, sideLength);
        return height * heightScale;
    }

    private void SmoothHeightMap(float[,] heightMap, int iterations)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        for (int iter = 0; iter < iterations; iter++)
        {
            float[,] smoothedMap = new float[width, height];

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    float avg = (
                        heightMap[x - 1, y - 1] + heightMap[x - 1, y] + heightMap[x - 1, y + 1] +
                        heightMap[x, y - 1] + heightMap[x, y] + heightMap[x, y + 1] +
                        heightMap[x + 1, y - 1] + heightMap[x + 1, y] + heightMap[x + 1, y + 1]
                    ) / 9.0f;

                    smoothedMap[x, y] = avg;
                }
            }

            // Update the original height map with the smoothed values
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heightMap[x, y] = smoothedMap[x, y];
                }
            }
        }
    }
}