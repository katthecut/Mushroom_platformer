using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMoth : MonoBehaviour
{
    public enum State { Patrol, Approach, Chase, Return }

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Enemy enemy;

    //patrol up/down
    public float patrolUpSpeed = 3f;
    public float patrolDownSpeed = 5f;
    public float patrolRange = 1.5f;

    //approach player within time
    public float approachSpeed = 8f;
    public float maxTimeToReachPlayer = 3f;
    public float reachDistance = 1.5f;

    //chase and attack range
    public float chaseSpeed = 10f;
    public float attackRange = 6f;
    public float escapeGraceTime = 0.75f;

    //return
    public float returnSpeed = 7f;
    public float returnStopDistance = 0.05f;

    // ceiling/floor checks - must have it
    public Transform topCheck;
    public Transform bottomCheck;
    public float checkRadius = 0.15f;
    public LayerMask groundLayer;

    //flip
    public bool flipVerticallyWithPatrol = true;
    public bool facePlayerHorizontallyWhenAggro = true;

    //Debug
    public bool debugLogs = false;

    private Vector3 startPos;
    private float startY;
    private bool movingUp = true;

    private State state = State.Patrol;

    private Transform playerTarget;
    private float approachTimer;
    private float escapeTimer;

    private bool setupOk = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        enemy = GetComponent<Enemy>();

        //flying + not pushable
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.gravityScale = 0f;

        //stops Enemy.cs from moving the moth
        if (enemy != null)
        {
            enemy.enableMovement = false;
            enemy.verticalMovement = false;
            enemy.movementSpeed = 0f;
        }
    }

    private void Start()
    {
        startPos = transform.position;
        startY = startPos.y;

        if (topCheck == null || bottomCheck == null)
        {
            setupOk = false;
        }

        ApplyPatrolFlipVisual();
    }

    private void Update()
    {
        if (!setupOk) return;

        //timers + state transitions
        if (state == State.Approach)
        {
            if (playerTarget == null) BeginReturn();
            else
            {
                approachTimer -= Time.deltaTime;

                float dist = Vector2.Distance(transform.position, playerTarget.position);
                if (dist <= reachDistance)
                {
                    state = State.Chase;
                    escapeTimer = 0f;
                    //if (debugLogs) Debug.Log("Moth reached player - CHASE");
                }
                else if (approachTimer <= 0f)
                {
                    //if (debugLogs) Debug.Log("Moth could not reach in time - RETURN");
                    BeginReturn();
                }
            }
        }
        else if (state == State.Chase)
        {
            if (playerTarget == null) BeginReturn();

            else
            {
                float dist = Vector2.Distance(transform.position, playerTarget.position);

                if (dist > attackRange)
                {
                    escapeTimer += Time.deltaTime;
                    if (escapeTimer >= escapeGraceTime)
                    {
                        if (debugLogs) Debug.Log("Moth: Player escaped -> RETURN");
                        BeginReturn();
                    }
                }
                else escapeTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!setupOk) return;

        switch (state)
        {
            case State.Patrol:
                PatrolVertical();
                break;
            case State.Approach:
                MoveTowardPlayer(approachSpeed);
                break;
            case State.Chase:
                MoveTowardPlayer(chaseSpeed);
                break;
            case State.Return:
                ReturnToStart();
                break;
        }
    }

    //called by LightZoneTrigger
    public void OnPlayerEnteredLightZone(Transform player)
    {
        if (!setupOk) return;

        playerTarget = player;
        state = State.Approach;
        approachTimer = maxTimeToReachPlayer;
        escapeTimer = 0f;

        //if (debugLogs) Debug.Log("Moth sees player in Light Zone - APPROACH");
    }

    public void OnPlayerExitedLightZone(Transform player) { }

    private void PatrolVertical()
    {
        //detect BEFORE move
        bool hitTop = Physics2D.OverlapCircle(topCheck.position, checkRadius, groundLayer);
        bool hitBottom = Physics2D.OverlapCircle(bottomCheck.position, checkRadius, groundLayer);

        if (movingUp && hitTop) movingUp = false;
        else if (!movingUp && hitBottom) movingUp = true;

        float speed = movingUp ? patrolUpSpeed : patrolDownSpeed;
        Vector2 dir = movingUp ? Vector2.up : Vector2.down;

        Vector2 next = rb.position + dir * speed * Time.fixedDeltaTime;

        float minY = startY - Mathf.Max(0.01f, patrolRange);
        float maxY = startY + Mathf.Max(0.01f, patrolRange);

        //prevents moth from getting stuck if it goes beyond the patrol range
        //i hope idk if it works dont touch it

        if (next.y > maxY)
        {
            next.y = maxY;
            movingUp = false;
        }
        else if (next.y < minY)
        {
            next.y = minY;
            movingUp = true;
        }

        rb.MovePosition(next);
        ApplyPatrolFlipVisual();
    }

    private void FacePlayerOnX(Vector3 targetPos)
    {
        if (sr == null) return;

        // okreni se prema igraču po X osi
        sr.flipX = targetPos.x < transform.position.x;
    }

    private void MoveTowardPlayer(float speed)
    {
        if (playerTarget == null)
        {
            BeginReturn();
            return;
        }

        Vector2 target = playerTarget.position;
        rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime));

        if (facePlayerHorizontallyWhenAggro)
            FacePlayerOnX(playerTarget.position);
    }

    private void ApplyPatrolFlipVisual()
    {
        if (!flipVerticallyWithPatrol || sr == null) return;
        sr.flipY = !movingUp;
    }

    private void ReturnToStart()
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, startPos, returnSpeed * Time.fixedDeltaTime));

        if (Vector2.Distance(rb.position, startPos) <= returnStopDistance)
        {
            rb.position = startPos;
            playerTarget = null;
            approachTimer = 0f;
            escapeTimer = 0f;

            state = State.Patrol;
            movingUp = true;
            ApplyPatrolFlipVisual();

            if (debugLogs) Debug.Log("Moth returned - PATROL");
        }
    }

    private void BeginReturn()
    {
        state = State.Return;
        playerTarget = null;

    }
}