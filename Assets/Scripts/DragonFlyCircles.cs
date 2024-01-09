using UnityEngine;

public class DragonFlyCircles : MonoBehaviour
{
    private Animator anim;
    int Hover;

    // Circular motion parameters
    public float radius = 5f; // Radius of the circle
    public float speed = 2f; // Speed of the circular motion
    public float waveAmplitude = 1f; // Amplitude of the waving motion
    public float waveFrequency = 1f; // Frequency of the waving motion
    public float rotationSpeed = 1f;

    // Initial position
    private Vector3 initialPosition;

    void Start()
    {
        anim = GetComponent<Animator>();
        Hover= Animator.StringToHash("Hover");

        // Store the initial position
        initialPosition = transform.position;
    }

    void Update()
    {
        // Update the angle based on time and speed
        float angle = speed * Time.time;

        // Calculate the new position using polar coordinates
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Calculate the waving motion for the y component
        float yWave = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;

         // Set the new position for the dragon with waving motion
        transform.position = initialPosition + new Vector3(x, yWave, z);

        // Calculate the target position based on the movement direction
        Vector3 targetPosition = initialPosition + new Vector3(-x, 0f, -z);

        // Calculate the rotation angle based on the movement direction
        float rotationAngle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;

        // Set the rotation of the dragon's head
        Quaternion targetRotation = Quaternion.Euler(0f, rotationAngle, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Set the new position for the dragon
        transform.position = initialPosition + new Vector3(x, 0f, z);

        // Trigger the FlyingFWD animation
        anim.SetTrigger(Hover);
    }
}
