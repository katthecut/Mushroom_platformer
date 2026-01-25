using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Rigidbody2D rb;
    public int normalDamage = 10;
    public float movementSpeed = 5.0f;

    public Transform groundCheck;
    public Transform wallCheck;
   
    public float groundCheckDistance = 0.1f;
    public float wallCheckDistance = 0.1f;

    public LayerMask groundLayer;

    //raycast
    public bool hasRaycast = false;
    public LayerMask Player;
    public float playerCheckDistance = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        Move();

        if (!Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer))
        {
            Flip();
        }

        if (Physics2D.OverlapCircle(wallCheck.position, wallCheckDistance, groundLayer))
        {
            Flip();
        }

        FindPlayer();

        if (hasRaycast) 
        { 
            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, playerCheckDistance, Player);
            
            if (hit.collider != null)
            {
                Debug.Log("Raycast radii!");
            }
        }

    }

    public void Move()
    {
        transform.Translate(Vector2.right * movementSpeed * Time.deltaTime);

    }

    public void Flip()
    {   
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;

        movementSpeed *= -1;
        
    }

    void FindPlayer()
    {
        Debug.Log("FIND PLAYER CALLED");


        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector3 rayOrigin = transform.position;// + new Vector3(0, 1f, 0);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, playerCheckDistance);

        Debug.DrawRay(rayOrigin, direction * playerCheckDistance, Color.red);
    }


    //deal dmg playeru
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(normalDamage);
                //Debug.Log("Damage dealt to player");
                
            }
        }
    }

}    