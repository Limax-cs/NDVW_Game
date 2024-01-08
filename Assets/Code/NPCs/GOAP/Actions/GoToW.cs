using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToW : GAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        GWorld.Instance.GetWorld().ModifyState("Waiting", 1);
        return true;
    }
}
