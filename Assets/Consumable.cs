using UnityEngine;
using UnityEngine.Events;

public class Consumable : MonoBehaviour
{
    public CollisionEvent collisionEvent;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            collisionEvent.Invoke(collider);
        }
    }
}
