using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GWorld
{
    private static readonly GWorld instance = new GWorld();
    private static WorldStates world;

    static GWorld()
    {
        world = new  WorldStates();
    }

    private GWorld()
    {

    }

    // Get GWorld instance
    public static GWorld Instance
    {
        get { return instance; }
    }

    // Get World States
    public WorldStates GetWorld()
    {
        return world;
    }

}
