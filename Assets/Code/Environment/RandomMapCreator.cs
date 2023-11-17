using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapCreator : MonoBehaviour{
    //
    public int sideLength = 3;

    //
    // private RandomLocationGenerator rgl = null;

    // Start is called before the first frame update
    void Start()
    {   
        floorGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void tileGenerator(Vector3 location, int type){

        RandomLocationGenerator rlg = new RandomLocationGenerator(location);

        // Create the tile
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        plane.transform.position = location;

        Texture2D myTexture = Resources.Load ("Almgp_grassyTerrain/Models/MESHes/Materials/diffuse_x0_y0") as Texture2D;
        plane.GetComponent<Renderer>().material.mainTexture = myTexture;


        // Get the planes vertices
            float detailScale = 1;
            float heightScale = 0.5f;
            float buffer = 1; // (buffer * 2 + 1)^2 will be total number of planes in the scene at any one time
            float planeSize  = 10f / buffer;

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
        if(type == 0){ // Empty tile, just texture added.

        } 
        else if(type == 1){ // Tree tile
            for (int item_index = 1; item_index <= 2; item_index++){
                GameObject tmp_go = Resources.Load ("Handpainted_Forest_pack/Models/Fir_v1_"+item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);   
            }
        }
        else if(type == 2){ // Pine tile
            for (int item_index = 1; item_index <= 2; item_index++){
                GameObject tmp_go = Resources.Load ("Handpainted_Forest_pack/Models/Pine_v1_"+item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);   
            }
        }
        else if(type == 3){ // Grass tile
            for (int item_index = 1; item_index <= 3; item_index++){
                GameObject tmp_go = Resources.Load ("Handpainted_Forest_pack/Models/Grass_v1_"+item_index) as GameObject;
                Instantiate(tmp_go, rlg.getRandomLocation(), Quaternion.identity);   
            }
        }
        else if(type == 4){ // Rocks tile
            for (int item_index = 1; item_index <= 4; item_index++){
                GameObject tmp_go = Resources.Load ("Free_Rocks/_prefabs/rock"+item_index) as GameObject;
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

    private void floorGenerator(){
        int halfPositiveSideLenght = (int)(this.sideLength/2);
        int halfNegativeSideLenght = (int)(this.sideLength/-2);

        for(int z_i=halfNegativeSideLenght; z_i<=halfPositiveSideLenght; z_i++){
            for(int x_i=halfNegativeSideLenght; x_i<=halfPositiveSideLenght; x_i++){
                tileGenerator(new Vector3(10f*x_i, 0, 10f*z_i), Random.Range(0, 5));
            }
        }
    }
}
