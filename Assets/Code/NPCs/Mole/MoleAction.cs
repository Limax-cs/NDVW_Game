using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class MoleAction : MonoBehaviour
{

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    // ACTION PARAMETERS
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Action Parameters")]
    public string actionName = "Action";
    public float cost = 1.0f;
    public float duration = 0;
    public WorldState[] preConditions;
    public WorldState[] afterEffects;

    public Dictionary<string, float> preconditions;
    public Dictionary<string, float> effects;

    public bool running = false;


    // GOAP PLANNER
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("GOAP Planner")]
    public MolePlanner planner;
    public Queue<MoleAction> actionQueue;
    public MoleAction currentAction;
    public SubGoal currentGoal;


    // AGENT PARAMETERS
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Agent Parameters")]

    // Action-Oriented Parameters
    public Dictionary<SubGoal, float> goals;
    public WorldStates beliefs;

    // Backpack
    public List<GameObject> backpack;
    public int indexItem;

    // Agent Attention
    public GameObject targetDirection;
    public LayerMask pickableLayerMask;
    public RaycastHit hit;
    public float range;
    public Vector3 centerBias;
    public Transform pickUpParent;
    public Transform pickUpParentStatic;

    // Navigation
    public NavMeshAgent agent;

    // Agent Attributes
    public AgentParams agentParams;


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION INITIALIZATION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public MoleAction()
    {
        preconditions = new Dictionary<string, float>();
        effects = new Dictionary<string, float>();
    }

    // Initialize World State of the action
    public void Start()
    {
        // Initialize
        goals = new Dictionary<SubGoal, float>();
        beliefs = new WorldStates();
        backpack = new List<GameObject>();
        indexItem = 4;
        targetDirection = new GameObject();
        hit = new RaycastHit();
        range = 7;
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        agentParams = new AgentParams(-1, 1,1);


        // Preconditinos and Effects

        if (preConditions != null)
            foreach (WorldState w in preConditions)
            {
                preconditions.Add(w.key, w.value);
            }
        
        if (afterEffects != null)
            foreach (WorldState w in afterEffects)
            {
                effects.Add(w.key, w.value);
            }
    }

    // Update Agent Status
    public void SetAgentStatus(
        Dictionary<SubGoal, float> goals, 
        WorldStates beliefs, 
        List<GameObject> backpack,
        int indexItem,
        GameObject targetDirection,
        LayerMask pickableLayerMask,
        RaycastHit hit,
        Vector3 centerBias,
        Transform pickUpParent,
        Transform pickUpParentStatic,
        float range,
        AgentParams agentParams
        )
    {
        this.goals = goals;
        this.beliefs = beliefs;
        this.backpack = backpack;
        this.indexItem = indexItem;
        this.targetDirection = targetDirection;
        this.pickableLayerMask = pickableLayerMask;
        this.pickUpParent = pickUpParent;
        this.pickUpParentStatic = pickUpParentStatic;
        this.range = range;
        this.hit = hit;
        this.centerBias = centerBias;
        this.agent = this.gameObject.GetComponent<NavMeshAgent>();
        this.agentParams = agentParams;
    }



    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    CHECK ACTION STATES
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    // Check if the action is achievable
    public abstract bool IsAchievable();
    public abstract bool IsAchievableGiven(Dictionary<string, float> conditions);

    // Finishing Action Condition
    public abstract bool IsFinished();


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    PERFORM ACTION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    // Pre- Post- Perform
    public abstract bool PrePerform();
    public abstract bool PostPerform();

    // Perform the action
    public abstract void Perform();


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    GET UTILITY OF THE ACTION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public abstract float ComputeUtilityScore();


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    PERFORM SUB-ACTIONS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    bool invoked = false;
    void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
        //Debug.Log(currentAction + " finished");
    }

    public Queue<MoleAction> RunSubPlan(SubGoal sg, List<MoleAction> subactions)
    {
        // Create a new plan
        planner = new MolePlanner();

        Queue<MoleAction> actionQueuePlanned = planner.plan(subactions, sg.sgoals, this.beliefs);
        return actionQueuePlanned;
    }

    public void RunSubActions()
    {
        
        // Run the current action
        if (currentAction != null && currentAction.running)
        {
            if (currentAction.IsFinished())
            {
                if (!invoked)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }

        // Complete goal
        if (actionQueue != null && actionQueue.Count == 0)
        {
            //if (currentGoal.remove)
            //{
                //goals.Remove(currentGoal);
            //}
            planner = null;
            
        }

        // Continue with the following nodes
        if (actionQueue != null && actionQueue.Count > 0)
        {
            //Debug.Log("Action Queue Count: " + actionQueue.Count);
            
            currentAction = actionQueue.Dequeue();
            //Debug.Log("Current action = " + currentAction.actionName);
            //Debug.Log("Action Queue Count Then: " + actionQueue.Count);

            if (currentAction.PrePerform())
            {
                currentAction.Perform();
            }
            else // Avoid the agent to get stuck in the middle of a plan
            {
                actionQueue = null;
            }
        }
    }

    public void SubPlanPostPerform()
    {
        
    }

    public bool FinishedSubPlan()
    {
        return ((actionQueue == null) || (actionQueue != null && actionQueue.Count == 0));
    }

}
