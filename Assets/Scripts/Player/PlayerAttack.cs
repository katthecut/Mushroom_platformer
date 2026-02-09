using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPosition;

    private float timeBetweenAttack;
    public float startTimeBetweenAttack;

    public bool enforceLocalAttackOrigin = true;
    public Vector2 localAttackOffset = new Vector2(0.6f, 0f);
    public float attackRange;

    //target filtering
    //ide na layer koji samo enemy koristi
    public LayerMask enemyHurtboxLayer;

    public string hurtboxTag = "";


    public int damage = 10;
    //mislim da ne treba deklarirati dmg value sada ako cu hardkodirati skinove...
    [Range(4, 64)] public int maxHits = 32;

    //Debug
    public bool debugOrigin = true;
    public bool debugGizmos = true;

    private Collider2D[] hitBuffer;
    private readonly HashSet<EnemyHealth> damagedThisSwing = new HashSet<EnemyHealth>();
    private PlayerAudio playerAudio;

    private void Awake()
    {
        hitBuffer = new Collider2D[Mathf.Max(4, maxHits)];
        playerAudio = GetComponent<PlayerAudio>();

        if (attackPosition == null)
        {
            Transform found = transform.Find("attackPosition");
            if (found == null) found = transform.Find("AttackPosition");
            if (found != null) attackPosition = found;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FixOrCreateAttackPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeBetweenAttack > 0)
        {
            timeBetweenAttack -= Time.deltaTime;
        }
    }

    //ja sebe ocito ne mrzim dovoljno
    private void FixOrCreateAttackPosition()
    {
        if (!enforceLocalAttackOrigin) return;

        if (attackPosition == null)
        {
            GameObject go = new GameObject("AttackPosition");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = localAttackOffset;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            attackPosition = go.transform;

            if (debugOrigin)
            {
                Debug.Log("PlayerAttack attackPosition missing -> created.");
            }
            return;
        }

        if (!attackPosition.IsChildOf(transform))
            attackPosition.SetParent(transform, false);

        float worldDist = Vector2.Distance(transform.position, attackPosition.position);
        if (worldDist > 5f)
        {
            attackPosition.localPosition = localAttackOffset;
            attackPosition.localRotation = Quaternion.identity;
            attackPosition.localScale = Vector3.one;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (timeBetweenAttack > 0f) return;

        timeBetweenAttack = startTimeBetweenAttack;

        FixOrCreateAttackPosition(); //osigurava da attackPosition postoji ili ga kreira
        if (attackPosition == null) return; //ako se attackPosition nije napravio

        if (enforceLocalAttackOrigin)
        {
            float facing = Mathf.Sign(transform.localScale.x);
            attackPosition.localPosition = new Vector3(localAttackOffset.x * facing, localAttackOffset.y, 0f);
        }

        Vector2 origin = attackPosition.position;

        if (debugOrigin)
        {
            //debug info za provjeru attack origin-a
            Debug.Log($"[PlayerAttack] Origin={origin} PlayerPos={(Vector2)transform.position} range={attackRange}");
        }

        playerAudio?.PlayAttack();

        ContactFilter2D filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = enemyHurtboxLayer,
            useTriggers = true
        };

        int count = Physics2D.OverlapCircle(origin, attackRange, filter, hitBuffer);

        damagedThisSwing.Clear(); //reset liste pogodjenih enemies

        for (int i = 0; i < count; i++)
        {
            //uzimanje pogodjenog collidera
            Collider2D col = hitBuffer[i];
            if (col == null) continue;

            if (!string.IsNullOrEmpty(hurtboxTag) && !col.CompareTag(hurtboxTag))
                continue;

            Vector2 closest = col.ClosestPoint(origin);
            if (Vector2.Distance(origin, closest) > attackRange) continue;

            //trazi EnemyHealth na parent object
            EnemyHealth eh = col.GetComponentInParent<EnemyHealth>();
            if (eh == null) continue;
            if (damagedThisSwing.Contains(eh)) continue;

            damagedThisSwing.Add(eh);
            eh.TakeDamage(damage);
        }
    }


    void OnDrawGizmosSelected()
    {
        if (!debugGizmos) return;

        Vector3 pos = attackPosition != null
            ? attackPosition.position
            : transform.position + (Vector3)localAttackOffset;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(pos, attackRange);
    }

}