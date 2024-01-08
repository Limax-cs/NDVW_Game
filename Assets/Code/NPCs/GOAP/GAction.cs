using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GAction : MonoBehaviour
{

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    // Action parameters
    public string actionName = "Action";
    public float cost = 1.0f;
    public float duration = 0;
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    public WorldState[] afterCounterEffects;

    public Dictionary<string, float> preconditions;
    public Dictionary<string, float> effects;
    public Dictionary<string, float> counter_effects;

    public bool running = false;

    // Agent data
    public GOAPAgent agentData;

    //GOAP Planner Instantiation
    public GPlanner planner;
    public Queue<GAction> actionQueue;
    public GAction currentAction;
    public SubGoal currentGoal;


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION INITIALIZATION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public GAction()
    {
        //Debug.Log("Status A1");
        preconditions = new Dictionary<string, float>();
        effects = new Dictionary<string, float>();
        counter_effects = new Dictionary<string, float>();
        //Debug.Log("Status A2");
    }

    // Initialize World State of the action
    public void Start()
    {
        //Debug.Log("Status A3");
        agentData = GetComponent<GOAPAgent>();

        //Debug.Log("Status A4");

        if (preConditions != null)
            foreach (WorldState w in preConditions)
            {
                preconditions.Add(w.key, w.value);
            }

        //Debug.Log("Status A5");
        
        if (afterEffects != null)
            foreach (WorldState w in afterEffects)
            {
                effects.Add(w.key, w.value);
            }

        //Debug.Log("Status A6");

        if (afterCounterEffects != null)
            foreach (WorldState w in afterCounterEffects)
            {
                counter_effects.Add(w.key, w.value);
            }

        //Debug.Log("Status A7");
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

    public Queue<GAction> RunSubPlan(SubGoal sg, List<GAction> subactions)
    {
        // Create a new plan
        planner = new GPlanner();

        Queue<GAction> actionQueuePlanned = planner.plan(subactions, sg.sgoals, agentData.beliefs);
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
