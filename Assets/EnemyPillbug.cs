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

    public float rollGroundCheckRadius = 0.1f;

    public LayerMask groundLayer;

    public Enemy enemy;
    public float enemyNormalSpeed;

    //za raycast da (ne "vidi" playera ako je previse ispod/iznad)
    //public float maxHeightDifference = 2f;

    void Awake()
    {
        // ovdje se pretplacujemo na event iz Enemy skripte
        //kapitalisam woooo
        enemy.OnPlayerDetected.AddListener(RollAttack);
        enemyNormalSpeed = enemy.movementSpeed;

        // nema potrebe doljeu kodu ponovno provjerava jel zid ili kraj blizu, vec to radimu u enemyu pa samo cemo se pretplatiti na evente koji ce se tamo okiniti
        enemy.OnWallDetected.AddListener(Dazed);
        enemy.OnPlatformEndDetected.AddListener(Dazed);
    }

    // Update is called once per frame
    void Update()
    {

        if (isDazed)
        {
            dazedTimer -= Time.deltaTime;

            if (dazedTimer <= 0f)
            {
                isDazed = false;
                enemy.Flip();
                enemy.movementSpeed = enemyNormalSpeed;
            }

        }

    }

    void RollAttack()
    {
        if (isDazed) return;

        isRolling = true;
        enemy.movementSpeed = rollSpeed;
    }

    void Dazed()
    {
        if (!isRolling || isDazed)
            return;

        isRolling = false;
        isDazed = true;
        dazedTimer = dazedTime;
        enemy.movementSpeed = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isRolling && !isDazed) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                if (isRolling)
                {
                    playerHealth.TakeDamage(rollDamage);
                }
                else
                {
                    playerHealth.TakeDamage(GetComponent<Enemy>().normalDamage);
                }
                Dazed();

            }
        }
    }
}