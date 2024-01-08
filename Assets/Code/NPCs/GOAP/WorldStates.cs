using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Definition of the world state
[System.Serializable]
public class WorldState
{
    public string key;
    public int value;
}

// Generate the dictionary of states
public class WorldStates
{
    public Dictionary<string, int> states;

    public WorldStates()
    {
        states = new Dictionary<string, int>();
    }

    // Obtain state value
    public bool HasState(string key)
    {
        return states.ContainsKey(key);
    }

    // Add state to the dictionary
    void AddState(string key, int value)
    {
        states.Add(key, value);
    }

    // Modify State
    public void ModifyState(string key, int value)
    {
        if(states.ContainsKey(key))
        {
            states[key] += value;
            if (states[key] <= 0)
                RemoveState(key);
        }
        else
            states.Add(key, value);
    }

    // Remove State
    public void RemoveState(string key)
    {
        if (states.ContainsKey(key))
            states.Remove(key);
    }

    // Set State
    public void SetState(string key, int value)
    {
        if (states.ContainsKey(key))
            states[key] = value;
        else
            states.Add(key, value);
    }

    // Return States
    public Dictionary<string, int> GetStates()
    {
        return states;
    }

}
