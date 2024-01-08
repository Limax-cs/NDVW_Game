using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Definition of the world state
[System.Serializable]
public class WorldState
{
    public string key;
    public float value;

    public WorldState()
    {

    }

    public WorldState(string key, float value)
    {
        this.key = key;
        this.value = value;
    }
}

// Generate the dictionary of states
[System.Serializable]
public class WorldStates
{
    public Dictionary<string, float> states;

    public WorldStates()
    {
        states = new Dictionary<string, float>();
    }

    // Obtain state value
    public bool HasState(string key)
    {
        return states.ContainsKey(key);
    }

    // Add state to the dictionary
    void AddState(string key, float value)
    {
        states.Add(key, value);
    }

    // Modify State
    public void ModifyState(string key, float value)
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
    public void SetState(string key, float value)
    {
        if (states.ContainsKey(key))
            states[key] = value;
        else
            states.Add(key, value);
    }

    // Return States
    public Dictionary<string, float> GetStates()
    {
        return states;
    }

}
