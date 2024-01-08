using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

/*
public class SubGoal
{
    public Dictionary<string, float> sgoals;
    public bool remove;

    public SubGoal(string s, float i, bool r)
    {
        sgoals = new Dictionary<string, float>();
        sgoals.Add(s, i);
        remove = r;
    }
}*/

public class GAgent : MonoBehaviour
{

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    AGENT PARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    // GOAP items
    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, float> goals = new Dictionary<SubGoal, float>();
    public GInventory inventory = new GInventory();
    public WorldStates beliefs = new WorldStates();


    // Agent Backpack
    public List<GameObject> backpack = new List<GameObject>();
    public int indexItem = 4;


    // Agent Navigation
    public NavMeshAgent agent;

    
    //GOAP Planner Instantiation
    GPlanner planner;
    Queue<GAction> actionQueue;
    public GAction currentAction;
    SubGoal currentGoal;


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    AGENT INITIALIZATION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    public void Start()
    {
        // Navmesh initialization
        agent = this.gameObject.GetComponent<NavMeshAgent>();

        // Initialize inventory
        for (int i=0; i<9; i++)
        {
            backpack.Add(null);
        }
        
    }


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    AGENT BACKPACK
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    void FixedUpdate()
    {
        
    }

    

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    GOAP PLANNER INTERACTION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    bool invoked = false;
    void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
    }

    public void LateUpdate()
    {
        //Debug.Log("Status P1");
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

        //Debug.Log("Status P2");

        // Create a new plan
        if (planner == null | actionQueue == null)
        {
            planner = new GPlanner();

            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach(KeyValuePair<SubGoal, float> sg in sortedGoals)
            {
                actionQueue = planner.plan(actions, sg.Key.sgoals, beliefs);
                if (actionQueue != null)
                {
                    currentGoal = sg.Key;
                    break;
                }
            }
        }

        //Debug.Log("Status P3");

        // Complete goal
        if (actionQueue != null && actionQueue.Count == 0)
        {
            if (currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }
            planner = null;
            
        }

        //Debug.Log("Status P4");

        // Continue with the following nodes
        if (actionQueue != null && actionQueue.Count > 0)
        {
            
            currentAction = actionQueue.Dequeue();
            // Debug.Log("Current action = " + currentAction.actionName);

            if (currentAction.PrePerform())
            {
                currentAction.Perform();
            }
            else // Avoid the agent to get stuck in the middle of a plan
            {
                actionQueue = null;
            }
        }

        //Debug.Log("Status P5");
    }
}
