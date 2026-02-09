using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ShopManualRowLayout : MonoBehaviour
{
    //references
    [SerializeField] private RectTransform viewport; // ScrollView/Viewport
    [SerializeField] private RectTransform content;

    //layout
    [Min(0f)] public float leftPadding = 20f;
    [Min(0f)] public float topPadding = 20f;
    [Min(0f)] public float itemGapX = 20f;
    [Min(0f)] public float itemGapY = 20f;

    //item size
    public Vector2 itemSize = new Vector2(260f, 320f);

    //rows
    [Min(1)] public int maxColumns = 2;
    //2 items per row is default

    //behavior
    public bool autoSizeContentHeight = true;
    public bool autoFindViewport = true;

    private void Reset()
    {
        content = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        if (content == null) content = GetComponent<RectTransform>();

        if (autoFindViewport && viewport == null)
        {
            // try find Viewport above
            Transform t = transform.parent;
            while (t != null)
            {
                if (t.name.ToLower().Contains("viewport"))
                {
                    viewport = t.GetComponent<RectTransform>();
                    break;
                }
                t = t.parent;
            }
        }
    }

    public void LayoutNow()
    {
        if (viewport == null || content == null) return;

        //stretch content
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = new Vector2(0f, content.anchoredPosition.y);
        content.sizeDelta = new Vector2(0f, content.sizeDelta.y);

        int count = 0;

        float x = leftPadding;
        float y = -topPadding;

        int col = 0;
        int row = 0;

        float viewportWidth = viewport.rect.width;
        float usableWidth = viewportWidth - leftPadding * 2f;

        // If maxColumns too high for the viewport, reduce it automatically
        float neededForCols = maxColumns * itemSize.x + (maxColumns - 1) * itemGapX;
        if (neededForCols > usableWidth)
        {
            maxColumns = Mathf.Max(1, Mathf.FloorToInt((usableWidth + itemGapX) / (itemSize.x + itemGapX)));
        }

        List<RectTransform> items = new List<RectTransform>();

        for (int i = 0; i < content.childCount; i++)
        {
            var rt = content.GetChild(i) as RectTransform;
            if (rt == null) continue;

            if (!rt.gameObject.activeSelf) continue;

            items.Add(rt);
        }

        for (int i = 0; i < items.Count; i++)
        {
            RectTransform item = items[i];

            item.anchorMin = new Vector2(0f, 1f);
            item.anchorMax = new Vector2(0f, 1f);
            item.pivot = new Vector2(0f, 1f);

            item.sizeDelta = itemSize;

            col = count % maxColumns;
            row = count / maxColumns;

            float px = leftPadding + col * (itemSize.x + itemGapX);
            float py = -topPadding - row * (itemSize.y + itemGapY);

            item.anchoredPosition = new Vector2(px, py);

            count++;
        }

        if (autoSizeContentHeight)
        {
            int rows = Mathf.CeilToInt((items.Count) / (float)maxColumns);
            float totalHeight = topPadding + rows * itemSize.y + Mathf.Max(0, rows - 1) * itemGapY + topPadding;

            content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
        }
    }
    private void OnEnable()
    {
        LayoutNow();
    }

#if UNITY_EDITOR
    private void Update()
    {
        // Live update in editor when tweaking numbers
        if (!Application.isPlaying)
            LayoutNow();
    }
#endif
}
