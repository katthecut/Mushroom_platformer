using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    //UI (Optional - TextMeshPro
    [SerializeField] private TMP_Text collectedText;       // e.g. "3/10"
    [SerializeField] private TMP_Text totalCollectedText;  // e.g. "Total: 57"

    //Auto Count
    //If true, counts all collectibles in the scene at Start
    public bool autoCountCollectibles = true;

    private int collectedThisLevel = 0;
    private int totalInLevel = 0;

    private const string PREF_TOTAL = "TOTAL_COLLECTED";

    public int CollectedThisLevel => collectedThisLevel;
    public int TotalInLevel => totalInLevel;
    public int TotalCollectedAllTime => PlayerPrefs.GetInt(PREF_TOTAL, 0);

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

    public void Collect(CollectibleItem item)
    {
        if (item == null) return;

        // Persist collectible if needed
        item.MarkPersisted();

        // Level counter
        collectedThisLevel += 1;

        // Global total saved
        int total = PlayerPrefs.GetInt(PREF_TOTAL, 0);
        total += Mathf.Max(1, item.value);
        PlayerPrefs.SetInt(PREF_TOTAL, total);
        PlayerPrefs.Save();

        // Remove pickup
        Destroy(item.gameObject);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (collectedText != null)
            collectedText.text = $"Leaves: {collectedThisLevel}/{totalInLevel}";

        if (totalCollectedText != null)
            totalCollectedText.text = $"Total: {TotalCollectedAllTime}";
    }

    // Optional manual override
    public void SetTotalInLevel(int total)
    {
        totalInLevel = Mathf.Max(0, total);
        UpdateUI();
    }
}
