using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    //UI
    public TMP_Text skinNameText;
    public Image skinImage;
    public TMP_Text costText;
    public Button actionButton;
    public TMP_Text actionButtonText;
    public TMP_Text messageText;

    public bool forceFillCell = true;

    private string skinId;
    private int cost;

    public void Init(string id, string displayName, Sprite sprite, int skinCost)
    {
        skinId = id;
        cost = skinCost;

        if (forceFillCell)
            ForceRootToFillCell();

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

    private void ForceRootToFillCell()
    {
        //this makes root obey GridLayoutGroup placement and sizing
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null) return;

        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);

        LayoutElement le = GetComponent<LayoutElement>();
        if (le != null)
        {
            le.preferredWidth = -1;
            le.preferredHeight = -1;
            le.flexibleWidth = 0;
            le.flexibleHeight = 0;
        }
    }

    public void Refresh()
    {
        bool unlocked = SkinSave.IsUnlocked(skinId);
        if (actionButtonText != null)
            actionButtonText.text = unlocked ? "Equip" : "Buy";
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
