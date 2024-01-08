using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

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

    //[SerializeField]
    //private TerrainType[] terrainTypes;

    [SerializeField]
    private float heightMultiplier;

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
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        // calculate the offsets based on the tile position
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        // generate a heightMap using noise
        float[,] heightMap = this.noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, this.mapScale, offsetX, offsetZ, waves);

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
    }

    //private Texture2D BuildTexture(float[,] heightMap)
    //{
    //    int tileDepth = heightMap.GetLength(0);
    //    int tileWidth = heightMap.GetLength(1);

    //    Color[] colorMap = new Color[tileDepth * tileWidth];
    //    for (int zIndex = 0; zIndex < tileDepth; zIndex++)
    //    {
    //        for (int xIndex = 0; xIndex < tileWidth; xIndex++)
    //        {
    //            // transform the 2D map index is an Array index
    //            int colorIndex = zIndex * tileWidth + xIndex;
    //            float height = heightMap[zIndex, xIndex];
    //            // assign as color a shade of grey proportional to the height value
    //            //colorMap[colorIndex] = Color.Lerp(Color.black, Color.white, height);
    //            TerrainType terrainType = ChooseTerrainType(height);
    //            colorMap[colorIndex] = terrainType.color;
    //        }
    //    }
    //    Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
    //    tileTexture.wrapMode = TextureWrapMode.Clamp;
    //    tileTexture.SetPixels(colorMap);
    //    tileTexture.Apply();
    //    return tileTexture;
    //}

    //TerrainType ChooseTerrainType(float height)
    //{
    //    // for each terrain type, check if the height is lower than the one for the terrain type
    //    foreach (TerrainType terrainType in terrainTypes)
    //    {
    //        // return the first terrain type whose height is higher than the generated one
    //        if (height < terrainType.height)
    //        {
    //            return terrainType;
    //        }
    //    }
    //    return terrainTypes[terrainTypes.Length - 1];
    //}

    private void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = this.meshFilter.mesh.vertices;

        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for(int xIndex = 0; xIndex < tileWidth; xIndex++)
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
}