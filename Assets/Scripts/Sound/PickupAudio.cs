using UnityEngine;
using Game.Audio;

public class PickupAudio : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string id = "pickup";
    [SerializeField] private bool useObjectName = true;

    [Header("Sound")]
    public AudioClip pickupClip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Play Options")]
    public bool playOnDisable = true;
    public bool playOnDestroy = false;

    public bool requireCollected = true;
    public bool ignoreQuit = true;


    private bool collected;
    private bool played;

    private static bool quitting;

    public string Id
    {
        get
        {
            return Normalize(
                useObjectName
                ? gameObject.name
                : id
            );
        }
    }

    private void OnApplicationQuit()
    {
        quitting = true;
    }

    public void MarkCollected()
    {
        collected = true;
    }

    public void Collected()
    {
        MarkCollected();
        Play();
    }

    private void OnDisable()
    {
        if (playOnDisable)
            Play();
    }
    private void OnDestroy()
    {
        if (playOnDestroy)
            Play();
    }

    private void Play()
    {
        if (played)
            return;

        if (ignoreQuit && quitting)
            return;

        if (requireCollected && !collected)
            return;

        if (!pickupClip)
            return;

        played = true;

        if (GameAudioManager.Instance)
        {
            GameAudioManager.Instance.PlaySFX(
                pickupClip,
                volume
            );
        }
        else
        {
            AudioSource.PlayClipAtPoint(
                pickupClip,
                transform.position,
                volume
            );
        }
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        return value
            .Replace("(Clone)", "")
            .Trim();
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        id = Normalize(id);
    }

#endif
}