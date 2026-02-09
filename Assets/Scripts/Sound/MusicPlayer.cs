using UnityEngine;
using Game.Audio;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusic;

    private void Start()
    {
        if (GameAudioManager.Instance != null && backgroundMusic != null)
            GameAudioManager.Instance.PlayMusic(backgroundMusic);
    }
}