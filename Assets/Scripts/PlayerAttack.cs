using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;

    void Update()
    {
        // Check for player input to initiate an attack
        if (Input.GetKeyDown(KeyCode.F))
        {
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        // Assuming a melee attack, you might use a raycast or an overlap sphere to detect hits
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Metalon"))
            {
                // Assuming MetalonController script is attached to the enemy GameObject
                MetalonController metalon = enemy.GetComponent<MetalonController>();
                if (metalon != null)
                {
                    // Call the OnHitByPlayer method on Metalon
                    metalon.OnHitByPlayer();
                }
            }
        }
    }

    // Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
