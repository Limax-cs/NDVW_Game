using UnityEngine;

public class RandomDragonFlying : MonoBehaviour
{
    private Animator anim;
    public float flyingSpeed = 5f;
    public float rotationSpeed = 2f;
    public Vector2 boundaryX = new Vector2(-125f, 125f);
    public Vector2 boundaryY = new Vector2(10f, 50f);
    public Vector2 boundaryZ = new Vector2(-125f, 125f);
    public float collisionAvoidanceDistance = 30f; // Distance to check for collision

    private Vector3 targetPosition;
    private bool isFlying = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        SetRandomTargetPosition();
    }

    void Update()
    {
        if (!isFlying)
        {
            anim.SetTrigger("FlyingFWD");
            isFlying = true;
        }

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, flyingSpeed * Time.deltaTime);

        // Rotate towards the target position
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // If the dragon reaches the target position or an obstacle is detected, set a new target position
        if (Vector3.Distance(transform.position, targetPosition) < 1.0f || IsObstacleInPath())
        {
            SetRandomTargetPosition();
        }
    }

    void SetRandomTargetPosition()
    {
        // Generate a random position within specified boundaries
        float randomX = Random.Range(boundaryX.x, boundaryX.y);
        float randomY = Random.Range(boundaryY.x, boundaryY.y);
        float randomZ = Random.Range(boundaryZ.x, boundaryZ.y);

        // Set the new target position with a completely new Y value
        targetPosition = new Vector3(randomX, randomY, randomZ);
    }

    bool IsObstacleInPath()
    {
        RaycastHit hit;
        // Raycast forward from the current position to the target position
        if (Physics.Raycast(transform.position, targetPosition - transform.position, out hit, collisionAvoidanceDistance))
        {
            // If an obstacle is hit, return true
            return true;
        }
        // No obstacle detected, return false
        return false;
    }

    // Optional: A visualization for debugging purposes
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(
            (boundaryX.x + boundaryX.y) / 2, 
            (boundaryY.x + boundaryY.y) / 2, 
            (boundaryZ.x + boundaryZ.y) / 2), 
            new Vector3(
                boundaryX.y - boundaryX.x, 
                boundaryY.y - boundaryY.x, 
                boundaryZ.y - boundaryZ.x));
        Gizmos.DrawLine(transform.position, targetPosition);
        // Draw ray for collision detection
        Gizmos.DrawLine(transform.position, transform.position + (targetPosition - transform.position).normalized * collisionAvoidanceDistance);
    }
}
