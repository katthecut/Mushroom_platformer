using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;    
    private bool isFacingRight = true;

    private float originalGravity;

    public float moveSpeed = 15f;
    float horizontalMove;

    public float jumpPower = 25f;
    public int maxJumps = 2;
    private int jumpCount;

    //crkni

    private bool isDashing;
    private bool canDash = true;

    [SerializeField] private float dashPower = 20f;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private float dashCooldown = 1f;

    private float dashTimer;
    private float dashCooldownTimer;


    public ParticleSystem jumpParticles;
    [SerializeField] private TrailRenderer trail;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 8f;
    }


    // Update is called once per frame

    void Update()
    {
        //bog zna zasto si ti tu, a to nisam ja
        //fr gdje su ostali pls
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.gravityScale = originalGravity;

                if (trail != null)
                {
                    trail.emitting = false;
                }

            }

        }

        //dash cooldown

        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            
            if (dashCooldownTimer <= 0f)
            {
                canDash = true;
            }

        }

    }

    //razlika fixed i normal update jednom

    void FixedUpdate()
    {
        if (isDashing)
            return;

        rb.linearVelocity = new Vector2(horizontalMove * moveSpeed, rb.linearVelocity.y);

        if (horizontalMove != 0)
            Flip();

    }


    public void Move(InputAction.CallbackContext context)
    {
        horizontalMove = context.ReadValue<Vector2>().x;
    }


    public void Jump(InputAction.CallbackContext context)
    {

        if (context.performed && jumpCount < maxJumps)
        {
            // reset vertikalne brzine
            //f ovdje mora biti veci od 0 da vise skace ne mijenjaj si to prog je vradzbina
            // note to self: ako je manje od 5 onda ne moze jednim skokom skociti na najnizu platformu
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5f);
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            
            //jump particles null da provjeri za slucaj da particle system nestane
            if (jumpParticles != null)
            {
                jumpParticles.Play();

            }
                jumpCount++;

        }

    }
    
    //i ti crkni
    public void Dash(InputAction.CallbackContext context)
    {   
        if (!context.performed)
        {
            return;
        }

        if (!canDash || isDashing)
        {
            return;
        }


        canDash = false;
        isDashing = true;
        
        dashTimer = dashTime;
        dashCooldownTimer = dashCooldown;

        //mijenjamo gravotaciju tako da ne padamo dok dashamo
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2((isFacingRight ? 1f : -1f) * dashPower, 0f);


        if (trail != null) 
        {
            trail.emitting = true;
        }  

    }

    private void Flip()
    {
        // okrece ako player mijenja stranu hoda/gledanja
        if ((isFacingRight && horizontalMove < 0) || (!isFacingRight && horizontalMove > 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            //ovo tu *= je za mnozenje i nema veze sa pokazivacima pls
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // kad dotakne tlo opet

        //tjedan dana kasnije: molim??

        // dva i pol tjedna kasnije ghuiashgiasjgiajaso
        if (collision.contacts[0].normal.y > 0.5f)
        {
            jumpCount = 0;
        }
    }

}
