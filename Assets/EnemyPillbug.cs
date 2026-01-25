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

    //rollGroundCheck je tu zato sto se enemy prije okrenuo nakon RollAttack() neovisno o poziciji na platformi pa je ovo trebalo to sprijeciti, ali sad ima bug
    public Transform rollGroundCheck;
    public float rollGroundCheckRadius = 0.1f;

    public LayerMask groundLayer;

    //za raycast da (ne "vidi" playera ako je previse ispod/iznad)
    public float maxHeightDifference = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
                GetComponent<Enemy>().Flip();
                GetComponent<Enemy>().movementSpeed = 5f;
            }

        }

        if (GetComponent<Enemy>().hasRaycast && !isDazed && !isRolling)
        {
            Vector2 direction = GetComponent<Enemy>().transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(GetComponent<Enemy>().transform.position, direction, GetComponent<Enemy>().playerCheckDistance, GetComponent<Enemy>().Player);
            
            Debug.DrawRay(GetComponent<Enemy>().transform.position, direction * GetComponent<Enemy>().playerCheckDistance, Color.red);

            if (hit.collider != null)
            {
                float yDifference = Mathf.Abs(hit.collider.transform.position.y - GetComponent<Enemy>().transform.position.y);
                //provjerava razliku visine playera i enemya
                if (yDifference <= maxHeightDifference)
                {
                    RollAttack();
                }
            }
        }

        if (isRolling)
        {
            if (!Physics2D.OverlapCircle(rollGroundCheck.position, rollGroundCheckRadius, groundLayer))
            {
                Dazed();
            }
        }

    }

    void RollAttack()
    {
        if (isDazed) return;
      
        isRolling = true;
        GetComponent<Enemy>().movementSpeed = rollSpeed * (GetComponent<Enemy>().movementSpeed > 0 ? 1 : -1);
    }

    void Dazed()
    {
        isRolling = false;
        isDazed = true;
        dazedTimer = dazedTime;
        GetComponent<Enemy>().movementSpeed = 0f;
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
                    playerHealth.TakeDamage(GetComponent<Enemy>().normalDamage);
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
