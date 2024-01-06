using UnityEngine;

public enum DragonState
{
    Roaming,
    Attack,
    Defend,
    Flee,
    Die
}

public class DragonController : MonoBehaviour
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
    private DragonState currentState = DragonState.Roaming;
    private Animator animator;
    private Vector3 randomDestination;
    private float timer = 0.0f;

    float visDist = 10.0f;
    float visAngle = 30.0f;
    float shootDist = 7.0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        SetRandomDestination();
    }

    void Update()
    {
        Debug.Log("Dragon current state: "+ currentState);
        switch (currentState)
        {
            case DragonState.Roaming:
                Roam();
                break;
            case DragonState.Attack:
                Attack();
                break;
            case DragonState.Defend:
                Defend();
                break;
            case DragonState.Flee:
                Flee();
                break;
            case DragonState.Die:
                Die();
                break;
        }
    }

    void Roam()
    {
        animator.SetTrigger("Walk Forward");
        // Move towards the random destination
        transform.position = Vector3.MoveTowards(transform.position, randomDestination, roamSpeed * Time.deltaTime);

        // Make Dragon face the direction it is moving
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
            currentState = DragonState.Attack;
        }
    }


    void Attack()
    {
        animator.SetTrigger("Stab Attack");
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, player.position, attackSpeed * Time.deltaTime);

        // Check if the player has resources
        // Perform stealing logic here

        // Check if player is defending
        if (currentHits == 2)
        {
            currentState = DragonState.Defend;
        }

        // Check if Dragon should flee
        if (currentHits == maxHits-1)
        {
            currentState = DragonState.Flee;
        }
    }

    void Defend()
    {
        animator.SetTrigger("Take Damage");
        animator.SetTrigger("Defend");
        // Check if Dragon should flee
        if (currentHits == maxHits-1)
        {
            currentState = DragonState.Flee;
        }
    }

    void Flee()
    {
        animator.SetTrigger("Run Backward");
        // Move away from the player
        transform.position = Vector3.MoveTowards(transform.position, transform.position - (player.position - transform.position), fleeSpeed * Time.deltaTime);

        // Check if Dragon should die
        if (currentHits == maxHits)
        {
            currentState = DragonState.Die;
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
        randomDestination = new Vector3(Random.Range(-125f, 0f), 0f, Random.Range(0f, 10f));
    }

    // You may need to implement other methods or conditions based on your game's requirements

    public void ChangeState(DragonState newState)
    {
        currentState = newState;
    }
}


