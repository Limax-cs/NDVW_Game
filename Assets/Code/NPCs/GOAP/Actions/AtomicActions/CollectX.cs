using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectX : GAction
{

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    public GameObject target;

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    UPDATE ACTION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void UpdateConditions(List<WorldState> preconditionsList, List<WorldState> effectsList)
    {
        if (target != null)
        {
            // Effects
            WorldState detectedItem = new WorldState();
            WorldState collectableItem = new WorldState();
            WorldState hasItem = new WorldState();
            WorldState posX = new WorldState("Agent X POS", target.transform.position[0]);
            WorldState posY = new WorldState("Agent Y POS", target.transform.position[1]);
            WorldState posZ = new WorldState("Agent Z POS", target.transform.position[2]);

            // Normal status
            if (target.tag == "spaceship1_item")
            {
                ObjectItem objectItem = target.GetComponent<ObjectItem>();

                detectedItem.key = "Detect Babo SSItem " + objectItem.ID;
                collectableItem.key = "Collectable Babo SSItem " + objectItem.ID;
                hasItem.key = "Has Babo SSItem " + objectItem.ID;
            }
            else if (target.tag == "spaceship2_item")
            {
                ObjectItem objectItem = target.GetComponent<ObjectItem>();
                
                detectedItem.key = "Detect Mole SSItem " + objectItem.ID;
                collectableItem.key = "Collectable Mole SSItem " + objectItem.ID;
                hasItem.key = "Has Mole SSItem " + objectItem.ID;
            }
            detectedItem.value = 1;
            collectableItem.value = 1;
            hasItem.value = 1;

            // Update lists
            preconditionsList.Add(detectedItem);
            preconditionsList.Add(collectableItem);
            preconditionsList.Add(posX);
            preconditionsList.Add(posY);
            preconditionsList.Add(posZ);
            effectsList.Add(hasItem);

            // Update status
            this.preConditions = preconditionsList.ToArray();
            this.afterEffects = effectsList.ToArray();

            this.preconditions.Clear();
            foreach(WorldState s in preconditionsList)
            {
                this.preconditions.Add(s.key, s.value);
            }


            this.effects.Clear();
            foreach(WorldState s in effectsList)
            {
                this.effects.Add(s.key, s.value);
            }
        }
    }

    public void UpdateAction()
    {
    
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    CHECK ACTION STATES
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public override bool IsAchievable()
    {
        bool targetNotNull = target != null;
        return targetNotNull;
    }

    public override bool IsAchievableGiven(Dictionary<string, float> conditions)
    {
        bool targetNotNull = target != null;
        if (targetNotNull)
        {
            Vector3 preconditionPos = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 conditionPos = new Vector3(0.0f, 0.0f, 0.0f);

            if (conditions.ContainsKey("Agent X POS"))
                conditionPos[0] = conditions["Agent X POS"];
            if (conditions.ContainsKey("Agent Y POS"))
                conditionPos[1] = conditions["Agent Y POS"];
            if (conditions.ContainsKey("Agent Z POS"))
                conditionPos[2] = conditions["Agent Z POS"];

            if (preconditions.ContainsKey("Agent X POS"))
                preconditionPos[0] = preconditions["Agent X POS"];
            if (preconditions.ContainsKey("Agent Y POS"))
                preconditionPos[1] = preconditions["Agent Y POS"];
            if (preconditions.ContainsKey("Agent Z POS"))
                preconditionPos[2] = preconditions["Agent Z POS"];

            foreach (KeyValuePair<string, float> p in preconditions)
            {
                if (!conditions.ContainsKey(p.Key))
                    return false;
            }

            //Debug.Log("Distance: " + Vector3.Distance(preconditionPos, conditionPos));
            if (Vector3.Distance(preconditionPos, conditionPos) > 10.0f)
                return false;
            return true;
        }
        return false;
    }

    public override bool IsFinished()
    {
        return true;
    }


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    PERFORM ACTION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }

    public override void Perform()
    {
        // Select backpack space
        bool freeSpace = false;
        for(int k=0; k < agentData.backpack.Count; k++){
            if(agentData.backpack[k] == null)
            {
                agentData.indexItem = k;
                freeSpace = true;
                break;
            }
        }

        // COllect item
        if (freeSpace)
        {
            //Debug.Log("Backpack space selected: " + agentData.indexItem);

            //Debug.Log("Collecting");

            // Emmision and backpack
            Rigidbody rb = agentData.hit.collider.GetComponent<Rigidbody>();
            agentData.hit.collider.GetComponent<Highlight>()?.ToggleHighLight(false);
            agentData.backpack[agentData.indexItem] = agentData.hit.collider.gameObject;

            // Type-specific
            if (agentData.hit.collider.GetComponent<EdibleItem>() || agentData.hit.collider.GetComponent<WeaponItem>())
            {
                //Debug.Log("Edible!!!");
                agentData.backpack[agentData.indexItem].transform.position = Vector3.zero;
                agentData.backpack[agentData.indexItem].transform.rotation = Quaternion.identity;
                agentData.backpack[agentData.indexItem].transform.SetParent(agentData.pickUpParent.transform, false);
            }
            if (agentData.hit.collider.GetComponent<ObjectItem>())
            {
                //Debug.Log("Object");
                ObjectItem objectItem = target.GetComponent<ObjectItem>();
                agentData.backpack[agentData.indexItem].transform.SetParent(agentData.pickUpParentStatic.transform, true);

                objectItem.ToggleNavMeshObstacle(false);
            }

            // Parent object
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        else
        {
            Debug.Log("No Backpack space");
        }
    }

    

}
