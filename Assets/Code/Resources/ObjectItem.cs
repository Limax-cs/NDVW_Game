using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

public class ObjectItem : MonoBehaviour
{
    public Texture2D icon;
    public bool inCorrectSpaceship = false;
    public int ID = -1;
    public bool usedItem = false;
    public NavMeshObstacle itemObstacle;

    //public float disableObstCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.tag == "spaceship1_item" || this.gameObject.tag == "spaceship2_item")
            itemObstacle = GetComponent<NavMeshObstacle>();
        else
            itemObstacle = null;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (disableObstCount > 0)
        {
            disableObstCount -= Time.deltaTime;
        }
        else
        {
            if (itemObstacle is not null)
            {
                itemObstacle.carving = true;
                itemObstacle.enabled = true;
            }     
        }*/
    }

    // Put the item in the spaceship
    public void UseObject()
    {
        if (inCorrectSpaceship && (usedItem == false))
        {
            if (this.gameObject.tag == "spaceship1_item")
            {
                GWorld.Instance.GetWorld().ModifyState("Player_Goals_Achieved", (int) Math.Pow(2, ID));
                usedItem = true;
                Destroy(this.gameObject);
            }
            else if (this.gameObject.tag == "spaceship2_item")
            {
                GWorld.Instance.GetWorld().ModifyState("Rival_Goals_Achieved", (int) Math.Pow(2, ID));
                usedItem = true;
                Destroy(this.gameObject);
            }
        }
    }

    // Trigger
    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("Status O1");
        if ((this.gameObject.tag == "spaceship1_item" && collision.gameObject.tag == "spaceship1") || 
            (this.gameObject.tag == "spaceship2_item" && collision.gameObject.tag == "spaceship2"))
            {
                inCorrectSpaceship = true;
            }
        //Debug.Log("Status O2");
    }

    private void OnTriggerExit(Collider collision)
    {
        //Debug.Log("Status O3");
        if ((this.gameObject.tag == "spaceship1_item" && collision.gameObject.tag == "spaceship1") || 
            (this.gameObject.tag == "spaceship2_item" && collision.gameObject.tag == "spaceship2"))
            {
                inCorrectSpaceship = false;
            }
        //Debug.Log("Status O4");
    }

    // Disable/Enable obstacle
    /*
    public void DisableObstacle(float disableTime)
    {
        if (itemObstacle is not null)
        {
            Debug.Log("Disable Obstacle");
            disableObstCount = disableTime;
            itemObstacle.carving = false;
            itemObstacle.enabled = false;
        }
    }*/

    public void ToggleNavMeshObstacle(bool mode)
    {
        //Debug.Log("Status O5");
        if (itemObstacle is not null)
        {
            if (mode)
            {
                itemObstacle.carving = true;
                itemObstacle.enabled = true;
            }
            else
            {
                itemObstacle.carving = false;
                itemObstacle.enabled = false;
            }
        }
        //Debug.Log("Status O6");
    }
}
