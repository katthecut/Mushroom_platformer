using UnityEngine;

public class EnemyPillbug : MonoBehaviour
{
    //roll attack
    public float rollSpeed = 12f;
    public int rollDamage = 15;
    public float dazedTime = 0.5f;

    //states
    private bool isRolling = false;
    private bool isDazed = false;
    private float dazedTimer;

    //colors for dazed
    public bool debugLogs = true;
    public bool tintWhileDazed = true;
    public Color dazedTint = Color.red;

    public LayerMask groundLayer;

    private Enemy enemy;
    private float enemyNormalSpeed;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        enemyNormalSpeed = enemy.movementSpeed;

        //player cant shove the pillbug around
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        //this is a trauma i shall bear forever ili sto god
        //hvala spud, smrt interpolaciji
    }

    // Update is called once per frame
    private void Update()
    {

        if (!isDazed) return;

        dazedTimer -= Time.deltaTime;

        if (dazedTimer <= 0f)
        {
            isDazed = false;

            //flip AFTER recovering
            enemy.Flip();
            enemy.movementSpeed = enemyNormalSpeed;

            if (tintWhileDazed && sr != null)
                sr.color = Color.white;
        }
    }

    //unity event
    void RollAttack()
    {
        if (isDazed) return;

        isRolling = true;
        enemy.movementSpeed = rollSpeed;
    }

    //unity event
    void Dazed()
    {
        if (!isRolling || isDazed)
            return;

        isRolling = false;
        isDazed = true;
        dazedTimer = dazedTime;
        enemy.movementSpeed = 0f;

        if (tintWhileDazed && sr != null)
            sr.color = dazedTint;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isRolling && !isDazed) return;

        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        if (isRolling)
        {
            playerHealth.TakeDamage(rollDamage);
        }
        else
        {
            playerHealth.TakeDamage(enemy.normalDamage);
        }

        Dazed();

    }
}