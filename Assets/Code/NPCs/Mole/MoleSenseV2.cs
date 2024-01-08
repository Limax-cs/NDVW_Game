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

        // Detect other items
        if (collider.CompareTag("weapon"))
        {
            WeaponItem weaponItem = collider.GetComponent<WeaponItem>();
            this.beliefs.ModifyState("Detect Weapon " + weaponItem.weaponDescrib.ID, 1);
        }

        if (collider.CompareTag("consumable"))
        {
            EdibleItem edibleItem = collider.GetComponent<EdibleItem>();
            this.beliefs.ModifyState("Detect Edible " + edibleItem.edibleDescrib.ID, 1);
        }

        // Detect agents
        if (collider.CompareTag("Player"))
        {
            this.beliefs.ModifyState("Detect Player", 1);
            this.beliefs.ModifyState("Presence Player", 1);
        }

        if (collider.CompareTag("mole"))
        {
            MoleAgent moleAgent = collider.GetComponent<MoleAgent>();
            this.beliefs.ModifyState("Detect Mole " + moleAgent.moleParams.ID, 1);
            this.beliefs.ModifyState("Presence Mole " + moleAgent.moleParams.ID, 1);
        }
        
    }

    public void OnTriggerExit(Collider collider)
    {
        // Detect agents
        if (collider.CompareTag("Player"))
        {
            this.beliefs.RemoveState("Presence Player");
        }

        if (collider.CompareTag("mole"))
        {
            MoleAgent moleAgent = collider.GetComponent<MoleAgent>();
            this.beliefs.RemoveState("Presence Mole " + moleAgent.moleParams.ID);
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