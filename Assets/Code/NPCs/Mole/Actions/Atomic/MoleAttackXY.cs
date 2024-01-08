using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleAttackXY : MoleAction
{

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public GameObject target;
    public MoleAgent moleAgent;

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    UPDATE ACTION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void UpdateConditions(List<WorldState> preconditionsList, List<WorldState> effectsList)
    {
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

    
    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    CHECK ACTION STATES
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public override bool IsAchievable()
    {
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, float> conditions)
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
        moleAgent = GetComponent<MoleAgent>();

        // Check if the item is in backpack
        bool hasItem = false;

        if (target is not null)
        {
            for(int k=0; k < this.backpack.Count; k++){
                if(this.backpack[k] == target)
                {
                    this.indexItem = k;
                    hasItem = true;
                    moleAgent.UpdateIdex(k);
                    break;
                }
            }
        }
        
        if (hasItem)
        {
            Debug.Log("Use Item in index " + this.indexItem);
            // Use item
            if (this.backpack[this.indexItem] is not null)
            {
                if (this.backpack[this.indexItem].GetComponent<ObjectItem>())
                {
                    if(this.backpack[this.indexItem].tag == "spaceship1_item" || this.backpack[this.indexItem].tag == "spaceship2_item")
                    {
                        ObjectItem objectitem = this.backpack[this.indexItem].GetComponent<ObjectItem>();
                        objectitem.UseObject();
                        this.backpack[this.indexItem] = null;
                    }
                    
                }
                
            }
        }
        else
        {
            Debug.Log("Cannot use the item");
        }
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    GET UTILITY OF THE ACTION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public override float ComputeUtilityScore()
    {
        return 1;
    }

    
}
