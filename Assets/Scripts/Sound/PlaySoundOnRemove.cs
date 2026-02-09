using UnityEngine;
using Game.Audio;

[DisallowMultipleComponent]
public class PickupPlaySoundOnRemove : MonoBehaviour
{
    //pickup sound
    public AudioClip pickupClip;

    [Range(0f, 1f)]
    public float volumeMultiplier = 1f;

    public bool playOnDisable = true;
    public bool playOnDestroy = false;

    //plays if MarkCollected() was called before removal
    public bool requireCollectedFlag = true;

    public bool ignoreOnApplicationQuit = true;

    private bool collected;
    private bool hasPlayed;
    private static bool isQuitting;

    public void MarkCollected()
    {
        collected = true;
    }

    private void OnApplicationQuit() => isQuitting = true;

    private void OnDisable()
    {
        if (!playOnDisable) return;
        TryPlay();
    }

    private void OnDestroy()
    {
        if (!playOnDestroy) return;
        TryPlay();
    }

    private void TryPlay()
    {
        if (hasPlayed) return;
        if (ignoreOnApplicationQuit && isQuitting) return;

        if (requireCollectedFlag && !collected)
            return;

        hasPlayed = true;

        if (pickupClip == null) return;

        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.PlaySFX(pickupClip, volumeMultiplier);
            return;
        }

        AudioSource.PlayClipAtPoint(pickupClip, transform.position, Mathf.Clamp01(volumeMultiplier));
    }
}