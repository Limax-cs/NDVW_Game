using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Recover : GAction
{
    
    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public bool canRecover = false;
    public bool perform = false;
    public GameObject[] spaceship_items;
    public GameObject target_item;


    // Possible subactions
    private GoToX goToGoalItem;
    private GoToX goToBase;
    private UseX applyItem;
    private CollectX collectItem;
    private DropAny dropAnyItem;
    public List<GAction> subactions;



    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    SELECT WHICH ITEM TO RECOVER (UTILITIES)
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void Start()
    {
        //Debug.Log("Status R1");
        base.Start();
        //Debug.Log("Status R2");

        // Configuration
        actionName = "Recover";
        spaceship_items = GameObject.FindGameObjectsWithTag("spaceship2_item");

        //Debug.Log("Status R3");

        // Initialize Recover Actions
        GameObject spaceship = GameObject.FindGameObjectWithTag("spaceship2");

        //Debug.Log("Status R4");

        // -- Go To Spaceship Item
        goToGoalItem = this.gameObject.AddComponent<GoToX>();
        goToGoalItem.target = spaceship;
        goToGoalItem.actionName = "Go To Spaceship Item";

        //Debug.Log("Status R5");

        // -- Go To Base
        goToBase = this.gameObject.AddComponent<GoToX>();
        goToBase.target = spaceship;
        goToBase.actionName = "Go To Base";
        
        //Debug.Log("Status R6");

        // -- Recover spaceship Item
        applyItem = this.gameObject.AddComponent<UseX>();
        applyItem.actionName = "Recover Spaceship Item";

        //Debug.Log("Status R7");

        // -- Collect spaceship Item
        collectItem = this.gameObject.AddComponent<CollectX>();
        collectItem.actionName = "Collect Spaceship Item";

        //Debug.Log("Status R8");

        // -- Drop any Item
        dropAnyItem = this.gameObject.AddComponent<DropAny>();
        dropAnyItem.actionName = "Drop Any Item (for Spaceship)";

        //Debug.Log("Status R9");

        // Add actions to list
        subactions = new List<GAction>();
        subactions.Add(goToGoalItem);
        subactions.Add(collectItem);
        subactions.Add(dropAnyItem);
        subactions.Add(applyItem);
        subactions.Add(goToBase);

        //Debug.Log("Status R10");

    }

    public void LateUpdate()
    {

        //Debug.Log("Perform: " + perform);
        //Debug.Log("Status R_1");

        if (perform)
        {
            this.RunSubActions();

            if(target_item != null)
            {
                //Debug.Log("Target Item: " + target_item);
                ObjectItem objectItem = target_item.GetComponent<ObjectItem>();
                if((Vector3.Distance(this.transform.position, target_item.transform.position) < agentData.range) && 
                    !(agentData.beliefs.HasState("Has Mole SSItem " + objectItem.ID)))
                    agentData.targetDirection.transform.position = target_item.transform.position;
                else
                    agentData.targetDirection.transform.localPosition = new Vector3(0.0f, 0.5f, 3.0f);
            }
            else
            {

            }
        }

        //Debug.Log("Status R_2");

        // If no plan, create a new plan
        if (planner == null | actionQueue == null)
        {
            spaceship_items = GameObject.FindGameObjectsWithTag("spaceship2_item");
            canRecover = false;
            cost = 9999999999;

            
            // Select a plan
            foreach(GameObject item in spaceship_items)
            {
                if (item != null)
                {
                    //Debug.Log("Item: " + item);
                    ObjectItem objectItem = item.GetComponent<ObjectItem>();

                    // Ensure that the item was detected before doing any plan
                    if (agentData.beliefs.HasState("Detect Mole SSItem " + objectItem.ID) && !(agentData.beliefs.HasState("Recover " + objectItem.ID)))
                    {

                        // Configure actions
                        configureActions(item);

                        // SubGoal
                        SubGoal recoverItemGoal = new SubGoal("Recover " + objectItem.ID, 1, true);

                        // Plan
                        Queue<GAction> actionQueueItem = this.RunSubPlan(recoverItemGoal, subactions);

                        if (actionQueueItem != null)
                        {

                            // Compute Plan Cost
                            float subplanCost = 0;
                            foreach(GAction a in actionQueueItem)
                            {
                                subplanCost += a.cost;
                            }

                            // Store plan
                            if (subplanCost < cost)
                            {
                                // Update plan parameters
                                canRecover = true;
                                cost = subplanCost;
                                actionQueue = actionQueueItem;
                                currentGoal = recoverItemGoal;
                                target_item = item;

                                // Update preconditions and effects
                                WorldState recoverItem = new WorldState("Recover " + objectItem.ID, 1);
                                WorldState detectItem = new WorldState("Detect Mole SSItem " + objectItem.ID, 1);

                                this.preConditions = new WorldState[]{detectItem};
                                this.afterEffects = new WorldState[]{recoverItem};
                                this.preconditions.Clear();
                                this.preconditions.Add("Detect Mole SSItem " + objectItem.ID, 1);
                                this.effects.Clear();
                                this.effects.Add("Recover " + objectItem.ID, 1);
                                this.actionName = "Recover " + objectItem.ID;

                                
                            }
                        }
                    } 
                }   
            }

            if (actionQueue != null && target_item != null)
            {
                configureActions(target_item);
            }
            /*
            foreach(GameObject item in spaceship_items)
            {
                if (item != null)
                {
                    ObjectItem objectItem = item.GetComponent<ObjectItem>();

                    // Ensure that the item was detected before doing any plan
                    if (agentData.beliefs.HasState("Detect Mole SSItem " + objectItem.ID) && !(agentData.beliefs.HasState("Recover " + objectItem.ID)))
                    {
                        //Calculate Cost
                        goToGoalItem.target = item;
                        goToGoalItem.CalculateReachability(this.transform.position);
                        
                        if(goToBase.isReachable && goToGoalItem.isReachable)
                        {
                            float subplanCost = goToBase.cost + goToGoalItem.cost;

                            // Store plan
                            if (subplanCost < cost)
                            {
                                // Update plan parameters
                                canRecover = true;
                                cost = subplanCost;
                                target_item = item;
                            }
                        }              
                    }
                }
            }

            if (canRecover && target_item != null)
            {
                ObjectItem objectItem = target_item.GetComponent<ObjectItem>();

                // Configure actions
                configureActions(target_item);

                // SubGoal
                SubGoal recoverItemGoal = new SubGoal("Recover " + objectItem.ID, 1, true);

                // Plan
                Queue<GAction> actionQueueItem = this.RunSubPlan(recoverItemGoal, subactions);

                // If valid path
                if (actionQueueItem != null)
                {

                    actionQueue = actionQueueItem;
                    currentGoal = recoverItemGoal;

                    // Update preconditions and effects
                    WorldState recoverItem = new WorldState("Recover " + objectItem.ID, 1);
                    WorldState detectItem = new WorldState("Detect Mole SSItem " + objectItem.ID, 1);

                    this.preConditions = new WorldState[]{detectItem};
                    this.afterEffects = new WorldState[]{recoverItem};
                    this.preconditions.Clear();
                    this.preconditions.Add("Detect Mole SSItem " + objectItem.ID, 1);
                    this.effects.Clear();
                    this.effects.Add("Recover " + objectItem.ID, 1);
                    this.actionName = "Recover " + objectItem.ID;
                }
            }*/
        }

        //Debug.Log("Status R_3");
    }

    public void configureActions(GameObject item)
    {
        GameObject spaceship = GameObject.FindGameObjectWithTag("spaceship2");

        // Object item
        ObjectItem objectItem = item.GetComponent<ObjectItem>();
        WorldState collectableItem = new WorldState("Collectable Mole SSItem " + objectItem.ID, 1);
        WorldState inSpaceship = new WorldState("InSpaceship2", 1);
        WorldState hasItem = new WorldState("Has Mole SSItem " + objectItem.ID, 1);
        WorldState recoverItem = new WorldState("Recover " + objectItem.ID, 1);

        // -- Go To Spaceship Item
        List<WorldState> goToGoalItemPrecond = new List<WorldState>();
        List<WorldState> goToGoalItemEffects = new List<WorldState>();
        goToGoalItemEffects.Add(collectableItem);
        goToGoalItem.target = item;
        goToGoalItem.distance = 3.0f;
        goToGoalItem.beliefTrigger = "Collectable Mole SSItem " + objectItem.ID;
        goToGoalItem.UpdateConditions(goToGoalItemPrecond, goToGoalItemEffects);
        goToGoalItem.CalculateReachability(this.transform.position);

        // -- Go To Base
        List<WorldState> goToBasePrecond = new List<WorldState>();
        List<WorldState> goToBaseEffects = new List<WorldState>();
        goToBaseEffects.Add(inSpaceship);
        goToBase.distance = 8.0f;
        goToBase.UpdateConditions(goToBasePrecond, goToBaseEffects);
        goToBase.CalculateReachability(item.transform.position);
        
        //goToBase.beliefTrigger = "InSpaceship2";

        // -- Recover spaceship Item
        List<WorldState> applyItemPrecond = new List<WorldState>();
        List<WorldState> applyItemEffects = new List<WorldState>();
        WorldState posX = new WorldState("Agent X POS", spaceship.transform.position[0]);
        WorldState posY = new WorldState("Agent Y POS", spaceship.transform.position[1]);
        WorldState posZ = new WorldState("Agent Z POS", spaceship.transform.position[2]);
        applyItemPrecond.Add(inSpaceship);
        applyItemPrecond.Add(hasItem);
        applyItemPrecond.Add(posX);
        applyItemPrecond.Add(posY);
        applyItemPrecond.Add(posZ);
        applyItemEffects.Add(recoverItem);
        applyItem.target = item;
        applyItem.UpdateConditions(applyItemPrecond, applyItemEffects);

        // -- Collect spaceship Item
        collectItem.target = item;
        collectItem.UpdateConditions(new List<WorldState>(), new List<WorldState>());

        // -- Drop Any Item
        // --
        
    }

    public GoToX createGoToAction(string[] preconditions, string[] effects)
    {
        GoToX act = this.gameObject.AddComponent<GoToX>();
        //act.SetAgentParams(this.agent, this.beliefs, this.backpack,  this.targetDirection, this.range);
        return act;
    }



    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    CHECK ACTION STATES
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    // Check if the action is achievable
    public override bool IsAchievable()
    {
        return canRecover;
    }

    public override bool IsAchievableGiven(Dictionary<string, float> conditions)
    {
        if (canRecover)
        {
            foreach (KeyValuePair<string, float> p in preconditions)
            {
                if (!conditions.ContainsKey(p.Key))
                    return false;
            }
            return true;
        }
        else {return false;}
    }

    public override bool IsFinished()
    {
        Debug.Log("Is Finished?");
        if (canRecover)
        {
            if (this.FinishedSubPlan())
            {
                perform = false;
                return true;
            }
        }
        return false;
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
        perform = true;
    }


}
