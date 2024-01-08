using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Definition of the node
public class Node2
{
    public Node2 parent;
    public float cost;
    public Dictionary<string, float> state;
    public MoleAction action;

    public Node2(Node2 parent, float cost, Dictionary<string, float> allstates, MoleAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, float>(allstates);
        this.action = action;
    }

    public Node2(Node2 parent, float cost, Dictionary<string, float> allstates, Dictionary<string, float> beliefstates, MoleAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, float>(allstates);
        foreach (KeyValuePair<string, float> b in beliefstates)
            if (!this.state.ContainsKey(b.Key))
                this.state.Add(b.Key, b.Value);
        this.action = action;
    }
}

// Definition of the Planner
public class MolePlanner
{
    public Queue<MoleAction> plan(List<MoleAction> actions, Dictionary<string, float> goal, WorldStates beliefstates)
    {
        // Get achievable actions
        List<MoleAction> usableActions = new List<MoleAction>();
        foreach (MoleAction a in actions)
        {
            if (a.IsAchievable())
            {
                //Debug.Log("Achievable: " + a);
                usableActions.Add(a);
            }
        }
        

        // Generate the graph
        List<Node2> leaves = new List<Node2>();
        Node2 start = new Node2(null, 0, GWorld.Instance.GetWorld().GetStates(), beliefstates.GetStates(), null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if(!success)
        {
            //Debug.Log("No Plan");
            return null;
        }

        // Find the cheapest node
        Node2 cheapest = null;
        foreach (Node2 leaf in leaves)
        {
            if(cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.cost < cheapest.cost)
                    cheapest = leaf;
            }
        }
        Debug.Log("Possible solutions: " + leaves.Count);
        
        // Extract the path
        List<MoleAction> result = new List<MoleAction>();
        Node2 n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        Queue<MoleAction> queue = new Queue<MoleAction>();
        foreach(MoleAction a in result)
        {
            queue.Enqueue(a);
        } 

        Debug.Log("The Plan is: ");
        foreach(MoleAction a in queue)
        {
            Debug.Log("Q: " + a.actionName);
        }

        return queue;
    }

    // Build the graph of the GOAP
    private bool BuildGraph(Node2 parent, List<Node2> leaves, List<MoleAction> usableActions, Dictionary<string, float> goal)
    {
        bool foundPath = false;
        //Debug.Log("Node");
        foreach (MoleAction action in usableActions)
        {
            
            // Ensure that the current action is feasible
            if(action.IsAchievableGiven(parent.state))
            {
                //Debug.Log("Action " + action.actionName + " is achievable");
                // Modify states according that action
                Dictionary<string, float> currentState = new Dictionary<string, float>(parent.state);


                foreach (KeyValuePair<string, float> eff in action.effects)
                {
                    if (!currentState.ContainsKey(eff.Key))
                        currentState.Add(eff.Key, eff.Value);
                    else
                        currentState[eff.Key] = eff.Value;
                }
                /*
                foreach (KeyValuePair<string, float> ceff in action.counter_effects)
                {
                    if (!currentState.ContainsKey(ceff.Key))
                        currentState.Remove(ceff.Key);
                }*/

                // Update Node
                Node2 node = new Node2(parent, parent.cost + action.cost, currentState, action);

                // Check if the goal was achieved
                if (GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    List<MoleAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                        foundPath = true;
                }
            }
            //else
            //    Debug.Log("Action " + action.actionName + " is not achievable");
        }
        
        return foundPath;
    }

    // Check if a goal was achieved
    private bool GoalAchieved(Dictionary<string, float> goal, Dictionary<string, float> state)
    {
        foreach (KeyValuePair<string, float> g in goal)
        {
            if (!state.ContainsKey(g.Key))
                return false;
        }

        return true;
    }

    // Actions already executed (and removed from the tree)
    private List<MoleAction> ActionSubset(List<MoleAction> actions, MoleAction removeMe)
    {
        List<MoleAction> subset = new List<MoleAction>();
        foreach (MoleAction a in actions)
        {
            if (!a.Equals(removeMe))
                subset.Add(a);
        }
        return subset;
    }
}
