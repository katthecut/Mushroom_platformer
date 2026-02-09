using UnityEngine;
using Game.Audio;

public class PickupAudio : MonoBehaviour
{
    public AudioClip pickupClip;

    public void PlayPickup()
    {
        GameAudioManager.Instance?.PlaySFX(pickupClip, 1f);
    }
}