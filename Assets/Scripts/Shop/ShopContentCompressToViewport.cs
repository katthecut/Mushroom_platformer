using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(GridLayoutGroup))]
public class ShopContentCompressToViewport : MonoBehaviour
{
    //references
    [SerializeField] private RectTransform viewport; // ScrollView/Viewport
    [SerializeField] private RectTransform effectiveWidthSource;

    //grid layout
    [Min(1)] public int columns = 2;
    public float cellHeight = 320f;

    //spacing and padding
    public Vector2 spacing = new Vector2(10f, 10f);
    [Min(0)] public int paddingLeft = 10;
    [Min(0)] public int paddingRight = 10;
    [Min(0)] public int paddingTop = 10;
    [Min(0)] public int paddingBottom = 10;

    [Min(0f)] public float tightenX = 0f;
    [Min(0f)] public float tightenSpacingX = 0f;
    [Min(0f)] public float contentMaxWidthPx = 0f;

    [Min(1f)] public float minCellWidth = 120f;

    //behavior
    public bool autoRebuildOnEnable = true;
    public bool logDebug = false;

    private RectTransform content;
    private GridLayoutGroup grid;

    private void Awake()
    {
        content = GetComponent<RectTransform>();
        grid = GetComponent<GridLayoutGroup>();

        // Auto-find viewport via ScrollRect if not set
        if (viewport == null)
        {
            var sr = GetComponentInParent<ScrollRect>();
            if (sr != null && sr.viewport != null)
                viewport = sr.viewport;
        }

        ApplyGridSettings();
        ForceContentToViewportWidth();
        RecalculateCellSizeToFit();
    }

    private void OnEnable()
    {
        if (!autoRebuildOnEnable) return;

        ForceContentToViewportWidth();
        RecalculateCellSizeToFit();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!isActiveAndEnabled) return;

        ForceContentToViewportWidth();
        RecalculateCellSizeToFit();
    }

    public void Refresh()
    {
        ForceContentToViewportWidth();
        RecalculateCellSizeToFit();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private void ApplyGridSettings()
    {
        grid.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);

        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = Mathf.Max(1, columns);
    }

    private void ForceContentToViewportWidth()
    {
        if (viewport == null) return;

        // Content stretches to width of viewport
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);

        content.anchoredPosition = new Vector2(0f, content.anchoredPosition.y);
        content.sizeDelta = new Vector2(0f, content.sizeDelta.y);
    }

    private float GetUsableWidth()
    {
        RectTransform source = effectiveWidthSource != null ? effectiveWidthSource : viewport;
        if (source == null) return 0f;

        float w = source.rect.width;

        if (contentMaxWidthPx > 0f)
            w = Mathf.Min(w, contentMaxWidthPx);

        return w;
    }

    private void RecalculateCellSizeToFit()
    {
        float usableWidth = GetUsableWidth();
        if (usableWidth <= 0f) return;

        //applies updated padding from inspector
        grid.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);

        // Apply spacing with manual tightening
        float finalSpacingX = Mathf.Max(0f, spacing.x - tightenSpacingX);
        grid.spacing = new Vector2(finalSpacingX, spacing.y);

        int col = Mathf.Max(1, columns);
        float totalPadding = paddingLeft + paddingRight;
        float totalSpacing = finalSpacingX * (col - 1);

        float available = usableWidth - totalPadding - totalSpacing;
        float computedCellWidth = available / col;

        //manual tightening, shrink cell width more
        computedCellWidth -= tightenX;

        //clamp
        computedCellWidth = Mathf.Max(minCellWidth, computedCellWidth);

        // If it still overflows (because minCellWidth too big), reduce columns
        float required = computedCellWidth * col + totalPadding + totalSpacing;
        if (required > usableWidth)
        {
            for (int c = col; c >= 1; c--)
            {
                float sp = finalSpacingX * (c - 1);
                float avail = usableWidth - totalPadding - sp;
                float w = (avail / c) - tightenX;

                w = Mathf.Max(minCellWidth, w);

                if (w * c + totalPadding + sp <= usableWidth || c == 1)
                {
                    columns = c;
                    grid.constraintCount = columns;
                    computedCellWidth = w;
                    break;
                }
            }
        }

        grid.cellSize = new Vector2(computedCellWidth, cellHeight);

        if (logDebug)
        {
            Debug.Log(
                $"[ShopContentCompressToViewport] usableWidth={usableWidth:F1} cols={columns} " +
                $"cellWidth={computedCellWidth:F1} spacingX={finalSpacingX:F1} paddingLR={totalPadding}"
            );
        }
    }
}
