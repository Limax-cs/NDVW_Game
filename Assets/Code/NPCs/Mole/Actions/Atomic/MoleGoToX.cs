using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoleGoToX : MoleAction
{

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    
    public GameObject target;
    public string targetTag;
    public bool isReachable = false;
    public string beliefTrigger = "";
    public float distance = 3.0f;


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    MOVE FUNCTIONS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void UpdateConditions(List<WorldState> preconditionsList, List<WorldState> effectsList)
    {
        if (target != null)
        {
            // Effects
            WorldState posX = new WorldState("Agent X POS", target.transform.position[0]);
            WorldState posY = new WorldState("Agent Y POS", target.transform.position[1]);
            WorldState posZ = new WorldState("Agent Z POS", target.transform.position[2]);

            effectsList.Add(posX);
            effectsList.Add(posY);
            effectsList.Add(posZ);

            this.preConditions = preconditionsList.ToArray();
            this.afterEffects = effectsList.ToArray();

            this.preconditions.Clear();
            foreach(WorldState s in preconditionsList)
            {
                this.preconditions.Add(s.key, s.value);
            }

            this.effects.Clear();
            foreach(WorldState s in effectsList)
            {
                this.effects.Add(s.key, s.value);
            }
        }
    }

    public void CalculateReachability(Vector3 originPos)
    {
        
        isReachable = false;
        cost = 9999999999;

        // Determine if a path is reachable
        NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        bool hasPath = NavMesh.CalculatePath(originPos, target.transform.position, NavMesh.AllAreas, path);
        //Debug.Log("Target: " + target.name + " | Position Agent: " + originPos + " | Target position: " + target.transform.position + " | has path: " + hasPath);

        if (hasPath)
        {
            // Compute the distance of a path
            float distance = CalculatePathDistance(path);
            float Dcost = distance;

            // If cost is lower, select this point
            if (Dcost < cost)
            {
                isReachable = true;
                cost = Dcost;
            }
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
        return isReachable;
        //return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, float> conditions)
    {
        Vector3 preconditionPos = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 effectsPos = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 conditionPos = new Vector3(0.0f, 0.0f, 0.0f);

        if (conditions.ContainsKey("Agent X POS"))
            conditionPos[0] = conditions["Agent X POS"];
        if (conditions.ContainsKey("Agent Y POS"))
            conditionPos[1] = conditions["Agent Y POS"];
        if (conditions.ContainsKey("Agent Z POS"))
            conditionPos[2] = conditions["Agent Z POS"];

        if (preconditions.ContainsKey("Agent X POS"))
            preconditionPos[0] = preconditions["Agent X POS"];
        if (preconditions.ContainsKey("Agent Y POS"))
            preconditionPos[1] = preconditions["Agent Y POS"];
        if (preconditions.ContainsKey("Agent Z POS"))
            preconditionPos[2] = preconditions["Agent Z POS"];

        CalculateReachability(conditionPos);

        // Determine if it is reachable according to reachibility
        if (isReachable)
        {

            foreach (KeyValuePair<string, float> p in preconditions)
            {
                if (!conditions.ContainsKey(p.Key))
                    return false;
            }

            //Debug.Log("Distance: " + Vector3.Distance(preconditionPos, conditionPos));
            //if (Vector3.Distance(preconditionPos, conditionPos) > 10.0f)
            //    return false;

            return true;
        }
        else {return false;}
    }

    public override bool IsFinished()
    {

        if (isReachable && running)
        {
            this.agent.SetDestination(target.transform.position);

            float targetDist = 0;
            if (target != null)
            {
                targetDist = Vector3.Distance(target.transform.position, this.transform.position);
            }

            if ((this.agent.hasPath && this.agent.remainingDistance < distance && targetDist < distance + 2.0f)||(this.beliefs.HasState(this.beliefTrigger)))
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
        if (target == null && targetTag != "")
            target = GameObject.FindWithTag(targetTag);
                
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
        return 1;
    }

}
