using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    // UI
    [SerializeField] private TMP_Text collectedText;
    [SerializeField] private TMP_Text totalCollectedText;

    // Win
    [SerializeField] private GameObject winPanel;

    // Collectibles
    [SerializeField] private bool autoCountCollectibles = true;

    // Skins
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

        Time.timeScale = 1f;

        //prebroji leaves
        if (autoCountCollectibles)
        {
            CollectibleItem[] items =
                FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None);

            totalInLevel = items.Length;
        }

        //counter pocinje od 0
        collectedThisLevel = 0;

        // VictoryPanel skriven na pocetku
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        UpdateUI();
    }

    private void Update()
    {
        //reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetAllProgress();
        }
    }

    public void Collect(CollectibleItem item)
    {
        if (item == null)
            return;

        item.MarkPersisted();

        collectedThisLevel++;

        // Dodaje leaf u ukupni broj za shop
        int total = PlayerPrefs.GetInt(PREF_TOTAL, 0);

        total += Mathf.Max(1, item.value);

        PlayerPrefs.SetInt(PREF_TOTAL, total);
        PlayerPrefs.Save();

        UpdateUI();

        if (collectedThisLevel >= totalInLevel && totalInLevel > 0)
        {
            Victory();
        }
    }

    private void UpdateUI()
    {
        if (collectedText != null)
        {
            collectedText.text = $"{collectedThisLevel}/{totalInLevel}";
        }

        if (totalCollectedText != null)
        {
            totalCollectedText.text =
                $"Total: {PlayerPrefs.GetInt(PREF_TOTAL, 0)}";
        }
    }

    private void Victory()
    {
        Debug.Log("YOU WIN!");

        if (winPanel == null)
        {
            Debug.LogError("VictoryPanel nije dodijeljen u CollectibleManageru!");
            return;
        }

        //pause gameplay
        Time.timeScale = 0f;

        //prikazi VictoryPanel
        winPanel.SetActive(true);

        Debug.Log("VictoryPanel on!");
    }

    public void ResetAllProgress()
    {
        Time.timeScale = 1f;

        collectedThisLevel = 0;
        totalInLevel = 0;

        PlayerPrefs.DeleteKey(PREF_TOTAL);

        ResetSkins();
        ResetShopUI();
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
        {
            ShopManager.Instance.RefreshAll();
        }
    }

    private void ResetPlayerSkin()
    {
        PlayerSkinController controller =
            FindFirstObjectByType<PlayerSkinController>();

        if (controller != null)
        {
            controller.ForceResetToDefault();
        }
    }
}