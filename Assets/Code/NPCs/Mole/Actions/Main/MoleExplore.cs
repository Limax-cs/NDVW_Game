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
    private int exploreUpdate = 0;
    private float distanceCourse = 999999;


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
        exploreUpdate = exploreUpdate + 1;

        if (exploreUpdate > 30)
        {
            canExplore = false;
            cost = 9999999999;
            distanceCourse = 9999999999;

            foreach(GameObject eP in toExplorePoints)
            {
                if (Vector3.Distance(transform.position, eP.transform.position) + 20 < distanceCourse)
                {
                    // Determine if a path is reachable
                    NavMeshPath path = new NavMeshPath();
                    bool hasPath = NavMesh.CalculatePath(transform.position, eP.transform.position, NavMesh.AllAreas, path);

                    if (hasPath)
                    {
                        float ePcost = Vector3.Distance(transform.position, eP.transform.position);
                        if (ePcost < 100)
                        {
                            // Compute the distance of a path
                            float distance = CalculatePathDistance(path);
                            ePcost = distance;
                        }

                        // If cost is lower, select this point
                        if (ePcost < cost)
                        {
                            canExplore = true;
                            cost = ePcost;
                            distanceCourse = Vector3.Distance(transform.position, eP.transform.position);
                            target = eP;
                        }
                    }
                }
            }

            exploreUpdate = 0;
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
        // Extract point distance
        List<float> exploreDistance = new List<float>();
        foreach(GameObject eP in toExplorePoints)
        {
            if (Vector3.Distance(transform.position, eP.transform.position) < 25)
            {
                NavMeshPath path = new NavMeshPath();
                bool hasPath = NavMesh.CalculatePath(transform.position, eP.transform.position, NavMesh.AllAreas, path);
                if (hasPath)
                {
                    exploreDistance.Add(CalculatePathDistance(path));
                }
            }
            else
            {
                exploreDistance.Add(Vector3.Distance(transform.position, eP.transform.position));
            }
        }

        // Compute Utility
        float utilityDistance = 0;
        foreach(float eD in exploreDistance)
        {
            utilityDistance += Mathf.Pow((5/Mathf.Max(5, eD)),3);
        }

        if (utilityDistance > 1.0f)
            utilityDistance = 1.0f;
        
        return utilityDistance/2;
    }


}
