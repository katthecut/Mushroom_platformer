using UnityEngine;

public class HeartCollectable : Consumable
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PickupAudio audio = GetComponent<PickupAudio>();

        if (audio != null)
            audio.MarkCollected();

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        playerHealth.AddHealth(50f);
        Destroy(gameObject);
    }
}