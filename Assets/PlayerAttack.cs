using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    private float timeBetweenAttack;
    public float startTimeBetweenAttack;

    public Transform attackPosition;
    public float attackRange;

    public LayerMask whatIsEnemies;

    public int damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeBetweenAttack > 0)
        {
            timeBetweenAttack -= Time.deltaTime;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (timeBetweenAttack > 0) return;

        timeBetweenAttack = startTimeBetweenAttack;

        
         //nevidljivi krug na nekoj pozosoji i unutar njega igrac deala dmg
         Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, whatIsEnemies);
         
        
        for (int i = 0; i < enemiesToDamage.Length; i++)
         {
         enemiesToDamage[i].GetComponent<EnemyHealth>().TakeDamage(damage);
         }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPosition == null) return;

        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }

}