using UnityEngine;
using Game.Audio;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip hitClip;
    public AudioClip jumpClip;
    public AudioClip moveClip;
    public AudioClip attackClip;


    [Header("Footsteps")]
    public float moveInterval = 0.35f;
    public float moveMinSpeed = 0.1f;

    public LayerMask groundLayer;
    public float groundCastDistance = 0.08f;

    [Range(0.2f, 1f)]
    public float groundCastWidthFactor = 0.9f;

    private Rigidbody2D rb;
    private Collider2D col;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Mathf.Abs(rb.linearVelocity.x) > moveMinSpeed && IsGrounded())
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                Play(moveClip, 0.7f);
                timer = moveInterval;
            }
        }
        else
        {
            timer = 0f;
        }
    }

    private bool IsGrounded()
    {
        Bounds b = col.bounds;

        Vector2 size = new Vector2(
            b.size.x * groundCastWidthFactor,
            0.05f
        );

        Vector2 origin = new Vector2(
            b.center.x,
            b.min.y + 0.02f
        );


        return Physics2D.BoxCast(
            origin,
            size,
            0f,
            Vector2.down,
            groundCastDistance,
            groundLayer
        );
    }


    private void Play(AudioClip clip, float volume = 1f)
    {
        if (!clip)
            return;

        GameAudioManager.Instance?.PlaySFX(
            clip,
            volume
        );
    }

    public void PlayHit()
    {
        Play(hitClip);
    }

    public void PlayJump()
    {
        Play(jumpClip);
    }

    public void PlayMove()
    {
        Play(moveClip, 0.7f);
    }

    public void PlayAttack()
    {
        Play(attackClip);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();

        if (!c)
            return;

        Bounds b = c.bounds;

        Vector2 size = new Vector2(
            b.size.x * groundCastWidthFactor,
            0.05f
        );

        Vector2 start = new Vector2(
            b.center.x,
            b.min.y + 0.02f
        );

        Gizmos.DrawWireCube(start, size);
        Gizmos.DrawWireCube(
            start + Vector2.down * groundCastDistance,
            size
        );
    }
#endif
}