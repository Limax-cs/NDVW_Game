using UnityEngine;

public class DragonHealth : MonoBehaviour {
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;
    void Start() {
        currentHealth = maxHealth; // Initialize health
        animator = GetComponent<Animator>(); // Find the Animator component
    }


    // Call this method to deal damage to the dragon
    public void TakeDamage(int damage) {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within bounds

        if (currentHealth <= 0) {
            Die(); // Handle the dragon's death
        }
    }

    // Method to check if the dragon's health is low
    public bool IsLowHealth() {
        return currentHealth <= maxHealth * 0.3; // For example, consider low health as 30% of max health
    }

    private void Die() {
        // Implement what happens when the dragon dies
        Debug.Log("Dragon has died.");
        animator.SetTrigger("Die");
        gameObject.SetActive(false);
    }
}
