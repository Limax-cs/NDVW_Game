using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropX : GAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }

    public override bool IsFinished()
    {
        return true;
    }

    public override void Perform()
    {
        
    }

    public override bool IsAchievable()
    {
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, float> conditions)
    {
        return true;
    }

}
