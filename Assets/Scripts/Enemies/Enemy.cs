using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{

    private Rigidbody2D rb;
    public int normalDamage = 10;

    public bool enableMovement = true;
    public float movementSpeed = 5.0f;

    public bool canBePushedByPlayer = false;

    // ovdje ide groundcheck ispod enemya
    public Transform groundCheck;
    // ovdje je groundcheck koji je ispred enemya
    public Transform groundCheckFront;
    public Transform wallCheck;

    //ne smije preko 0.5
    public float groundCheckRadius = 0.5f;
    public float wallCheckRadius = 0.5f;

    public LayerMask groundLayer;

    //sve za vertical movement
    public bool verticalMovement = false;

    public float upSpeed = 3f;
    public float downSpeed = 5f;
    public float verticalRange = 1.5f;

    private float startY;
    private bool movingUp = true;

    //raycast
    public bool hasRaycast = false;
    public LayerMask Player;
    public float playerCheckDistance = 20f;

    private float _direction = 1f;

    public UnityEvent OnPlayerDetected;
    public UnityEvent OnWallDetected;
    public UnityEvent OnPlatformEndDetected;

    //prevent flipp-spamming
    private bool prevHasGroundFront = true;
    private bool prevHitWall = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        //non-pushable by default
        if (!canBePushedByPlayer)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        startY = transform.position.y;
    }

    void Start()
    {
        prevHasGroundFront = CheckGroundFront();
        prevHitWall = CheckWall();
    }

    //  Ovaj dio koda pomaze da vidimo gdje se nalaze transformi za raycastove u editoru
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (groundCheckFront != null)
            Gizmos.DrawWireSphere(groundCheckFront.position, groundCheckRadius);

        if (wallCheck != null)
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
    }


    // Update is called once per frame
    void Update()
    {
        // Movement
        if (enableMovement)
        {
            if (verticalMovement) VerticalMovement();
            else Move();

            //EDGE: only flip when there is 'ground' to 'no ground'

            bool hasGroundFront = CheckGroundFront();
            if (prevHasGroundFront && !hasGroundFront)
            {
                OnPlatformEndDetected?.Invoke();
                Flip();
            }
            prevHasGroundFront = hasGroundFront;

            //WALL: only flip when there is 'no wall' to 'wall'
            bool hitWall = CheckWall();
            if (!prevHitWall && hitWall)
            {
                OnWallDetected?.Invoke();
                Flip();
            }
            prevHitWall = hitWall;
        }

        if (hasRaycast && FindPlayer())
        {
            OnPlayerDetected?.Invoke();
        }

    }

    public void Move()
    {
        transform.Translate(Vector2.right * movementSpeed * _direction * Time.deltaTime);
    }

    void VerticalMovement()
    {
        float speed = movingUp ? upSpeed : downSpeed;

        transform.Translate(Vector2.up * speed * Time.deltaTime);

        float offset = transform.position.y - startY;

        if (movingUp && offset >= verticalRange)
        {
            movingUp = false;
        }
        else if (!movingUp && offset <= -verticalRange)
        {
            movingUp = true;
        }
    }

    public void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;

        transform.localScale = scale;

        _direction *= -1;
    }

    bool FindPlayer()
    {
        Vector2 direction = _direction > 0 ? Vector2.right : Vector2.left;
        Vector3 rayOrigin = transform.position;

        // player layer mask da sve ostale collidere koji nisu player ignorira
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, playerCheckDistance, Player);
        Debug.DrawRay(rayOrigin, direction * playerCheckDistance, Color.red);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private bool CheckGroundFront()
    {
        if (groundCheckFront == null) return true;
        return Physics2D.OverlapCircle(groundCheckFront.position, groundCheckRadius, groundLayer);
    }

    private bool CheckWall()
    {
        if (wallCheck == null) return false;
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer);
    }

    //deal dmg playeru
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(normalDamage);
            //Debug.Log("Damage dealt to player");

        }
    }
}