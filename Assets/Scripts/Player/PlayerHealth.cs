using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerHealth : MonoBehaviour
{
    //health
    private float health;
    [SerializeField] private float maxHealth = 100f;

    //ui
    private HealthBarSystem healthBar;

    //game over
    [SerializeField] private GameOverManager gameOverUI;


    [SerializeField] private bool receiveContactDamage = true;
    //cooldown za primanje dmg-a
    [SerializeField] private float contactDamageCooldown = 0.25f;

    private float nextContactDamageTime;

    private PlayerAudio playerAudio;
    private bool isDead;

    public float Health => health;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        if (healthBar == null)
            healthBar = GetComponentInChildren<HealthBarSystem>(true);

        if (gameOverUI == null)
            gameOverUI = Object.FindFirstObjectByType<GameOverManager>(FindObjectsInactive.Include);

        playerAudio = GetComponent<PlayerAudio>();

        health = maxHealth;
        SyncUI();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = MaxHealth;
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

    private void SyncUI()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(health);
        }
        else
        {
            Debug.LogWarning("PlayerHealth: HealthBarSystem missing.");
        }
    }

    public void SetHealth(float value)
    {
        if (isDead) return;

        health = Mathf.Clamp(value, 0f, maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(health);

        if (health <= 0f)
            Die();
    }

    public void AddHealth(float amount)
    {
        SetHealth(health + amount);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (damage <= 0f) return;

        playerAudio?.PlayHit();
        AddHealth(-damage);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (gameOverUI != null)
            gameOverUI.ShowGameOver();
        else
            Debug.LogWarning("PlayerHealth: GameOverUI not found/assigned.");
    }

    // contact damage

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!receiveContactDamage) return;
        TryTakeContactDamage(other);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!receiveContactDamage) return;
        if (collision == null) return;
        TryTakeContactDamage(collision.collider);
    }

    private void TryTakeContactDamage(Collider2D other)
    {
        if (isDead) return;
        if (other == null) return;
        if (Time.time < nextContactDamageTime) return;

        //trazi DamageToPlayer
        DamageToPlayer dmg = other.GetComponent<DamageToPlayer>();
        if (dmg == null) dmg = other.GetComponentInParent<DamageToPlayer>();
        if (dmg == null) return;

        //provjera za primjeniti dmg
        if (!dmg.IsDamageCollider(other))
            return;

        TakeDamage(dmg.damagePerTick);
        nextContactDamageTime = Time.time + Mathf.Max(0.01f, contactDamageCooldown);
    }
}
