using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyProximityAudioLoop : MonoBehaviour
{
    //loop
    public AudioClip loopClip;

    //volume
    [Range(0f, 1f)]
    public float volume = 0.8f;

    [Range(0f, 3f)]
    public float volumeMultiplier = 1f;

    //fade
    [Min(0f)]
    public float fadeTime = 0.15f;

    //3D/2D
    [Range(0f, 1f)]
    public float spatialBlend = 0f;

    private AudioSource src;
    private int playersInRange = 0;
    private float targetVolume = 0f;

    private void Awake()
    {
        //ensure trigger collider
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        //setup looping audio
        src = GetComponent<AudioSource>();
        if (src == null) src = gameObject.AddComponent<AudioSource>();

        src.playOnAwake = false;
        src.loop = true;
        src.spatialBlend = spatialBlend;
        src.clip = loopClip;

        // Start silent (fade in)
        src.volume = 0f;
        targetVolume = 0f;
    }

    private void Update()
    {
        if (fadeTime <= 0f)
        {
            //instant
            src.volume = targetVolume;
            if (targetVolume > 0f)
            {
                if (!src.isPlaying && loopClip != null)
                    src.Play();
            }
            else
            {
                if (src.isPlaying)
                    src.Stop();
            }
            return;
        }

        //smooth fade
        src.volume = Mathf.MoveTowards(src.volume, targetVolume, Time.deltaTime / fadeTime);

        //start/stop based on volume transitions
        if (targetVolume > 0f)
        {
            if (!src.isPlaying && loopClip != null)
                src.Play();
        }
        else
        {
            // stop only after fully faded out
            if (src.isPlaying && Mathf.Approximately(src.volume, 0f))
                src.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playersInRange++;
        if (playersInRange < 1) playersInRange = 1;

        BeginLoop();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playersInRange--;
        if (playersInRange <= 0)
        {
            playersInRange = 0;
            EndLoop();
        }
    }

    private void BeginLoop()
    {
        if (loopClip == null)
            return;

        //za koristiti najnociji clip
        if (src.clip != loopClip)
            src.clip = loopClip;

        targetVolume = Mathf.Clamp01(volume) * Mathf.Max(0f, volumeMultiplier);

        //start
        if (!src.isPlaying)
            src.Play();
    }

    private void EndLoop()
    {
        targetVolume = 0f;
        // Update() will fade out then Stop()
    }

    // Optional change volume at runtime
    public void SetVolume(float newVolume01)
    {
        volume = Mathf.Clamp01(newVolume01);
        if (playersInRange > 0)
            targetVolume = Mathf.Clamp01(volume) * Mathf.Max(0f, volumeMultiplier);
    }

    public void SetMultiplier(float newMultiplier)
    {
        volumeMultiplier = Mathf.Max(0f, newMultiplier);
        if (playersInRange > 0)
            targetVolume = Mathf.Clamp01(volume) * Mathf.Max(0f, volumeMultiplier);
    }
}