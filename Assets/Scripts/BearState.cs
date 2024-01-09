using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BearState
{
    Idle,
    Roaming,
    Attack,
    Defend,
    Flee,
    Sleep,
    Eat,
    Die
}

public class BearController : MonoBehaviour
{
    public float roamSpeed = 2.0f;
    public float attackSpeed = 4.0f;
    public float fleeSpeed = 5.0f;
    public float stuckTime = 10.0f;
    public float attackDistance = 3.0f;
    public int maxHits = 4;

    private Transform player;
    private Animator anim;
    private int currentHits = 0;
    private BearState currentState = BearState.Roaming;
    private Animator animator;
    private Vector3 randomDestination;
    private float timer = 0.0f;
    public float roamRadius = 50.0f;
    public int maxRoamAttempts = 10;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        SetRandomDestination();
    }

    void Update()
    {
        Debug.Log("Bear current state: " + currentState);
        switch (currentState)
        {
            case BearState.Idle:
                Idle();
                break;
            case BearState.Roaming:
                Roam();
                break;
            case BearState.Attack:
                Attack();
                break;
            case BearState.Defend:
                Defend();
                break;
            case BearState.Flee:
                Flee();
                break;
            case BearState.Sleep:
                Sleep();
                break;
            case BearState.Eat:
                Eat();
                break;
            case BearState.Die:
                Die();
                break;
        }
    }

    void Idle()
    {
        animator.SetBool("Idle", true);
        timer += Time.deltaTime;
        if (timer >= stuckTime)
        {
            animator.SetBool("Idle", false);
            currentState = BearState.Roaming;
            timer = 0.0f;
        }

    }

    void Roam()
    {

        animator.SetBool("WalkForward", true);

        // Move towards the random destination
        transform.position = Vector3.MoveTowards(transform.position, randomDestination, roamSpeed * Time.deltaTime);

        // Make Bear face the direction it is moving
        if (Vector3.Distance(transform.position, randomDestination) > 0.5f)
        {
            transform.LookAt(new Vector3(randomDestination.x, transform.position.y, randomDestination.z));
        }

        // Check if reached the destination
        if (Vector3.Distance(transform.position, randomDestination) < 0.5f)
        {
            // Stop the walking animation
            animator.SetBool("WalkForward", false);
            SetRandomDestination();
            timer = 0.0f;
            
            // Set a new random destination after a certain time
            // timer += Time.deltaTime;
            // if (timer >= roamTime)
            // {
            //     SetRandomDestination();
            //     timer = 0.0f;
            // }
        } else if(timer >= stuckTime ){
            timer += Time.deltaTime;
            SetRandomDestination();
            timer = 0.0f;

        }

        // Check if player is within attack range
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            // Stop the walking animation
            animator.SetBool("WalkForward", false);

            currentState = BearState.Attack;
        }
    }


    void Attack()
    {
        animator.SetBool("Attack1", true);
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, player.position, attackSpeed * Time.deltaTime);

        // Check if player is defending
        if (currentHits == 2)
        {
            currentState = BearState.Defend;
            animator.SetBool("Attack1", false); // Ensure to reset the attack animation when changing state.
        }

        // Check if Bear should flee
        if (currentHits == maxHits-1)
        {
            currentState = BearState.Flee;
            animator.SetBool("Attack1", false); // Ensure to reset the attack animation when changing state.
        }

        // Check if player is fleeing
        if (PlayerIsFleeing())
        {
            animator.SetTrigger("Buff");
            currentState = BearState.Idle;
            animator.SetBool("Attack1", false);
            animator.ResetTrigger("Buff");
        }

        // Face the player at all times
        Vector3 lookAtPlayer = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookAtPlayer);
    }

    // You need to implement this method based on how your game detects the player fleeing.
    bool PlayerIsFleeing()
    {
        // Determine the logic for when a player is considered to be fleeing.
        // This could be based on distance, player's speed, direction of movement, etc.
        // For example:
        float fleeDistance = 3.0f;
        return Vector3.Distance(transform.position, player.position) > fleeDistance;

    }


    void Defend()
    {
        animator.SetTrigger("Hit Front");
        animator.SetBool("Stunned Loop", true);
        // Check if Bear should flee
        if (currentHits == maxHits-1)
        {
            animator.ResetTrigger("Hit Front");
            animator.SetBool("Stunned Loop", false);
            currentState = BearState.Flee;
        }
    }

    // Define a safe distance for roaming
    public float safeDistance = 10.0f;

    void Flee()
    {
        animator.SetBool("Run Backward", true);
        // Move away from the player
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 newFleePosition = transform.position + fleeDirection * fleeSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, newFleePosition, fleeSpeed * Time.deltaTime);

        // Turn the Bear to face away from the player as it flees
        transform.LookAt(newFleePosition);

        // Check if Bear should die
        if (currentHits == maxHits)
        {
            animator.SetBool("Run Backward", false);
            currentState = BearState.Die;
        }
        // Check if the Bear is at a safe distance to go back to roaming
        else if (Vector3.Distance(transform.position, player.position) > safeDistance)
        {
            animator.SetBool("Run Backward", false);
            currentState = BearState.Roaming;
            SetRandomDestination(); // Set a new random roaming destination
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
        bool validDestinationFound = false;
        int attempts = 0;

        while (!validDestinationFound && attempts < maxRoamAttempts)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection += transform.position;
            UnityEngine.AI.NavMeshHit hit;

            if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1))
            {
                randomDestination = hit.position;
                validDestinationFound = true;
            }
            else
            {
                // Increment the attempt counter
                attempts++;
            }
        }

        if (!validDestinationFound)
        {
            Debug.LogWarning("Failed to find a valid random destination after " + maxRoamAttempts + " attempts.");
        }
    }

    // You may need to implement other methods or conditions based on your game's requirements

    public void ChangeState(BearState newState)
    {
        currentState = newState;
    }

    void Sleep()
    {
        float sleepTime = 10.0f;
        animator.SetBool("Sleep", true);
        timer += Time.deltaTime;
        if (timer >= sleepTime)
        {
            animator.SetBool("Sleep", false);
            currentState = BearState.Idle;
            timer = 0.0f;
        }

    }

    void Eat()
    {
        float sleepTime = 6.0f;
        animator.SetBool("Eat", true);
        timer += Time.deltaTime;
        if (timer >= sleepTime)
        {
            animator.SetBool("Eat", false);
            currentState = BearState.Idle;
            timer = 0.0f;
        }

    }
}


