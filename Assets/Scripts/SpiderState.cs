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
    // Check if the animation is already playing
    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
    {
        // If not, then set the Walk animation to play
        animator.SetBool("IsWalking", true);
    }

    // Move towards the random destination
    transform.position = Vector3.MoveTowards(transform.position, randomDestination, roamSpeed * Time.deltaTime);

    // Make Spider face the direction it is moving
    if (Vector3.Distance(transform.position, randomDestination) > 0.1f)
    {
        transform.LookAt(new Vector3(randomDestination.x, transform.position.y, randomDestination.z));
    }

    // Check if reached the destination
    if (Vector3.Distance(transform.position, randomDestination) < 0.1f)
    {
        // Stop the walking animation
        animator.SetBool("IsWalking", false);
        
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
        // Stop the walking animation
        animator.SetBool("IsWalking", false);

        // Face the player at all times
        Vector3 lookAtPlayer = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookAtPlayer);

        currentState = SpiderState.Attack;
    }
}


    void Attack()
    {
        animator.SetBool("IsStabbing", true);
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, player.position, attackSpeed * Time.deltaTime);

        animator.SetBool("IsStabbing", false);
        currentState = SpiderState.Flee;

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

        // Face the player at all times
        Vector3 lookAtPlayer = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookAtPlayer);
    }

    void Defend()
    {
        animator.SetTrigger("TakeDamage");
        // Check if Spider should flee
        if (currentHits == maxHits-1)
        {
            animator.ResetTrigger("TakeDamage");
            currentState = SpiderState.Flee;
        }
    }

    // Define a safe distance for roaming
    public float safeDistance = 10.0f;

    void Flee()
    {
        animator.SetBool("IsWalkingBack", true);
        // Move away from the player
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 newFleePosition = transform.position + fleeDirection * fleeSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, newFleePosition, fleeSpeed * Time.deltaTime);

        // Turn the spider to face away from the player as it flees
        transform.LookAt(newFleePosition);

        // Check if Spider should die
        if (currentHits == maxHits)
        {
            animator.SetBool("IsWalkingBack", false);
            currentState = SpiderState.Die;
        }
        // Check if the spider is at a safe distance to go back to roaming
        else if (Vector3.Distance(transform.position, player.position) > safeDistance)
        {
            animator.SetBool("IsWalkingBack", false);
            currentState = SpiderState.Roaming;
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

    public void ChangeState(SpiderState newState)
    {
        currentState = newState;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Spider Hit");
        Debug.Log("Collider Tag: " + collision.gameObject.tag);
        //Debug.Log("Spider Hit");
        if (collision.gameObject.tag == "damage")
        {
            //WeaponItem weaponItem = collision.collider.GetComponent<WeaponItem>();
            //moleParams.HP = Mathf.Max(moleParams.HP - weaponItem.weaponDescrib.attack, 0.0f);
            //Debug.Log("Mole Hit");
            Debug.Log("Spider Hit");
            currentHits++;
        }
    }
}


