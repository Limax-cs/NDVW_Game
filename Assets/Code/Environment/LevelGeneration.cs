//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LevelGeneration : MonoBehaviour
//{
//    [SerializeField]
//    private int mapWidthInTiles, mapDepthInTiles, biomeWidth;

//    [SerializeField]
//    private GameObject tilePrefab;

//    void Start()
//    {
//        GenerateMap();
//    }

//    void GenerateMap()
//    {
//        // get the tile dimensions from the tile Prefab
//        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
//        int tileWidth = (int)tileSize.x;
//        int tileDepth = (int)tileSize.z;

//        // for each Tile, instantiate a Tile in the correct position
//        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++)
//        {
//            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
//            {
//                Vector3 originalTileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;

//                // calculate the tile position based on the X and Z indices
//                Vector3 tilePosition = new Vector3(this.gameObject.transform.position.x + xTileIndex * tileWidth,
//                  this.gameObject.transform.position.y,
//                  this.gameObject.transform.position.z + zTileIndex * tileDepth);
//                // instantiate a new Tile
//                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
//            }
//        }
//    }

//    //void GenerateBiome(Material biomeMaterial, int startX, int startZ)
//    //{
//    //    for (int xTileIndex = startX; xTileIndex < startX + biomeWidth; xTileIndex++)
//    //    {
//    //        for (int zTileIndex = startZ; zTileIndex < startZ + biomeWidth; zTileIndex++)
//    //        {
//    //            // Instantiate tiles for this specific biome region
//    //            Vector3 tilePosition = new Vector3(xTileIndex, 0, zTileIndex);
//    //            GameObject tile = Instantiate(levelPrefab, tilePosition, Quaternion.identity, transform);

//    //            // Assign the biome material to the tile's renderer
//    //            Renderer tileRenderer = tile.GetComponent<Renderer>();
//    //            if (tileRenderer != null)
//    //            {
//    //                tileRenderer.material = biomeMaterial;
//    //            }
//    //            // Adjust other properties as needed
//    //        }
//    //    }
//    //}
//}
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class LevelGeneration : MonoBehaviour
//{
//    [SerializeField]
//    private int mapWidthInBiomes, mapDepthInBiomes, biomeWidth; // User input for the width of each biome

//    [SerializeField]
//    private GameObject levelPrefab; // The base level prefab to use for all biomes

//    [SerializeField]
//    private Texture2D defaultTexture; // Default texture for biome

//    [SerializeField]
//    private Texture2D forestTexture; // Texture for forest biome

//    [SerializeField]
//    private Texture2D desertTexture; // Texture for desert biome

//    [SerializeField]
//    private Texture2D rockyTexture; // Texture for rocky biome

//    [SerializeField]
//    private Texture2D snowyTexture; // Texture for snowy biome

//    // Other variables and functions...

//    private Dictionary<BiomeType, Texture2D> biomeTextures;

//    void Start()
//    {
//        // Populate the dictionary with biome textures
//        PopulateBiomeTextures();
//        GenerateMap();
//    }

//    private void PopulateBiomeTextures()
//    {
//        biomeTextures = new Dictionary<BiomeType, Texture2D>
//        {
//            { BiomeType.Forest, forestTexture },
//            { BiomeType.Desert, desertTexture },
//            { BiomeType.Rocky, rockyTexture },
//            { BiomeType.Snowy, snowyTexture }
//        };
//    }

//    void GenerateMap()
//    {
//        int mapWidthInTiles = mapWidthInBiomes * biomeWidth;
//        int mapDepthInTiles = mapDepthInBiomes * biomeWidth;

//        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex += biomeWidth)
//        {
//            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex += biomeWidth)
//            {
//                // Get a random biome type
//                BiomeType selectedBiome = GetRandomBiome();

//                // Generate a biome grid of tiles
//                GenerateBiome(selectedBiome, xTileIndex, zTileIndex);
//            }
//        }
//    }

//    private BiomeType GetRandomBiome()
//    {
//        // Randomly select a biome type
//        System.Random rand = new System.Random();
//        BiomeType currBiome = (BiomeType)rand.Next(0, System.Enum.GetValues(typeof(BiomeType)).Length);
//        return currBiome;
//    }

//    void GenerateBiome(BiomeType biomeType, int startX, int startZ)
//    {
//        Texture2D biomeTexture = biomeTextures[biomeType]; // Get texture based on biome type

//        for (int xTileIndex = startX; xTileIndex < startX + biomeWidth; xTileIndex++)
//        {
//            for (int zTileIndex = startZ; zTileIndex < startZ + biomeWidth; zTileIndex++)
//            {
//                // Calculate position based on the tile's index and biome width
//                Vector3 tilePosition = new Vector3(xTileIndex * mapWidthInBiomes, 0, zTileIndex * mapDepthInBiomes);
//                GameObject tile = Instantiate(levelPrefab, tilePosition, Quaternion.identity, transform);

//                // Set the biome texture to the tile's renderer
//                Renderer tileRenderer = tile.GetComponent<Renderer>();
//                if (tileRenderer != null)
//                {
//                    tileRenderer.material.mainTexture = biomeTexture;
//                }
//                // Adjust other properties as needed
//            }
//        }
//    }
//}
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Unity.AI.Navigation;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField]
    private int mapWidthInTiles, mapDepthInTiles;

    [SerializeField]
    private GameObject tilePrefab;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // get the tile dimensions from the tile Prefab
        //tilePrefab.transform.localScale *= 10.0f;
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        UnityEngine.Debug.Log(tileSize);
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        // for each Tile, instantiate a Tile in the correct position
        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
            {
                Vector3 originalTileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;

                // calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new Vector3(this.gameObject.transform.position.x + xTileIndex * tileWidth,
                  this.gameObject.transform.position.y,
                  this.gameObject.transform.position.z + zTileIndex * tileDepth);
                // instantiate a new Tile
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                
            }
        }
        Destroy(tilePrefab);
        
        //GameObject firstTile = GameObject.Find("Level Tile(Clone)");
        //firstTile.GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}