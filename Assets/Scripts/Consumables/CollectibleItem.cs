using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollectibleItem : MonoBehaviour
{
    // ID za save
    public string collectibleId;

    //za currency
    public int value = 1;

    public bool persistCollected = false;
    public bool destroyAfterCollect = true;

    private const string PREF_PREFIX = "COLLECTED_";

    private PickupAudio pickupSound;

    private void Awake()
    {
        pickupSound = GetComponent<PickupAudio>();

        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // Kreira ID ako ga vec nema
        if (string.IsNullOrWhiteSpace(collectibleId))
        {
            Vector3 p = transform.position;

            collectibleId =
                $"{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}_" +
                $"{Mathf.RoundToInt(p.x * 10f)}_" +
                $"{Mathf.RoundToInt(p.y * 10f)}";
        }

        if (persistCollected)
        {
            if (PlayerPrefs.GetInt(PREF_PREFIX + collectibleId, 0) == 1)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Play pickup audio
        if (pickupSound != null)
        {
            pickupSound.MarkCollected();
        }

        // Tell manager the leaf was collected
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.Collect(this);
        }
        else
        {
            Debug.LogWarning(
                "CollectibleItem: No CollectibleManager in scene."
            );

            return;
        }

        // Destroy leaf
        if (destroyAfterCollect)
        {
            Destroy(gameObject);
        }
    }

    public void MarkPersisted()
    {
        if (!persistCollected)
            return;

        PlayerPrefs.SetInt(
            PREF_PREFIX + collectibleId,
            1
        );

        PlayerPrefs.Save();
    }
}
