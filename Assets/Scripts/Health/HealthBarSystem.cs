using UnityEngine;
using UnityEngine.UI;

public class HealthBarSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    //Bar Size
    [SerializeField] private float width = 100f;
    [SerializeField] private float height = 10f;

    //References
    [SerializeField] private RectTransform healthBarFill;

    //health bar nece flippati sa igracem
    private bool preventFlipWithParent = true;

    private Vector3 baseLocalScale;
    private Transform parentTransform;


    [SerializeField]
    private RectTransform healthBar;

    private void Awake()
    {
        baseLocalScale = transform.localScale;
        parentTransform = transform.parent;
    }
    private void LateUpdate()
    {
        if (!preventFlipWithParent) return;
        if (parentTransform == null) return;

        //ako se igrac flippa, health bar nece
        float parentX = parentTransform.lossyScale.x;
        float sign = (parentX < 0f) ? -1f : 1f;

        Vector3 s = baseLocalScale;
        s.x = Mathf.Abs(baseLocalScale.x) * sign;
        s.y = Mathf.Abs(baseLocalScale.y);
        s.z = Mathf.Abs(baseLocalScale.z);
        transform.localScale = s;
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = Mathf.Max(1f, value);
    }

    public void SetHealth(float value)
    {
        if (healthBarFill == null)
        {
            Debug.LogWarning("HealthBarSystem: healthBarFill not assigned.");
            return;
        }

        float clamped = Mathf.Clamp(value, 0f, maxHealth);
        float percent = clamped / maxHealth;

        healthBarFill.sizeDelta = new Vector2(width * percent, height);
    }
}