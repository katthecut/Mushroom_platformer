using UnityEngine;
using Game.Audio;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerAudio : MonoBehaviour
{
    //clips
    public AudioClip hitClip;
    public AudioClip jumpClip;
    public AudioClip moveClip;
    public AudioClip attackClip;

    //footsteps
    public float moveInterval = 0.35f;
    public float moveMinSpeed = 0.1f;

    //Ground Check za footsteps
    public LayerMask groundLayer; // assignaj u inspectoru na ground
    public float groundCastDistance = 0.08f;

    [Range(0.2f, 1f)]
    public float groundCastWidthFactor = 0.9f;

    private Rigidbody2D rb;
    private Collider2D col;
    private float moveTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        //koristi trenutni horizontal speed
        float speed = Mathf.Abs(rb.linearVelocity.x);

        //footsteps only when moving on ground
        if (speed > moveMinSpeed && IsGrounded())
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f)
            {
                PlayMove();
                moveTimer = moveInterval;
            }
        }
        else
        {
            moveTimer = 0f;
        }
    }

    private bool IsGrounded()
    {
        if (col == null) return false;

        Bounds b = col.bounds;

        //BoxCast size kod nogu
        Vector2 castSize = new Vector2(b.size.x * groundCastWidthFactor, 0.05f);
        Vector2 castOrigin = new Vector2(b.center.x, b.min.y + 0.02f);

        //BoxCast kod groundLayer
        RaycastHit2D hit = Physics2D.BoxCast(
            castOrigin,
            castSize,
            0f,
            Vector2.down,
            groundCastDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    public void PlayHit()
    {
        if (hitClip == null) return;
        GameAudioManager.Instance?.PlaySFX(hitClip, 1f);
    }

    public void PlayJump()
    {
        if (jumpClip == null) return;
        GameAudioManager.Instance?.PlaySFX(jumpClip, 1f);
    }

    public void PlayMove()
    {
        if (moveClip == null) return;
        GameAudioManager.Instance?.PlaySFX(moveClip, 0.7f);
    }

    public void PlayAttack()
    {
        if (attackClip == null) return;
        GameAudioManager.Instance?.PlaySFX(attackClip, 1f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Visualize ground cast in editor
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;

        Bounds b = c.bounds;
        Vector2 castSize = new Vector2(b.size.x * groundCastWidthFactor, 0.05f);
        Vector2 castOrigin = new Vector2(b.center.x, b.min.y + 0.02f);
        Vector2 castEnd = castOrigin + Vector2.down * groundCastDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(castOrigin, castSize);
        Gizmos.DrawWireCube(castEnd, castSize);
    }
#endif
}
