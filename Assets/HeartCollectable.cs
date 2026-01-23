using UnityEngine;

public class HeartCollectable : Consumable
{
    public void Pickup(Collider2D collider)
    {
        GameObject player = collider.gameObject;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;
        playerHealth.AddHealth(50f);
        Destroy(gameObject);
    }
}
