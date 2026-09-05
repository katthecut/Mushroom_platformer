using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private float flashDuration = 0.25f;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.75f);
    private int currentHealth;

    private SpriteRenderer enemyRenderer;
    private SpriteRenderer overlayRenderer;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        currentHealth = maxHealth;

        enemyRenderer = GetComponent<SpriteRenderer>();

        if (enemyRenderer != null)
        {
            CreateHitOverlay();
        }
    }

    private void CreateHitOverlay()
    {
        GameObject overlay = new GameObject("HitOverlay");

        overlay.transform.SetParent(transform);
        overlay.transform.localPosition = Vector3.zero;
        overlay.transform.localRotation = Quaternion.identity;
        overlay.transform.localScale = Vector3.one;

        overlayRenderer = overlay.AddComponent<SpriteRenderer>();

        //pocetni sprite
        overlayRenderer.sprite = enemyRenderer.sprite;

        //poluprozirna crvena 
        overlayRenderer.color = flashColor;

        //overlay iznad enemyja
        overlayRenderer.sortingLayerID = enemyRenderer.sortingLayerID;
        overlayRenderer.sortingOrder = enemyRenderer.sortingOrder + 1;

        //false ako nije dobio hit
        overlayRenderer.enabled = false;
    }

    private void LateUpdate()
    {
        if (enemyRenderer == null || overlayRenderer == null)
            return;

        // Ako enemy ima animaciju, overlay prati trenutni sprite
        overlayRenderer.sprite = enemyRenderer.sprite;

        //prati flip
        overlayRenderer.flipX = enemyRenderer.flipX;
        overlayRenderer.flipY = enemyRenderer.flipY;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        Debug.Log($"{gameObject.name} took {amount} damage. Health: {currentHealth}/{maxHealth}");

        FlashRed();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void FlashRed()
    {
        if (overlayRenderer == null)
            return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        overlayRenderer.enabled = true;

        yield return new WaitForSeconds(flashDuration);

        overlayRenderer.enabled = false;

        flashCoroutine = null;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}