using UnityEngine;

public class Enemy : MonoBehaviour
{

    //RAY IZ NEKOG RAZLOGA NE RADI DOBRO 

    private Rigidbody2D rb;
    public float movementSpeed = 5.0f;

    //private bool isFacingLeft = true;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    public LayerMask groundLayer;

    //raycast
    public float playerCheckDistance = 20f; //valjda je ok 5
    public LayerMask playerLayer;
    public float maxHeightDifference = 2f;
    //maxHeightDifference da pillbug nema rolling attack dok se player krece ispod ili iznad

    //roll attack
    public float rollSpeed = 12f;
    public int normalDamage = 10;
    public int rollDamage = 15;

    //states
    private bool isRolling = false;
    private bool isDazed = false;
    public float dazedTime = 0.5f;
    private float dazedTimer;

    //sprite tba


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
                Flip();
                movementSpeed = 5f;
            }

            return;
        }

        transform.Translate(Vector2.right * movementSpeed * Time.deltaTime);

        if (!Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer))
        {
            if (isRolling)
            {
                Dazed();
            }
            else
            {
                Flip();
            }
        }

        FindPlayer();

    }

    private void Flip()
    {   // okrece ako player mijenja stranu hoda/gledanja
        
        //isFacingLeft = !isFacingLeft;
        Vector3 scale = transform.localScale;

        //ovo tu *= je za mnozenje i nema veze sa pokazivacima pls
        scale.x *= -1f;
        transform.localScale = scale;

        movementSpeed *= -1;
        
    }

    void FindPlayer()
    {
        Debug.Log("FIND PLAYER CALLED");

        if (isRolling || isDazed)
            return;

        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, playerCheckDistance, playerLayer);
        
        //ydiff radi da usporedis dvije verzije nekid stvari "side by side" 

        if (hit.collider != null)
        {
            float yDiff = Mathf.Abs(hit.collider.transform.position.y - transform.position.y);
            
            if (yDiff < maxHeightDifference)
            {
                Debug.Log("KATIA VIDIM PLAYERA");

                RollAttack();
            }
        }
    }

    void RollAttack()
    {
        isRolling = true;
        movementSpeed = rollSpeed * (movementSpeed > 0 ? 1 : -1);
    }

    void Dazed()
    {
        isRolling = false;
        isDazed = true;
        dazedTimer = dazedTime;
        movementSpeed = 0f;
    }


    //dealat dmg playeru
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                if (isRolling)
                {
                    playerHealth.TakeDamage(rollDamage);
                }
                else
                    playerHealth.TakeDamage(normalDamage);
            }
        }
    }

}    