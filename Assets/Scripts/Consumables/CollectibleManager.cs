using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text collectedText;
    [SerializeField] private TMP_Text totalCollectedText;

    [Header("Collectibles")]
    public bool autoCountCollectibles = true;

    [Header("Skins")]
    [SerializeField] private string[] allSkinIds;

    private int collectedThisLevel = 0;
    private int totalInLevel = 0;

    private const string PREF_TOTAL = "TOTAL_COLLECTED";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (autoCountCollectibles)
        {
            CollectibleItem[] items = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None);
            totalInLevel = items.Length;
        }

        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetAllProgress();
        }
    }

    public void Collect(CollectibleItem item)
    {
        if (item == null) return;

        item.MarkPersisted();

        collectedThisLevel++;

        int total = PlayerPrefs.GetInt(PREF_TOTAL, 0);
        total += Mathf.Max(1, item.value);
        PlayerPrefs.SetInt(PREF_TOTAL, total);
        PlayerPrefs.Save();

        Destroy(item.gameObject);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (collectedText != null)
            collectedText.text = $"Leaves: {collectedThisLevel}/{totalInLevel}";

        if (totalCollectedText != null)
            totalCollectedText.text = $"Total: {PlayerPrefs.GetInt(PREF_TOTAL, 0)}";
    }

    public void ResetAllProgress()
    {
        // 1. Reset collectibles
        collectedThisLevel = 0;
        totalInLevel = 0;
        PlayerPrefs.DeleteKey(PREF_TOTAL);

        // 2. Reset skinove
        ResetSkins();

        // 3. Osvježi shop UI
        ResetShopUI();

        // 4. Forsiraj default skin
        ResetPlayerSkin();

        PlayerPrefs.Save();
        UpdateUI();
    }

    private void ResetSkins()
    {
        if (allSkinIds != null)
        {
            foreach (string skinId in allSkinIds)
            {
                PlayerPrefs.DeleteKey("SKIN_UNLOCKED_" + skinId);
            }
        }

        PlayerPrefs.DeleteKey("SKIN_EQUIPPED");
        PlayerPrefs.DeleteKey("SKIN_PENDING");
    }

    private void ResetShopUI()
    {
        if (ShopManager.Instance != null)
            ShopManager.Instance.RefreshAll();
    }

    private void ResetPlayerSkin()
    {
        PlayerSkinController controller = FindFirstObjectByType<PlayerSkinController>();
        if (controller != null)
        {
            controller.ForceResetToDefault();
        }
    }
}