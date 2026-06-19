using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyProximityAudioLoop : MonoBehaviour
{
    public AudioClip loopClip;

    [Range(0f, 1f)]
    public float volume = 0.8f;

    [Range(0f, 3f)]
    public float volumeMultiplier = 1f;

    [Min(0f)]
    public float fadeTime = 0.15f;

    [Range(0f, 1f)]
    public float spatialBlend = 0f;

    private AudioSource source;
    private int players;
    private float targetVolume;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        source = GetComponent<AudioSource>();

        if (!source)
            source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = spatialBlend;
        source.clip = loopClip;
        source.volume = 0f;
    }

    private void Update()
    {
        if (fadeTime <= 0f)
        {
            source.volume = targetVolume;
            UpdatePlayback();
            return;
        }

        source.volume = Mathf.MoveTowards(
            source.volume,
            targetVolume,
            Time.deltaTime / fadeTime
        );

        UpdatePlayback();
    }

    private void UpdatePlayback()
    {
        if (targetVolume > 0f)
        {
            if (!source.isPlaying && loopClip)
                source.Play();
        }
        else if (source.isPlaying && source.volume <= 0f)
        {
            source.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        players++;
        SetTargetVolume();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        players = Mathf.Max(0, players - 1);
        SetTargetVolume();
    }


    private void SetTargetVolume()
    {
        targetVolume = players > 0
            ? Mathf.Clamp01(volume) * Mathf.Max(0f, volumeMultiplier)
            : 0f;
    }

    public void SetVolume(float value)
    {
        volume = Mathf.Clamp01(value);
        SetTargetVolume();
    }

    public void SetMultiplier(float value)
    {
        volumeMultiplier = Mathf.Max(0f, value);
        SetTargetVolume();
    }
}