using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour {

    NavMeshAgent agent;
    Animator anim;
    DragonState currentState;

    public Transform player;

    void Start() {

        // agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = new DragonIdle(gameObject, anim, player);
        Debug.Log("Start State: "+ currentState);
    }


    void Update() {

        currentState = currentState.Process();
        Debug.Log("Update State: "+ currentState);

        if (currentState == null)
        {
            Debug.LogError("There is no current state!");
            return;
        }
    }
}
