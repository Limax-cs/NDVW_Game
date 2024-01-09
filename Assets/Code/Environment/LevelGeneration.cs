using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Unity.AI.Navigation;
using System.Diagnostics;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField]
    private int mapWidthInTiles, mapDepthInTiles;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    public int tileScale = 1;

    void Start()
    {
        GenerateMap();
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
}