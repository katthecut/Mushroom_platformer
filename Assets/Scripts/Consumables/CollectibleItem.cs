using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollectibleItem : MonoBehaviour
{
    //id for saving
    public string collectibleId;

    //how musch pickup adds to total
    public int value = 1;

    //collectible wont spawn again after collecting it (only if true)
    public bool persistCollected = false;

    //collectible destroyed after collection (only if true) for audio
    public bool destroyAfterCollect = true;

    private const string PREF_PREFIX = "COLLECTED_";

    private PickupPlaySoundOnRemove pickupSound;

    private void Awake()
    {
        pickupSound = GetComponent<PickupPlaySoundOnRemove>();

        // Ensure trigger collider
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        if (string.IsNullOrWhiteSpace(collectibleId))
        {
            var p = transform.position;
            collectibleId =
                $"{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}_{Mathf.RoundToInt(p.x * 10f)}_{Mathf.RoundToInt(p.y * 10f)}";
        }

        if (persistCollected)
        {
            //ako je collected jos od prije unisti se bez zvuka
            if (PlayerPrefs.GetInt(PREF_PREFIX + collectibleId, 0) == 1)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        //mark collected, sound plays when removed
        pickupSound?.MarkCollected();

        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.Collect(this);
        }
        else
        {
            Debug.LogWarning("CollectibleItem: No CollectibleManager in scene.");
        }

        MarkPersisted();

        if (destroyAfterCollect)
            Destroy(gameObject);
    }

    public void MarkPersisted()
    {
        if (!persistCollected) return;
        PlayerPrefs.SetInt(PREF_PREFIX + collectibleId, 1);
        PlayerPrefs.Save();
    }
}
