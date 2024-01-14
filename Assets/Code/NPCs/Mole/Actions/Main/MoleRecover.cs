using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoleRecover : MoleAction
{
    
    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public bool plan = false;
    public bool canRecover = false;
    public GameObject[] spaceship_items;
    public List<GameObject> items2collect;
    public GameObject target_item;


    // Possible subactions
    private MoleGoToX goToGoalItem;
    private MoleGoToX goToBase;
    private MoleUseX applyItem;
    private MoleCollectX collectItem;
    private MoleDropAny dropAnyItem;
    public List<MoleAction> subactions;

    private int replan_counter = 0;



    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    SELECT WHICH ITEM TO RECOVER
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void Start()
    {
        // Configuration
        actionName = "Recover";
        spaceship_items = GameObject.FindGameObjectsWithTag("spaceship2_item");
        items2collect = new List<GameObject>();
        foreach(GameObject item in spaceship_items)
        {
            items2collect.Add(item);
        }

        // Initialize Recover Actions
        GameObject spaceship = GameObject.FindGameObjectWithTag("spaceship2");

        // -- Go To Spaceship Item
        goToGoalItem = this.gameObject.AddComponent<MoleGoToX>();
        goToGoalItem.target = spaceship;
        goToGoalItem.actionName = "Go To Spaceship Item";

        // -- Go To Base
        goToBase = this.gameObject.AddComponent<MoleGoToX>();
        goToBase.target = spaceship;
        goToBase.actionName = "Go To Base";

        // -- Recover spaceship Item
        applyItem = this.gameObject.AddComponent<MoleUseX>();
        applyItem.actionName = "Recover Spaceship Item";

        // -- Collect spaceship Item
        collectItem = this.gameObject.AddComponent<MoleCollectX>();
        collectItem.actionName = "Collect Spaceship Item";

        dropAnyItem = this.gameObject.AddComponent<MoleDropAny>();
        dropAnyItem.actionName = "Drop Any Item";

        // Add actions to list
        subactions = new List<MoleAction>();
        subactions.Add(goToGoalItem);
        subactions.Add(collectItem);
        subactions.Add(applyItem);
        subactions.Add(goToBase);
        subactions.Add(dropAnyItem);
    }

    public void LateUpdate()
    {
        // If no plan, create a new plan
        if (plan)
        {
            //Debug.Log("planning");
            canRecover = false;
            cost = 99999999;

            // Select a plan
            spaceship_items = GameObject.FindGameObjectsWithTag("spaceship2_item");
            foreach(GameObject item in spaceship_items)
            {
                if (item != null)
                {
                    // Get item description
                    ObjectItem objectItem = item.GetComponent<ObjectItem>();

                    // Ensure that the item was detected before doing any plan
                    if (this.beliefs.HasState("Detect Mole SSItem " + objectItem.ID) && (GWorld.Instance.GetWorld().HasState("Available Mole SSItem " + objectItem.ID) || this.beliefs.HasState("Has Mole SSItem " + objectItem.ID)) && !(this.beliefs.HasState("Recover " + objectItem.ID)))
                    {
                        //Debug.Log("Mole " + this.agentParams.ID + " - Plan for recovering SSItem " + objectItem.ID );

                        // Configure actions
                        configureActions(item);

                        // SubGoal
                        SubGoal recoverItemGoal = new SubGoal("Recover " + objectItem.ID, 1, true);

                        // Plan
                        Queue<MoleAction> actionQueueItem = this.RunSubPlan(recoverItemGoal, subactions);

                        if (actionQueueItem != null)
                        {
                            Debug.Log("Mole " + this.agentParams.ID + " - Found a plan for SSItem " + objectItem.ID );

                            // Compute Plan Cost
                            float subplanCost = 0;
                            foreach(MoleAction a in actionQueueItem)
                            {
                                subplanCost += a.cost;
                            }

                            // Preference to accumulate items
                            if (!this.beliefs.HasState("Has Mole SSItem " + objectItem.ID))
                                subplanCost -= 25;

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
                ObjectItem objectItem = target_item.GetComponent<ObjectItem>();
                Debug.Log("Mole " + this.agentParams.ID + " - Decided to go for SSItem " + objectItem.ID );
                configureActions(target_item);    
            }

            plan = false;

        }
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
        goToGoalItem.SetAgentStatus(this.goals, this.beliefs, this.backpack, this.indexItem,
                            this.targetDirection, this.pickableLayerMask, this.hit, this.centerBias, this.pickUpParent, this.pickUpParentStatic,
                            this.range, this.agentParams);
        List<WorldState> goToGoalItemPrecond = new List<WorldState>();
        List<WorldState> goToGoalItemEffects = new List<WorldState>();
        goToGoalItemEffects.Add(collectableItem);
        goToGoalItem.target = item;
        goToGoalItem.distance = 3.0f;
        goToGoalItem.beliefTrigger = "Collectable Mole SSItem " + objectItem.ID;
        goToGoalItem.UpdateConditions(goToGoalItemPrecond, goToGoalItemEffects);
        goToGoalItem.CalculateReachability(this.transform.position);

        // -- Go To Base
        goToBase.SetAgentStatus(this.goals, this.beliefs, this.backpack, this.indexItem,
                            this.targetDirection, this.pickableLayerMask, this.hit, this.centerBias, this.pickUpParent, this.pickUpParentStatic,
                            this.range, this.agentParams);
        List<WorldState> goToBasePrecond = new List<WorldState>();
        List<WorldState> goToBaseEffects = new List<WorldState>();
        goToBaseEffects.Add(inSpaceship);
        goToBase.distance = 8.0f;
        goToBase.UpdateConditions(goToBasePrecond, goToBaseEffects);
        goToBase.CalculateReachability(item.transform.position);
        
        //goToBase.beliefTrigger = "InSpaceship2";

        // -- Recover spaceship Item
        applyItem.SetAgentStatus(this.goals, this.beliefs, this.backpack, this.indexItem,
                            this.targetDirection, this.pickableLayerMask, this.hit, this.centerBias, this.pickUpParent, this.pickUpParentStatic,
                            this.range, this.agentParams);
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
        collectItem.SetAgentStatus(this.goals, this.beliefs, this.backpack, this.indexItem,
                            this.targetDirection, this.pickableLayerMask, this.hit, this.centerBias, this.pickUpParent, this.pickUpParentStatic,
                            this.range, this.agentParams);
        collectItem.target = item;
        collectItem.UpdateConditions(new List<WorldState>(), new List<WorldState>());

        // -- Drop Any Item
        dropAnyItem.SetAgentStatus(this.goals, this.beliefs, this.backpack, this.indexItem,
                                        this.targetDirection, this.pickableLayerMask, this.hit, this.centerBias, this.pickUpParent, this.pickUpParentStatic,
                                        this.range, this.agentParams);
        
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    CHECK ACTION STATES
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    // Check if the action is achievable
    public override bool IsAchievable()
    {
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, float> conditions)
    {
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
        replan_counter = replan_counter + 1;
        // If no plan
        if ((planner == null | actionQueue == null) && !plan)
        {
            //Debug.Log("Mole " + this.agentParams.ID + " - Start planning for recovering");
            plan = true;
            replan_counter = 0;
        }

        // if can recover item
        if (canRecover)
        {
            // If close enough, look at the object
            if(target_item != null)
            {
                //Debug.Log("Target Item: " + target_item);
                ObjectItem objectItem = target_item.GetComponent<ObjectItem>();
                if((Vector3.Distance(this.transform.position, target_item.transform.position) < this.range) && 
                    !(this.beliefs.HasState("Has Mole SSItem " + objectItem.ID)))
                    this.targetDirection.transform.position = target_item.transform.position;
                else
                    this.targetDirection.transform.localPosition = new Vector3(0.0f, 0.5f, 3.0f);
            }

            // Run subactions during execution
            this.RunSubActions();
        }

        if (replan_counter > 300)
        {
            replan_counter = 0;
            plan = false;
        }
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    GET UTILITY OF THE ACTION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public override float ComputeUtilityScore()
    {
        // Extract point distance
        spaceship_items = GameObject.FindGameObjectsWithTag("spaceship2_item");
        GameObject spaceship = GameObject.FindGameObjectWithTag("spaceship2");

        List<float> itemScore = new List<float>();
        List<int> notCollected = new List<int>();
        List<float> curiosity = new List<float>();

        foreach(GameObject item in spaceship_items)
        {
            ObjectItem objectItem = item.GetComponent<ObjectItem>();

            if (this.beliefs.HasState("Detect Mole SSItem " + objectItem.ID) && (GWorld.Instance.GetWorld().HasState("Available Mole SSItem " + objectItem.ID) || this.beliefs.HasState("Has Mole SSItem " + objectItem.ID)) && !(this.beliefs.HasState("Recover " + objectItem.ID)))
            {
                // Hasn't the Item
                if(!this.beliefs.HasState("Has Mole SSItem " + objectItem.ID))
                {
                    NavMeshPath path = new NavMeshPath();
                    bool hasPath = NavMesh.CalculatePath(transform.position, item.transform.position, NavMesh.AllAreas, path);
                    NavMeshPath path2 = new NavMeshPath();
                    bool hasPath2 = NavMesh.CalculatePath(item.transform.position, spaceship.transform.position, NavMesh.AllAreas, path2);
                    if (hasPath && hasPath2)
                    {
                        itemScore.Add(CalculatePathDistance(path) + CalculatePathDistance(path2));
                        notCollected.Add(1);
                    }
                }
                // Have the item
                else
                {
                    Debug.Log("Hi");
                    NavMeshPath path2 = new NavMeshPath();
                    bool hasPath2 = NavMesh.CalculatePath(item.transform.position, spaceship.transform.position, NavMesh.AllAreas, path2);
                    if (hasPath2)
                    {
                        Debug.Log("Hi2");
                        itemScore.Add(CalculatePathDistance(path2));
                        notCollected.Add(0);
                    }
                }
            }
        }

        // Compute Utility
        float utilityScore = 0;
        for(int k = 0; k < itemScore.Count; k++)
        {
            float itemScoreValue = Mathf.Pow((50/Mathf.Max(50, itemScore[k] - 25*notCollected[k])),2);
            if (utilityScore < itemScoreValue)
                utilityScore = itemScoreValue;
        }
        
        return utilityScore;
    }

    public float CalculatePathDistance(NavMeshPath path)
    {
        float totalDistance = 0f;

        if (path != null && path.corners.Length > 1)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
        }

        return totalDistance;
    }

}
