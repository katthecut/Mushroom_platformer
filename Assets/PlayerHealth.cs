using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public float Health, MaxHealth;

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

    public void SetHealth(float healthChange) {
        Health += healthChange;
        //clamp napravi da health ne ide ispod 0 npr '10 - 15' daje 0
        Health = Mathf.Clamp(Health, 0f, MaxHealth);

        healthBar.SetHealth(Health);
    }

    public void TakeDamage(float damage)
    {
        SetHealth(-damage);

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
