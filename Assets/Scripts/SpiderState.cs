using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpiderState
{
    Roaming,
    Attack,
    Defend,
    Flee,
    Die
}

public class SpiderController : MonoBehaviour
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
    private SpiderState currentState = SpiderState.Roaming;
    private Animator animator;
    private Vector3 randomDestination;
    private float timer = 0.0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        SetRandomDestination();
    }

    void Update()
    {
        Debug.Log("Spider current state: "+ currentState);
        switch (currentState)
        {
            case SpiderState.Roaming:
                Roam();
                break;
            case SpiderState.Attack:
                Attack();
                break;
            case SpiderState.Defend:
                Defend();
                break;
            case SpiderState.Flee:
                Flee();
                break;
            case SpiderState.Die:
                Die();
                break;
        }
    }

    void Roam()
    {
        animator.SetTrigger("Walk");
        // Move towards the random destination
        transform.position = Vector3.MoveTowards(transform.position, randomDestination, roamSpeed * Time.deltaTime);

        // Make Spider face the direction it is moving
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
            currentState = SpiderState.Attack;
        }
    }


    void Attack()
    {
        animator.SetTrigger("Attack1");
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, player.position, attackSpeed * Time.deltaTime);

        // Check if the player has resources
        // Perform stealing logic here

        // Check if player is defending
        if (currentHits == 2)
        {
            currentState = SpiderState.Defend;
        }

        // Check if Spider should flee
        if (currentHits == maxHits-1)
        {
            currentState = SpiderState.Flee;
        }
    }

    void Defend()
    {
        animator.SetTrigger("TakeDamage");
        // Check if Spider should flee
        if (currentHits == maxHits-1)
        {
            currentState = SpiderState.Flee;
        }
    }

    void Flee()
    {
        animator.SetTrigger("WalkBack");
        // Move away from the player
        transform.position = Vector3.MoveTowards(transform.position, transform.position - (player.position - transform.position), fleeSpeed * Time.deltaTime);

        // Check if Spider should die
        if (currentHits == maxHits)
        {
            currentState = SpiderState.Die;
        }
    }

    void Die()
    {
        animator.SetTrigger("Death");
        // Perform die logic here
        Destroy(gameObject);
    }

    void SetRandomDestination()
    {
        randomDestination = new Vector3(Random.Range(-125f, 125f), 10f, Random.Range(0f, 250f));
    }

    // You may need to implement other methods or conditions based on your game's requirements

    public void ChangeState(SpiderState newState)
    {
        currentState = newState;
    }
}


