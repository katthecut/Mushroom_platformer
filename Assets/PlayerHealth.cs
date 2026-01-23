using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private float health;
    public float Health { get { return health; } set { health = value; healthBar.SetHealth(value); } }
    public float MaxHealth;

    [SerializeField]
    private HealthBarSystem healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Health = MaxHealth;
        healthBar.SetMaxHealth(MaxHealth);
        healthBar.SetHealth(Health);

    }

    // Update is called once per frame
    void Update()
    {

        /*if (Input.GetKeyDown("g")) {
            SetHealth(-20f);
        }
        if (Input.GetKeyDown("h")) {
            SetHealth(20f);
        } 
        */

    }

    public void SetHealth(float healthChange)
    {
        Health = healthChange;
    }

    public void AddHealth(float healthChange)
    {
        Health += healthChange;

        if (Health + healthChange < 0f)
        {
            Health = 0f;
        }
        else if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        AddHealth(-damage);

        if (Health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player dead");
    }

    
}
