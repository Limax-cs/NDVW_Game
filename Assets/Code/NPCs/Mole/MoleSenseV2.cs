using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleSenseV2 : MonoBehaviour
{
    // Hyperparameters
    public WorldStates beliefs = new WorldStates();
    
    
    void Start()
    {
        
    }

    
    void Update()
    {
    }

    // Detect objects
    public void OnTriggerEnter(Collider collider)
    {
        // Detect spaceship items
        if (collider.CompareTag("spaceship2_item"))
        {
            ObjectItem objectItem = collider.GetComponent<ObjectItem>();
            this.beliefs.ModifyState("Detect Mole SSItem " + objectItem.ID, 1);
        }

        if (collider.CompareTag("spaceship1_item"))
        {
            ObjectItem objectItem = collider.GetComponent<ObjectItem>();
            this.beliefs.ModifyState("Detect Babo SSItem " + objectItem.ID, 1);
        }
        
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    MODIFY EXTERNAL BELIEF PARAM
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void UpdateBeliefs(WorldStates externalBeliefs)
    {
        foreach(KeyValuePair<string, float> b in this.beliefs.GetStates())
        {
            if (b.Value >= 0)
            {
                externalBeliefs.SetState(b.Key, b.Value);
            }
            else
            {
                externalBeliefs.RemoveState(b.Key);
            }
        }
    }
}