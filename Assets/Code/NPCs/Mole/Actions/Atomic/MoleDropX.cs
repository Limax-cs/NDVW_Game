using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleDropX : MoleAction
{

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    bool droppedItem = false;

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    INITIALIZATION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void Start()
    {
        base.Start();

        WorldState freeSpaceEffect = new WorldState();
        freeSpaceEffect.key = "FreeSpace";
        freeSpaceEffect.value = 1;
        afterEffects = new WorldState[]{freeSpaceEffect};
        effects.Clear();
        effects.Add("FreeSpace", 1);
    } 

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    CHECK ACTION STATES
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public override bool IsAchievable()
    {
        foreach(GameObject g in this.backpack)
        {
            if (g is not null)
                return true;
        }
        return false;
    }

    public override bool IsAchievableGiven(Dictionary<string, float> conditions)
    {
        // Check if there is an item to drop
        bool hasAnItemToDrop = false;
        foreach(GameObject g in this.backpack)
        {
            if (g is not null)
            {
                hasAnItemToDrop = true;
                break;
            }
        }
        
        // Check other preconditions
        if (hasAnItemToDrop)
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
            if (Vector3.Distance(preconditionPos, conditionPos) > 4.0f)
                return false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool IsFinished()
    {
        return droppedItem;
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
        if (!droppedItem)
        {
            // Select backpack Item
            int backpackIndex = 0;
            float value = 99999999;

            for(int k=0; k < this.backpack.Count; k++)
            {
                if (this.backpack[k] is not null)
                {
                    float itemValue = 1;
                    if (itemValue < value)
                    {
                        backpackIndex = k;
                    }
                }
            }

            // Drop Item
            this.backpack[backpackIndex].transform.SetParent(null);
            this.backpack[backpackIndex] = null;
            droppedItem = true;
        }
    }

    public override float ComputeUtilityScore()
    {
        return 1;
    }

}
