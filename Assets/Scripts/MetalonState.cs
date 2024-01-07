using UnityEngine;

public enum MetalonState
{
    Roaming,
    Attack,
    Defend,
    Flee,
    Die
}

public class MetalonController : MonoBehaviour
{
    public float roamSpeed = 2.0f;
    public float attackSpeed = 4.0f;
    public float fleeSpeed = 5.0f;
    public float roamTime = 3.0f;
    public float attackDistance = 3.0f;
    public int maxHits = 4;
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform player;
    private Animator anim;
    private int currentHits = 0;
    private MetalonState currentState = MetalonState.Roaming;
    private Animator animator;
    private Vector3 randomDestination;
    private float timer = 0.0f;
    public float roamRadius = 50.0f;
    public int maxRoamAttempts = 10;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        SetRandomDestination();
    }

    void Update()
    {
        Debug.Log("Metalon current state: " + currentState);
        switch (currentState)
        {
            case MetalonState.Roaming:
                Roam();
                break;
            case MetalonState.Attack:
                Attack();
                break;
            case MetalonState.Defend:
                Defend();
                break;
            case MetalonState.Flee:
                Flee();
                break;
            case MetalonState.Die:
                Die();
                break;
        }
    }

    void Roam()
    {
        animator.SetTrigger("Walk Forward");
        // Move towards the random destination
        transform.position = Vector3.MoveTowards(transform.position, randomDestination, roamSpeed * Time.deltaTime);

        // Make Metalon face the direction it is moving
        if (Vector3.Distance(transform.position, randomDestination) > 0.1f)
        {
            transform.LookAt(randomDestination);
        }

        // Check if reached the destination
        // if (Vector3.Distance(transform.position, randomDestination) < 1f)
        // {
        //     // Set a new random destination after a certain time
        //     timer += Time.deltaTime;
        //     if (timer >= roamTime)
        //     {
        //         SetRandomDestination();
        //         timer = 0.0f;
        //     }
        // }

        if (agent.pathPending || agent.remainingDistance > 1f)
        {
            return; // Already moving to a destination
        }

        animator.SetTrigger("Cast Spell");

        timer += Time.deltaTime;
        if (timer >= roamTime)
        {
            SetRandomDestination();
            agent.SetDestination(randomDestination);
            timer = 0.0f;
        }

        // Check if player is within attack range
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            animator.ResetTrigger("Walk Forward");
            animator.ResetTrigger("Cast Spell");   
            currentState = MetalonState.Attack;
        }
    }


    void Attack()
    {
        // animator.SetTrigger("Stab Attack");
        // // Move towards the player
        // transform.position = Vector3.MoveTowards(transform.position, player.position, attackSpeed * Time.deltaTime);
        int damage = 1;
        RaycastHit hit;
        Vector3 direction = player.position - transform.position;

        if (Physics.Raycast(transform.position, direction, out hit, attackDistance))
        {
            if (hit.transform.CompareTag("Player"))
            {
                animator.SetTrigger("Stab Attack");

                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(damage);
                }
            }
        }
        // Check if the player has resources
        // Perform stealing logic here

        // Check if player is defending
        if (currentHits == 2)
        {
            animator.ResetTrigger("Stab Attack");
            currentState = MetalonState.Defend;
        }

        // Check if Metalon should flee
        if (currentHits == maxHits-1)
        {
            animator.ResetTrigger("Stab Attack");
            currentState = MetalonState.Flee;
        }
    }

    void Defend()
    {
        animator.SetTrigger("Take Damage");
        animator.SetTrigger("Defend");
        // Check if Metalon should flee
        if (currentHits == maxHits-1)
        {
            animator.ResetTrigger("Take Damage");
            animator.ResetTrigger("Defend");
            currentState = MetalonState.Flee;
        }
    }

    void Flee()
    {
        animator.SetTrigger("Run Backward");
        // Move away from the player
        transform.position = Vector3.MoveTowards(transform.position, transform.position - (player.position - transform.position), fleeSpeed * Time.deltaTime);

        // Check if Metalon should die
        if (currentHits == maxHits)
        {
            animator.ResetTrigger("Run Backward");
            currentState = MetalonState.Die;
        } else {
            animator.ResetTrigger("Run Backward");
            currentState = MetalonState.Roaming;
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");
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

    public void ChangeState(MetalonState newState)
    {
        currentState = newState;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, randomDestination);
        // Draw ray for collision detection
        // Gizmos.DrawLine(transform.position, transform.position + (targetPosition - transform.position).normalized * collisionAvoidanceDistance);
        // Gizmos.DrawWireSphere(transform.position, collisionAvoidanceDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}


