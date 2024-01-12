using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrabState
{
    Idle,
    Roaming,
    Attack,
    Defend,
    Flee,
    Die
}

public class CrabController : MonoBehaviour
{
    public float roamSpeed = 2.0f;
    public float attackSpeed = 4.0f;
    public float fleeSpeed = 5.0f;
    public float roamTime = 3.0f;
    public float attackDistance = 3.0f;
    public int maxHits = 4;

    private Transform player;
    private Animator anim;
    private int currentHits = 0;
    private CrabState currentState = CrabState.Idle;
    private Animator animator;
    private Vector3 randomDestination;
    private float timer = 0.0f;

    private float idleDuration = 60f; 
    private float idleStartTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        // SetRandomDestination();
        idleStartTime = Time.time; // Record the start time of the idle state
    }

    void Update()
    {
        Debug.Log("Crab current state: "+ currentState);
        switch (currentState)
        {
            case CrabState.Idle:
                Idle();
                break;
            case CrabState.Roaming:
                Roam();
                break;
            case CrabState.Attack:
                Attack();
                break;
            case CrabState.Defend:
                Defend();
                break;
            case CrabState.Flee:
                Flee();
                break;
            case CrabState.Die:
                Die();
                break;
        }
    }

    void Idle(){
        if (Time.time - idleStartTime >= idleDuration) {
            currentState = CrabState.Roaming;
        }
        // 
    }

    void Roam()
    {
        animator.SetTrigger("walk");
        // Move towards the random destination
        transform.position = Vector3.MoveTowards(transform.position, randomDestination, roamSpeed * Time.deltaTime);

        // Make Crab face the direction it is moving
        if (Vector3.Distance(transform.position, randomDestination) > 0.1f)
        {
            transform.LookAt(randomDestination);
        }

        // Check if reached the destination
        if (Vector3.Distance(transform.position, randomDestination) < 0.1f)
        {
            // Set a new random destination after a certain time
            timer += Time.deltaTime;
            if (timer >= roamTime)
            {
                SetRandomDestination();
                timer = 0.0f;
            }
        }

        // Check if player is within attack range
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            animator.SetTrigger("fightstand");
            currentState = CrabState.Attack;
        }
    }


    void Attack()
    {
        animator.SetTrigger("attack1");
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, player.position, attackSpeed * Time.deltaTime);

        // Check if the player has resources
        // Perform stealing logic here

        // Check if player is defending
        if (currentHits == 2)
        {
            currentState = CrabState.Defend;
        }

        // Check if Crab should flee
        if (currentHits == maxHits-1)
        {
            currentState = CrabState.Flee;
        }
    }

    void Defend()
    {
        animator.SetTrigger("injured");
        animator.SetTrigger("knockout_floor");
        // Check if Crab should flee
        if (currentHits == maxHits-1)
        {
            currentState = CrabState.Flee;
        }
    }

    void Flee()
    {
        animator.SetTrigger("run");
        // Move away from the player
        transform.position = Vector3.MoveTowards(transform.position, transform.position - (player.position - transform.position), fleeSpeed * Time.deltaTime);

        // Check if Crab should die
        if (currentHits == maxHits)
        {
            currentState = CrabState.Die;
        }
    }

    void Die()
    {
        animator.SetTrigger("deadend");
        // Perform die logic here
        Destroy(gameObject);
    }

    void SetRandomDestination()
    {
        randomDestination = new Vector3(Random.Range(-125f, 125f), 0f, Random.Range(0f, 250f));
    }

    // You may need to implement other methods or conditions based on your game's requirements

    public void ChangeState(CrabState newState)
    {
        currentState = newState;
    }
}


