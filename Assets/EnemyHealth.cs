using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    //moze biti f za float
    public float maxHealth = 20f;
    private float currentHealth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemy took damage!");

        if (currentHealth <= 0f)
        {
            Die();
            Debug.Log("Enemy died!");

        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
