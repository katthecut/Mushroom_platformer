using UnityEngine;

public class DamageToPlayer : MonoBehaviour
{
    //dmg applied when player's cooldown allows it
    public float damagePerTick = 10f;

    //which colliders deal damage
    [SerializeField] private Collider2D[] damageColliders;

    public bool IsDamageCollider(Collider2D touched)
    {
        if (touched == null) return false;

        if (damageColliders == null || damageColliders.Length == 0)
            return touched.GetComponent<DamageToPlayer>() == this;

        for (int i = 0; i < damageColliders.Length; i++)
        {
            if (damageColliders[i] == touched)
                return true;
        }

        return false;
    }

}