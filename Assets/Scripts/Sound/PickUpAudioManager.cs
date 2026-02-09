using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Audio;

public class PickupAudioManager : MonoBehaviour
{
    [Serializable]
    public class PickupSound
    {
        public string pickupId;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;
    }

    //pickup sound
    public List<PickupSound> sounds = new List<PickupSound>();

    private readonly Dictionary<string, PickupSound> map = new Dictionary<string, PickupSound>(StringComparer.OrdinalIgnoreCase);

    private void Awake()
    {
        RebuildMap();
    }

    private void OnValidate()
    {
        RebuildMap();
    }

    private void RebuildMap()
    {
        map.Clear();

        for (int i = 0; i < sounds.Count; i++)
        {
            var e = sounds[i];
            if (e == null) continue;

            string key = Normalize(e.pickupId);
            if (string.IsNullOrEmpty(key)) continue;

            //last one wins if theres duplicates
            map[key] = e;
        }
    }

    private static string Normalize(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        return s.Replace("(Clone)", "").Trim();
    }

    public void PlayFor(string pickupId)
    {
        pickupId = Normalize(pickupId);
        if (string.IsNullOrEmpty(pickupId)) return;

        if (!map.TryGetValue(pickupId, out var entry) || entry == null)
        {
            // Optional: debug
            // Debug.LogWarning($"PickupAudioManager: No sound mapped for pickupId '{pickupId}'.");
            return;
        }

        if (entry.clip == null)
        {
            Debug.LogWarning($"PickupAudioManager: Clip missing for pickupId '{pickupId}'.");
            return;
        }

        GameAudioManager.Instance?.PlaySFX(entry.clip, Mathf.Clamp01(entry.volume));
    }

    public void NotifyPickedUp(GameObject pickupObject)
    {
        if (pickupObject == null) return;

        var id = pickupObject.GetComponent<PickupId>();
        if (id == null)
        {
            PlayFor(pickupObject.name);
            return;
        }

        PlayFor(id.Id);
    }
}