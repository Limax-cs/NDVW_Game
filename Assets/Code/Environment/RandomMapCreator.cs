using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RandomMapCreator : MonoBehaviour{
    //
    public int sideLength = 40;
    public int biomeLength = 10;

    //
    // private RandomLocationGenerator rgl = null;

    // Start is called before the first frame update
    void Start()
    {   
        floorGeneratorWithBiomes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void tileGenerator(Vector3 location, TileType tileType, BiomeType biomeType){

        RandomLocationGenerator rlg = new RandomLocationGenerator(location);

        // Create the tile
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        plane.transform.position = location;

        // Texture2D myTexture = Resources.Load ("Almgp_grassyTerrain/Models/MESHes/Materials/diffuse_x0_y0") as Texture2D;
        //Texture2D myTexture = Resources.Load ("Textures/terrainFloor") as Texture2D;
        Texture2D myTexture;

        // Assign textures based on biomeType
        switch (biomeType)
        {
            case BiomeType.Forest:
                myTexture = Resources.Load("Textures/terrainFloor") as Texture2D;
                break;
            case BiomeType.Desert:
                myTexture = Resources.Load("Texture/terrainFloor") as Texture2D;
                break;
            case BiomeType.Rocky:
                myTexture = Resources.Load("Texture/terrainFloor") as Texture2D;
                break;
            case BiomeType.Empty:
                myTexture = Resources.Load("Textures/terrainFloor") as Texture2D;
                break;
            default:
                myTexture = Resources.Load("Textures/terrainFloor") as Texture2D;
                break;
        }

        plane.GetComponent<Renderer>().material.mainTexture = myTexture;


        // Get the planes vertices
        float detailScale = 1;
        float heightScale = 0.5f;
        float buffer = 1; // (buffer * 2 + 1)^2 will be total number of planes in the scene at any one time
        float planeSize = 10f / buffer;

        Mesh mesh = plane.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        // alter vertex Y position depending on simplex noise)
        for (int v = 0; v < vertices.Length; v++)
        {
            // generate the height for current vertex
            Vector3 vertexPosition = plane.transform.position + vertices[v] * planeSize / 10f;
            float height = SimplexNoise.Noise(vertexPosition.x * detailScale, vertexPosition.z * detailScale);
            // scale it with the heightScale field
            vertices[v].y = height * heightScale;
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
        { // Tree tile
            for (int item_index = 1; item_index <= 2; item_index++)
            {
                GameObject tmp_go = Resources.Load("Handpainted_Forest_pack/Models/Fir_v1_" + item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);
            }
        }
        else if (tileType == TileType.Pine)
        { // Pine tile
            for (int item_index = 1; item_index <= 2; item_index++)
            {
                GameObject tmp_go = Resources.Load("Handpainted_Forest_pack/Models/Pine_v1_" + item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);
            }
        }
        else if (tileType == TileType.Grass)
        { // Grass tile
            for (int item_index = 1; item_index <= 3; item_index++)
            {
                GameObject tmp_go = Resources.Load("Handpainted_Forest_pack/Models/Grass_v1_" + item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);
            }
        }
        else if (tileType == TileType.Rocks)
        { // Rocks tile
            for (int item_index = 1; item_index <= 4; item_index++)
            {
                GameObject tmp_go = Resources.Load("Free_Rocks/_prefabs/rock" + item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);
            }
        }


        // if(Random.Range(0, 9) % 2 == 0){
        //     string[] biome = { "Cliff", "Rock", "Plate"};
        //     string[] rock_type = { "basic", "blank", "desert", "fancy", "forest", "ice", "infernal", "snow", "white"};
        //     GameObject tmp_go = Resources.Load ("Stylized Rock Pack - 8 styles, 3 shapes, LODs, PBR/Prefabs/"+biome[Random.Range(0, 3)]+"_"+rock_type[Random.Range(0, 9)]) as GameObject;
        //     Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);
        // }
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
    private void floorGeneratorWithBiomes(){
        System.Random random = new System.Random();

        int halfPositiveSideLenght = (int)(this.sideLength/2);
        int halfNegativeSideLenght = (int)(this.sideLength/-2);

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

    public enum BiomeType
    {
        Empty,
        Forest,
        Desert,
        Rocky,
    }

    public enum TileType
    {
        Empty,
        Tree,
        Pine,
        Grass,
        Rocks
    }
}
