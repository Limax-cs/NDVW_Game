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
    private float timeSinceLastHit = 0.0f;
    private float backOffTime = 3.0f; // The time in seconds after which Metalon will back off if no successful attack has occurred
    private int hitsTaken = 0;
    private int hitsToBackOff = 3; // The number of hits Metalon can take before it decides to back off


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
        timeSinceLastHit += Time.deltaTime;
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

        // Check if player is within attack range
        if (Vector3.Distance(transform.position, player.position) < roamRadius / 4)
        {
            animator.ResetTrigger("Walk Forward");
            animator.ResetTrigger("Cast Spell");
            currentState = MetalonState.Attack;
        }

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

        
    }


    private enum AttackSubState
    {
        Approaching,
        BackingOff,
        PreparingToAttack
    }

    private AttackSubState attackSubState = AttackSubState.Approaching;
    public float backOffDistance = 1.5f; // The distance Metalon will back off after approaching
    public float prepareAttackTime = 1.0f; // Time Metalon spends in PreparingToAttack state
    private float attackTimer = 0.0f;

    void Attack()
    {

        switch (attackSubState)
        {
            case AttackSubState.Approaching:
                animator.ResetTrigger("Stab Attack");
                Debug.Log("AttackSubState.Approaching");
                animator.SetTrigger("Walk Forward");

                animator.SetTrigger("Stab Attack");
                // animator.ResetTrigger("Stab Attack");
                // Determine the direction from the object to the player
                Vector3 directionToPlayer = (player.position - transform.position).normalized;

                // Set a desired distance from the player
                float distanceFromPlayer = 4.0f; // Adjust this value as needed

                float distanceToPlayer = Vector3.Distance(transform.position, player.position);


                Vector3 playerPosition = new Vector3(player.position.x, player.position.y - 0.58f, player.position.z);


                // Calculate the new target position in front of the player
                Vector3 targetPosition = playerPosition - directionToPlayer * distanceFromPlayer;

                // Move towards the target position
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, attackSpeed * Time.deltaTime);

                // Check for attack range using raycast
                // RaycastHit hit;
                // Vector3 direction = player.position - transform.position;
                // Debug.DrawRay(transform.position, direction * attackDistance, Color.red);

                // Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackDistance);
                bool playerHit = false;

                // animator.SetTrigger("Stab Attack");
                // attackTimer -= Time.deltaTime;
                // animator.ResetTrigger("Stab Attack");

                // foreach (var hitCollider in hitColliders)
                // {
                //     if (hitCollider.CompareTag("Player"))
                //     {
                //         animator.SetTrigger("Smash Attack");
                //         playerHit = true;
                //         // The player is hit, apply damage
                //         PlayerController playerController = hitCollider.GetComponent<PlayerController>();
                //         if (playerController != null)
                //         {
                //             Debug.Log("Damaging player");
                //             playerController.TakeDamage(1);
                //             // After dealing damage, Metalon backs off
                //             animator.ResetTrigger("Smash Attack");
                //             attackSubState = AttackSubState.BackingOff;
                //         }
                //         break; // Exit the loop as we've found our target
                //     }
                // } 

                // Check if the player is within attack range
                if (distanceToPlayer <= 3.01f)
                {
                    animator.ResetTrigger("Walk Forward");
                    animator.SetBool("Defend", true);
                    // The player is within range, trigger the attack
                    animator.SetTrigger("Smash Attack");

                    // Check if the attack animation is playing
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Smash Attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                    {
                        // Animation has finished, transition to next state
                        
                        // The player is hit, apply damage
                        PlayerController playerController = player.GetComponent<PlayerController>();
                        if (playerController != null)
                        {
                            Debug.Log("Damaging player");
                            playerController.TakeDamage(1);
                            // After dealing damage, back off
                            animator.ResetTrigger("Smash Attack");
                            attackSubState = AttackSubState.BackingOff;
                        }
                    }

                    
                }
                else
                {
                    // Player is not in range, reset the attack trigger
                    animator.ResetTrigger("Smash Attack");
                }

                // If the player was hit, reset the attack
                if (playerHit)
                {
                    timeSinceLastHit = 0.0f;
                    hitsTaken = 0;
                    animator.SetTrigger("Smash Attack");
                }
                else
                {
                    // If the player wasn't hit and the conditions are not met to back off, loop the smash attack
                    if (timeSinceLastHit < backOffTime && hitsTaken < hitsToBackOff)
                    {
                        animator.ResetTrigger("Smash Attack");
                        animator.SetTrigger("Smash Attack");
                    }
                    else
                    {
                        // Conditions are met to back off
                        animator.ResetTrigger("Smash Attack");
                        attackSubState = AttackSubState.BackingOff;
                    }
                }
                break;

            case AttackSubState.BackingOff:
                Debug.Log("AttackSubState.BackingOff");
                // Back off a little bit
                Vector3 backOffDirection = (transform.position - player.position).normalized * backOffDistance;
                Vector3 backOffPosition = player.position + backOffDirection;

                animator.SetTrigger("Walk Backward");
                transform.position = Vector3.MoveTowards(transform.position, backOffPosition, roamSpeed * Time.deltaTime);

                // Transition to preparing to attack after backing off a bit
                if (Vector3.Distance(transform.position, backOffPosition) < 0.1f)
                {
                    animator.ResetTrigger("Walk Backward");
                    attackSubState = AttackSubState.PreparingToAttack;
                    attackTimer = prepareAttackTime;
                }
                break;

            case AttackSubState.PreparingToAttack:
                Debug.Log("AttackSubState.Preparing ToAttack");
                // Wait for a moment before attacking again
                animator.SetTrigger("Stab Attack");
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0.0f)
                {
                    animator.ResetTrigger("Stab Attack");
                    attackSubState = AttackSubState.Approaching;
                }
                break;
        }

        // Check if player is defending
        if (currentHits == 2)
        {
            currentState = MetalonState.Defend;
        }

        // Check if Metalon should flee
        if (currentHits == maxHits - 1)
        {
            currentState = MetalonState.Flee;
        }

        // Face the player at all times
        Vector3 lookAtPlayer = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookAtPlayer);
    }

    public void ChangeState(MetalonState newState)
    {
        // Reset attack sub-state if leaving the attack state
        if (currentState == MetalonState.Attack && newState != MetalonState.Attack)
        {
            attackSubState = AttackSubState.Approaching;
        }

        currentState = newState;
    }

    public void OnHitByPlayer()
    {
        // This method should be called by the player's attack logic when Metalon is hit
        hitsTaken++;
        // Optionally, you may want to interrupt the attack if Metalon is hit
        animator.ResetTrigger("Smash Attack");
    }

    void Defend()
    {
        animator.SetTrigger("Take Damage");
        animator.SetTrigger("Defend");
        // Check if Metalon should flee
        if (currentHits == maxHits - 1)
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


