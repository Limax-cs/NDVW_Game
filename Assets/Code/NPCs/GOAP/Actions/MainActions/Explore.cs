using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Explore : GAction
{
    
    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public GameObject target;
    public string targetTag;

    // Explorable points
    private GameObject[] explorePoint;
    public List<GameObject> toExplorePoints;
    public bool canExplore = false;


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    SELECT WHAT TO EXPLORE (UTILITIES)
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void Start()
    {
        //Debug.Log("Status E1");
        base.Start();

        //Debug.Log("Status E2");

        // Configuration
        actionName = "Explore";

        // Add effect
        WorldState eff = new WorldState();
        eff.key = "Explore";
        eff.value = 1;
        afterEffects = new WorldState[]{eff};
        effects.Add("Explore", 1);

        //Debug.Log("Status E3");

        // List explorable points
        explorePoint = GameObject.FindGameObjectsWithTag("explorable");
        toExplorePoints = new List<GameObject>();

        foreach(GameObject eP in explorePoint)
        {
            toExplorePoints.Add(eP);
        }

        //Debug.Log("Status E4");
    }

    public void LateUpdate()
    {
        //Debug.Log("Status E_1");
        canExplore = false;
        cost = 9999999999;

        foreach(GameObject eP in toExplorePoints)
        {
            // Determine if a path is reachable
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, eP.transform.position, NavMesh.AllAreas, path);

            if (hasPath)
            {
                // Compute the distance of a path
                float distance = CalculatePathDistance(path);
                float ePcost = distance;

                // If cost is lower, select this point
                if (ePcost < cost)
                {
                    canExplore = true;
                    cost = ePcost;
                    target = eP;
                }
            }
        }
        //Debug.Log("Status E_2");
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


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    CHECK ACTION STATES
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    // Check if the action is achievable
    public override bool IsAchievable()
    {
        return canExplore;
    }

    public override bool IsAchievableGiven(Dictionary<string, float> conditions)
    {
        // Determine if it is reachable according to reachibility
        if (canExplore)
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
        if (canExplore)
        {
            if (agentData.agent.hasPath && agentData.agent.remainingDistance < 3f)
            {
                return true;
            }
            else {return false;}
        }
        else {return false;}
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
        toExplorePoints.Remove(target);
        return true;
    }

    public override void Perform()
    {
        // Set agent destination
        //if (target == null && targetTag != "")
        //    target = GameObject.FindWithTag(targetTag);
                
        if (target != null)
        {
            running = true;
            agentData.agent.SetDestination(target.transform.position);
        }
    }


}
