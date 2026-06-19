using UnityEngine;
using UnityEngine.UI;
using Game.Audio;

public class UIButtonSFX : MonoBehaviour
{
    public AudioClip clickClip;

    [Range(0f, 1f)]
    public float volume = 1f;


    private void OnEnable()
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.onClick.RemoveListener(PlayClick);
            button.onClick.AddListener(PlayClick);
        }
    }

    private void PlayClick()
    {
        if (!clickClip)
            return;

        GameAudioManager.Instance?.PlaySFX(
            clickClip,
            volume
        );
    }
}