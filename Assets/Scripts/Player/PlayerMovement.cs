using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = true;

    private float originalGravity;

    public float moveSpeed = 15f;
    float horizontalMove;

    public float jumpPower = 30f;
    public int maxJumps = 2;
    private int jumpCount;

    private bool isDashing;
    private bool canDash = true;

    public float dashPower = 25f;
    public float dashTime = 2f;
    public float dashCooldown = 1f;

    private float dashTimer;
    private float dashCooldownTimer;

    public ParticleSystem jumpParticles;
    public TrailRenderer trail;

    // Wall/ground contact tracking
    private bool isGrounded;
    private bool isTouchingWall;

    // cached audio
    private PlayerAudio playerAudio;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 8f;

        playerAudio = GetComponent<PlayerAudio>();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.gravityScale = originalGravity;

                if (trail != null)
                    trail.emitting = false;
            }
        }

        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0f)
                canDash = true;
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
            return;

        float appliedHorizontal = horizontalMove;

        // Wall fix: if touching wall while airborne, don't keep forcing X velocity into wall
        if (isTouchingWall && !isGrounded)
            appliedHorizontal = 0f;

        rb.linearVelocity = new Vector2(appliedHorizontal * moveSpeed, rb.linearVelocity.y);

        if (horizontalMove != 0)
            Flip();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMove = context.ReadValue<Vector2>().x;
        if (horizontalMove != 0)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5f);
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            if (jumpParticles != null)
                jumpParticles.Play();

            // jump sound (only when jump actually happens)
            playerAudio?.PlayJump();

            animator.SetTrigger("Jump");

            jumpCount++;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        Physics2D.IgnoreLayerCollision(6, 7, true);

        if (!context.performed) return;
        if (!canDash || isDashing) return;

        canDash = false;
        isDashing = true;
        animator.SetTrigger("Dash");

        dashTimer = dashTime;
        dashCooldownTimer = dashCooldown;

        originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2((isFacingRight ? 1f : -1f) * dashPower, 0f);

        if (trail != null)
            trail.emitting = true;

        Physics2D.IgnoreLayerCollision(6, 7, false);
    }

    private void Flip()
    {
        if ((isFacingRight && horizontalMove < 0) || (!isFacingRight && horizontalMove > 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) => EvaluateContacts(collision);
    private void OnCollisionStay2D(Collision2D collision) => EvaluateContacts(collision);

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        isTouchingWall = false;
    }

    private void EvaluateContacts(Collision2D collision)
    {
        isGrounded = false;
        isTouchingWall = false;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 n = collision.contacts[i].normal;

            if (n.y > 0.5f)
            {
                isGrounded = true;
                jumpCount = 0;
            }

            if (Mathf.Abs(n.x) > 0.5f && n.y < 0.5f)
            {
                isTouchingWall = true;
            }
        }
    }
}
