using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoleCuriosity : MoleAction
{
    
    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public bool plan = false;
    public bool canCollect = false;
    public GameObject[] spaceship_items;
    public GameObject[] weapon;
    public GameObject[] edible;
    public GameObject target_item;


    // Possible subactions
    private MoleGoToX goToGoalItem;
    private MoleCollectX collectItem;
    private MoleDropAny dropAnyItem;
    public List<MoleAction> subactions;



    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    SELECT WHICH ITEM TO RECOVER
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void Start()
    {
        // Configuration
        actionName = "Curiosity";
        spaceship_items = GameObject.FindGameObjectsWithTag("spaceship1_item");
        weapon = GameObject.FindGameObjectsWithTag("weapon");
        edible = GameObject.FindGameObjectsWithTag("consumable");

        // -- Go To Item
        goToGoalItem = this.gameObject.AddComponent<MoleGoToX>();
        goToGoalItem.actionName = "Go To Item";

        // -- Collect Item
        collectItem = this.gameObject.AddComponent<MoleCollectX>();
        collectItem.actionName = "Collect Item";

        // -- Drop Any Item
        dropAnyItem = this.gameObject.AddComponent<MoleDropAny>();
        dropAnyItem.actionName = "Drop Any Item";

        // Add actions to list
        subactions = new List<MoleAction>();
        subactions.Add(goToGoalItem);
        subactions.Add(collectItem);
        subactions.Add(dropAnyItem);
    }

    public void LateUpdate()
    {
        /*
        // If no plan, create a new plan
        if (plan)
        {
            //Debug.Log("planning");
            canCollect = false;
            cost = 99999999;

            // Select a plan
            if (target_item != null)
            {
                // Get item description
                string identification = "";
                ObjectItem objectItem = item.GetComponent<ObjectItem>();

                // Ensure that the item was detected before doing any plan
                if (this.beliefs.HasState("Detect Mole SSItem " + objectItem.ID))
                {
                    //Debug.Log("Mole " + this.agentParams.ID + " - Plan for recovering SSItem " + objectItem.ID );

                    // Configure actions
                    ObjectItem objectItem = item.GetComponent<ObjectItem>();
                    WorldState collectableItem = new WorldState("Collectable Mole SSItem " + objectItem.ID, 1);
                    WorldState hasItem = new WorldState("Has Mole SSItem " + objectItem.ID, 1);

                    // -- Go To Spaceship Item
                    goToGoalItem.SetAgentStatus(this.goals, this.beliefs, this.backpack, this.indexItem,
                                        this.targetDirection, this.pickableLayerMask, this.hit, this.centerBias, this.pickUpParent, this.pickUpParentStatic,
                                        this.range, this.agentParams);
                    List<WorldState> goToGoalItemPrecond = new List<WorldState>();
                    List<WorldState> goToGoalItemEffects = new List<WorldState>();
                    goToGoalItemEffects.Add(collectableItem);
                    goToGoalItem.target = target_item;
                    goToGoalItem.distance = 3.0f;
                    goToGoalItem.beliefTrigger = "Collectable Mole SSItem " + objectItem.ID;
                    goToGoalItem.UpdateConditions(goToGoalItemPrecond, goToGoalItemEffects);
                    goToGoalItem.CalculateReachability(this.transform.position);

                    // -- Collect spaceship Item
                    collectItem.SetAgentStatus(this.goals, this.beliefs, this.backpack, this.indexItem,
                                        this.targetDirection, this.pickableLayerMask, this.hit, this.centerBias, this.pickUpParent, this.pickUpParentStatic,
                                        this.range, this.agentParams);
                    collectItem.target = item;
                    collectItem.UpdateConditions(new List<WorldState>(), new List<WorldState>());

                    // -- Drop Any Item
                    // --

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


                        // Update plan parameters
                        canCollect = true;
                        cost = subplanCost;
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
                }
            }

            if (actionQueue != null && target_item != null)
            {
                   
            }

            plan = false;

        }*/
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
        // If no plan
        if ((planner == null | actionQueue == null) && !plan)
        {
            //Debug.Log("Mole " + this.agentParams.ID + " - Start planning for recovering");
            plan = true;
        }

        // if can attack agents
        if (canCollect)
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
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    GET UTILITY OF THE ACTION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public override float ComputeUtilityScore()
    {
        // Extract point distance
        spaceship_items = GameObject.FindGameObjectsWithTag("spaceship1_item");
        weapon = GameObject.FindGameObjectsWithTag("weapon");
        edible = GameObject.FindGameObjectsWithTag("consumable");

        List<float> itemScore = new List<float>();
        List<float> curiosity = new List<float>();
        List<GameObject> items2collect = new List<GameObject>();

        foreach(GameObject item in spaceship_items)
        {
            ObjectItem objectItem = item.GetComponent<ObjectItem>();

            if (this.beliefs.HasState("Detect Babo SSItem " + objectItem.ID) && GWorld.Instance.GetWorld().HasState("Available Babo SSItem " + objectItem.ID))
            {
                NavMeshPath path = new NavMeshPath();
                bool hasPath = NavMesh.CalculatePath(item.transform.position, transform.position, NavMesh.AllAreas, path);
                if (hasPath)
                {
                    itemScore.Add(CalculatePathDistance(path));
                    items2collect.Add(item);
                    foreach(AgentCuriosity agentCuriosity in agentParams.spaceshipCuriosity)
                    {
                        if(agentCuriosity.item == item)
                        {
                            curiosity.Add(agentCuriosity.curiosity);
                            
                            break;
                        }
                    }
                }
            }
        }

        foreach(GameObject item in weapon)
        {
            WeaponItem weaponItem = item.GetComponent<WeaponItem>();

            if (this.beliefs.HasState("Detect Weapon " + weaponItem.weaponDescrib.ID) && GWorld.Instance.GetWorld().HasState("Available Weapon " + weaponItem.weaponDescrib.ID))
            {
                NavMeshPath path = new NavMeshPath();
                bool hasPath = NavMesh.CalculatePath(item.transform.position, transform.position, NavMesh.AllAreas, path);
                if (hasPath)
                {
                    itemScore.Add(CalculatePathDistance(path));
                    items2collect.Add(item);
                    foreach(AgentCuriosity agentCuriosity in agentParams.weaponCuriosity)
                    {
                        if(agentCuriosity.item == item)
                        {
                            curiosity.Add(agentCuriosity.curiosity);
                            break;
                        }
                    }
                }
            }
        }

        foreach(GameObject item in edible)
        {
            EdibleItem edibleItem = item.GetComponent<EdibleItem>();

            if (this.beliefs.HasState("Detect Edible " + edibleItem.edibleDescrib.ID) && GWorld.Instance.GetWorld().HasState("Available Edible " + edibleItem.edibleDescrib.ID))
            {
                NavMeshPath path = new NavMeshPath();
                bool hasPath = NavMesh.CalculatePath(item.transform.position, transform.position, NavMesh.AllAreas, path);
                if (hasPath)
                {
                    itemScore.Add(CalculatePathDistance(path));
                    items2collect.Add(item);
                    foreach(AgentCuriosity agentCuriosity in agentParams.edibleCuriosity)
                    {
                        if(agentCuriosity.item == item)
                        {
                            curiosity.Add(agentCuriosity.curiosity);
                            break;
                        }
                    }
                }
            }
        }

        // Compute Utility
        float utilityScore = 0;
        for(int k = 0; k < itemScore.Count; k++)
        {
            float itemScoreValue = Mathf.Pow((7/Mathf.Max(7, itemScore[k])),2)*(1/(1+Mathf.Exp(-10*curiosity[k]*agentParams.Mood+5)));
            if (utilityScore < itemScoreValue)
            {
                utilityScore = itemScoreValue;
                target_item = items2collect[k];
            }
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
