using UnityEngine;
using UnityEngine.UI;
using Game.Audio;

public class UIButtonSFX : MonoBehaviour
{
    public AudioClip clickClip;
    [Range(0f, 1f)] public float volume = 1f;

    private void OnEnable()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button b in buttons)
        {
            b.onClick.RemoveListener(PlayClick);
            b.onClick.AddListener(PlayClick);
        }
    }

    private void PlayClick()
    {
        GameAudioManager.Instance?.PlaySFX(clickClip, volume);
    }
}