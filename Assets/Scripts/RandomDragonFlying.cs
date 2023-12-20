using UnityEngine;

public class RandomDragonFlying : MonoBehaviour
{
    private Animator anim;
    public float flyingSpeed = 5f;
    public float rotationSpeed = 2f;
    public float changeDirectionInterval = 5f;

    private float timeSinceLastDirectionChange;
    private Vector3 targetPosition;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Set an initial random target position
        SetRandomTargetPosition();
    }

    void Update()
    {
        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, flyingSpeed * Time.deltaTime);

        // Rotate towards the target position
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check if it's time to change direction
        timeSinceLastDirectionChange += Time.deltaTime;
        if (timeSinceLastDirectionChange >= changeDirectionInterval)
        {
            // Set a new random target position
            SetRandomTargetPosition();
        }

        // Trigger the Flying animation
        anim.SetTrigger("FlyingFWD");
    }

    void SetRandomTargetPosition()
    {
        // Generate a random position within a certain range
        float randomX = Random.Range(-125f, 0f);
        float randomZ = Random.Range(0f, 200f);

        // Set the new target position
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);

        // Reset the time since the last direction change
        timeSinceLastDirectionChange = 0f;
    }
}
