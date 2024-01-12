using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public Material defaultMaterial;
    public Material damageMaterial;
    public Renderer playerRenderer;
    public float flashDuration = 0.5f;
    private bool isDamaged = false;
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (!isDamaged)
        {
            isDamaged = true;
            playerRenderer.material = damageMaterial; // Change to damage material
            Invoke("ResetDamage", flashDuration); // Set a timer to reset the material
            // Subtract health from player here

            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
            // Update health bar UI here
        }
    }

    private void ResetDamage()
    {
        playerRenderer.material = defaultMaterial; // Revert to default material
        isDamaged = false;
    }


    void Die()
    {
        Debug.Log("Player die");
    }

}
