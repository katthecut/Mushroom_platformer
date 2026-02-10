using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopItemUI : MonoBehaviour
{
    public TMP_Text skinNameText;
    public Image skinImage;
    public TMP_Text costText;
    public Button actionButton;
    public TMP_Text actionButtonText;
    public TMP_Text messageText;

    private string skinId;
    private int cost;

    public void Init(string id, string displayName, Sprite sprite, int skinCost)
    {
        skinId = id;
        cost = skinCost;

        if (skinNameText != null) skinNameText.text = displayName;
        if (skinImage != null) skinImage.sprite = sprite;
        if (costText != null) costText.text = $"Cost: {cost}";
        if (messageText != null) messageText.text = "";

        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnActionPressed);
        }

        Refresh();
    }

    public void Refresh()
    {
        bool unlocked = SkinSave.IsUnlocked(skinId);

        if (!unlocked)
        {
            if (actionButtonText != null)
                actionButtonText.text = "Buy";

            if (messageText != null)
                messageText.text = "";

            return;
        }

        if (actionButtonText != null)
            actionButtonText.text = "Equip";
    }

    private void OnActionPressed()
    {
        if (messageText != null) messageText.text = "";

        bool unlocked = SkinSave.IsUnlocked(skinId);

        if (!unlocked)
        {
            int total = Currency.GetTotal();
            if (total < cost)
            {
                int missing = cost - total;
                if (messageText != null)
                    messageText.text = $"Not enough! Missing: {missing}";
                return;
            }

            Currency.Spend(cost);
            SkinSave.Unlock(skinId);

            if (messageText != null)
                messageText.text = "Purchased!";

            Refresh();
            ShopManager.Instance?.RefreshAll();
        }
        else
        {
            SkinSave.SetPending(skinId);
            if (messageText != null)
                messageText.text = "Restarting to equip...";

            ShopManager.Instance?.CloseShop();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}