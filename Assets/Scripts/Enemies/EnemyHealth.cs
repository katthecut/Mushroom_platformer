using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public float maxHealth = 20f;
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f) return;

        currentHealth -= damage;

        Debug.Log($"{gameObject.name} took {damage} dmg. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Debug.Log($"{gameObject.name} died!");
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}