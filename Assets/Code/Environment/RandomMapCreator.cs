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
