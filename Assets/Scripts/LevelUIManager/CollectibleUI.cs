using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleUI : MonoBehaviour
{
    //UI
    [SerializeField] private Image collectibleIcon;
    [SerializeField] private TMP_Text counterText;

    public void SetIcon(Sprite sprite)
    {
        if (collectibleIcon != null)
        {
            collectibleIcon.sprite = sprite;
        }
    }

    public void SetCounter(int current, int total)
    {
        if (counterText != null)
        {
            counterText.text = $"{current} / {total}";
        }
    }
}