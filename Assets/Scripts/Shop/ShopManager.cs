using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [System.Serializable]
    public class SkinDefinition
    {
        public string skinId;
        public string skinName;
        public Sprite skinSprite;
        public int cost;
    }

    //shop UI
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;

    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private ShopItemUI itemPrefab;
    [SerializeField] private TMP_Text totalCollectiblesText;

    //manual horizontal layout
    [SerializeField] private Vector2 itemSize = new Vector2(260f, 140f);
    [SerializeField] private float spacing = 20f;

    // 0 = top of viewport, 0.5 = middle, 1 = bottom
    [Range(0f, 1f)]
    [SerializeField] private float rowVerticalAnchor = 0.5f;

    [SerializeField] private float leftPadding = 20f;
    [SerializeField] private float rightPadding = 20f;

    //player skins
    [SerializeField] private SkinDefinition[] skins;

    private ShopItemUI[] spawned;
    private bool built;

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
        if (shopPanel != null)
            shopPanel.SetActive(false);

        ForceDisableAutoLayout();
        ForceCorrectRects();

        BuildShop();
        RefreshAll();
    }

    private void Update()
    {
        if (shopPanel != null && shopPanel.activeSelf && ShouldBlockShop())
            CloseShop();
    }

    public void OpenShop()
    {
        if (ShouldBlockShop())
            return;

        if (shopPanel == null) return;

        shopPanel.SetActive(true);

        ForceDisableAutoLayout();
        ForceCorrectRects();

        if (!built)
            BuildShop();

        RefreshAll();

        // Re-position after opening (viewport size might change)
        PositionItemsInRow();

        Canvas.ForceUpdateCanvases();
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    public void RefreshAll()
    {
        if (totalCollectiblesText != null)
            totalCollectiblesText.text = $"Total: {Currency.GetTotal()}";

        if (spawned == null) return;

        for (int i = 0; i < spawned.Length; i++)
            if (spawned[i] != null)
                spawned[i].Refresh();
    }

    private bool ShouldBlockShop()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsShown) return true;
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return true;
        return false;
    }

    // --- KEY FIX: disable GridLayoutGroup/ContentSizeFitter so they stop moving our items
    private void ForceDisableAutoLayout()
    {
        if (content == null) return;

        var grid = content.GetComponent<GridLayoutGroup>();
        if (grid != null) grid.enabled = false;

        var fitter = content.GetComponent<ContentSizeFitter>();
        if (fitter != null) fitter.enabled = false;

        // Optional: horizontal scrolling only
        if (scrollRect != null)
        {
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
        }
    }

    //ti have content match viewport height
    private void ForceCorrectRects()
    {
        if (viewport == null || content == null) return;

        //content MUST be anchored left-middle
        content.anchorMin = new Vector2(0f, 0.5f);
        content.anchorMax = new Vector2(0f, 0.5f);
        content.pivot = new Vector2(0f, 0.5f);

        //content starts aligned with viewport left
        content.anchoredPosition = Vector2.zero;

        // Match content height to viewport height so row stays inside
        float h = viewport.rect.height;
        content.sizeDelta = new Vector2(content.sizeDelta.x, h);
    }

    private void BuildShop()
    {
        if (content == null || itemPrefab == null)
        {
            Debug.LogError("ShopManager: Assign Viewport, Content and Item Prefab.");
            return;
        }

        //clear
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        spawned = new ShopItemUI[skins.Length];

        for (int i = 0; i < skins.Length; i++)
        {
            var def = skins[i];

            ShopItemUI ui = Instantiate(itemPrefab, content);
            ui.Init(def.skinId, def.skinName, def.skinSprite, def.cost);

            //this is so each item has RectTransform
            RectTransform rt = ui.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.sizeDelta = itemSize;

            spawned[i] = ui;
        }

        built = true;

        PositionItemsInRow();
        Canvas.ForceUpdateCanvases();
    }

    private void PositionItemsInRow()
    {
        if (viewport == null || content == null || spawned == null) return;

        float viewportHeight = viewport.rect.height;

        //row y inside viewport
        // 0 = top, 0.5 = center, 1 = bottom
        float yTop = (viewportHeight * 0.5f) - (itemSize.y * 0.5f);
        float yBottom = -(viewportHeight * 0.5f) + (itemSize.y * 0.5f);

        float y = Mathf.Lerp(yTop, yBottom, rowVerticalAnchor);
        //za center treba biti y = 0f;

        float x = leftPadding;

        for (int i = 0; i < spawned.Length; i++)
        {
            if (spawned[i] == null) continue;

            RectTransform rt = spawned[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(x, y);

            x += itemSize.x + spacing;
        }

        //ovo je da content bude dovoljno sirok za sve skinove
        float totalWidth = leftPadding + rightPadding + spawned.Length * itemSize.x + Mathf.Max(0, spawned.Length - 1) * spacing;
        content.sizeDelta = new Vector2(totalWidth, viewportHeight);

        //reset for scroll
        if (scrollRect != null)
            scrollRect.horizontalNormalizedPosition = 0f;
    }
}
