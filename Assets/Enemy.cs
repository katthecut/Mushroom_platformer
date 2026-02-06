using System;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{

    private Rigidbody2D rb;
    public int normalDamage = 10;
    public float movementSpeed = 5.0f;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //  Ovaj dio koda pomaze da vidimo gdje se nalaze transformi za raycastove u editoru
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        Gizmos.DrawWireSphere(groundCheckFront.position, wallCheckRadius);
    }


    // Update is called once per frame
    void Update()
    {
        if (verticalMovement)
        {
            VerticalMovement();
        }
        else
        {
            Move();
        }


        // imamo 2 groundChecka, jedan je ispod kako bi provjerio tlo, a drugi je ispred kako bi provjerio da nema rupa
        if (!Physics2D.OverlapCircle(groundCheckFront.position, groundCheckRadius, groundLayer))
        {
            OnPlatformEndDetected?.Invoke();
            Flip();
        }

        if (Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer))
        {
            OnWallDetected?.Invoke();
            Flip();
        }

        // tu spremimo rezultat funkcije FindPlayer
        bool playerFound = FindPlayer();

        if (playerFound)
        {
            // tu sad odraditi napad na igraca
            // pokrece se event koji se moze koristiti u drugim skriptama
            OnPlayerDetected?.Invoke();
        }

        //if (hasRaycast) 
        //{ 
        //    Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        //    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, playerCheckDistance, Player);

        //    if (hit.collider != null)
        //    {
        //        Debug.Log("Raycast radii!");
        //    }
        //}

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

    //promijeniti kasnije za MothEnemy da ima dobar flip...

    public void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;

        transform.localScale = scale;

        _direction *= -1;
    }

    // promjenio sam vamo da umjesto void vraca bool tako da mozes koristiti
    // rezultat funkcije - true kad je igrac pronaden, false kad nije

    bool FindPlayer()
    {
        //Debug.Log("FIND PLAYER CALLED");


        Vector2 direction = _direction > 0 ? Vector2.right : Vector2.left;
        Vector3 rayOrigin = transform.position;// + new Vector3(0, 1f, 0);

        // player layer mask da sve ostale collidere koji nisu player ignorira
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, playerCheckDistance, Player);
        Debug.DrawRay(rayOrigin, direction * playerCheckDistance, Color.red);

        // ovdje napravimo provjeru da li je nesto pogodjeno i jel to igrac
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            //Debug.Log("Player found!");
            // ako je igrac pronaden vrati true
            return true;
        }
        // ako igrac nije pronaden vrati false
        return false;
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
