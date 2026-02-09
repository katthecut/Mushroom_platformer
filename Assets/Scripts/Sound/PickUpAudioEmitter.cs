using UnityEngine;

[RequireComponent(typeof(PickupId))]
public class PickupAudioEmitter : MonoBehaviour
{
    public PickupAudioManager manager;

    private PickupId pickupId;

    private void Awake()
    {
        pickupId = GetComponent<PickupId>();

        if (manager == null)
            manager = Object.FindFirstObjectByType<PickupAudioManager>();
    }


    public void Collected()
    {
        if (manager == null)
            manager = Object.FindFirstObjectByType<PickupAudioManager>();

        if (manager == null) return;

        manager.PlayFor(pickupId != null ? pickupId.Id : gameObject.name);
    }
}