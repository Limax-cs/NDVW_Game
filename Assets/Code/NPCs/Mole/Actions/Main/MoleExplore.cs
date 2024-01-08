using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoleExplore : MoleAction
{
    
    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public GameObject target;

    // Explorable points
    private GameObject[] explorePoint;
    public List<GameObject> toExplorePoints;
    public bool canExplore = false;


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    SELECT WHAT TO EXPLORE (UTILITIES)
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void Start()
    {
        base.Start();

        // Configuration
        actionName = "Explore";

        // Add effect
        WorldState eff = new WorldState();
        eff.key = "Explore";
        eff.value = 1;
        afterEffects = new WorldState[]{eff};
        effects.Add("Explore", 1);

        // List explorable points
        explorePoint = GameObject.FindGameObjectsWithTag("explorable");
        toExplorePoints = new List<GameObject>();

        foreach(GameObject eP in explorePoint)
        {
            toExplorePoints.Add(eP);
        }
    }

    public void LateUpdate()
    {
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

        if (IsFinished())
        {
            toExplorePoints.Remove(target);
            running = false;
        }
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
        return canExplore;
    }

    public override bool IsFinished()
    {
        if (canExplore)
        {
            if (this.agent.hasPath && this.agent.remainingDistance < 3f)
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
        return true;
    }

    public override void Perform()
    {
        if (target != null)
        {
            running = true;
            this.agent.SetDestination(target.transform.position);
        }
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    GET UTILITY OF THE ACTION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public override float ComputeUtilityScore()
    {
        if (canExplore)
            return 0;
        else
            return 0;
    }


}
