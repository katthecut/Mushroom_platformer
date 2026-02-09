using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ShopContentLayout : MonoBehaviour
{
    //layout settings
    public Vector2 cellSize = new Vector2(260, 320);
    public Vector2 spacing = new Vector2(15, 15);
    public RectOffset padding = new RectOffset(10, 10, 10, 10);
    public int columns = 2;

    private void Awake()
    {
        var grid = GetComponent<GridLayoutGroup>();

        grid.cellSize = cellSize;
        grid.spacing = spacing;
        grid.padding = padding;

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = Mathf.Max(1, columns);

        grid.childAlignment = TextAnchor.UpperLeft;
    }
}
