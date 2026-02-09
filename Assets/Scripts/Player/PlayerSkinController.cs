using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinController : MonoBehaviour
{
    [System.Serializable]
    public class SkinVisual
    {
        public string skinId;
        public Sprite staticSprite;
        public AnimatorOverrideController overrideController;
    }

    [Header("Targets")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Animator targetAnimator;

    [Header("Base Animator Controller")]
    [SerializeField] private RuntimeAnimatorController baseController;

    [Header("Skins")]
    [SerializeField] private string defaultSkinId = "default";
    [SerializeField] private SkinVisual[] skins;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private Dictionary<string, SkinVisual> map;

    private bool forceStatic;
    private Sprite forcedSprite;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SpriteRenderer>(true);

        if (targetAnimator == null && targetRenderer != null)
        {
            targetAnimator = targetRenderer.GetComponent<Animator>();
            if (targetAnimator == null)
                targetAnimator = targetRenderer.GetComponentInParent<Animator>();
        }

        BuildMap();

        if (targetAnimator != null && baseController != null && targetAnimator.runtimeAnimatorController == null)
            targetAnimator.runtimeAnimatorController = baseController;
    }

    private void OnEnable()
    {
        ApplyFromSave();
    }

    private void Start()
    {
        ApplyFromSave();
    }

    private void LateUpdate()
    {
        if (!forceStatic) return;
        if (targetRenderer == null || forcedSprite == null) return;

        targetRenderer.sprite = forcedSprite;
    }

    private void BuildMap()
    {
        map = new Dictionary<string, SkinVisual>();
        if (skins == null) return;

        for (int i = 0; i < skins.Length; i++)
        {
            if (string.IsNullOrEmpty(skins[i].skinId)) continue;
            map[skins[i].skinId] = skins[i];
        }
    }

    private void ApplyFromSave()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("[PlayerSkinController] No SpriteRenderer found.");
            return;
        }

        string pending = SkinSave.GetPending();
        if (!string.IsNullOrEmpty(pending) && SkinSave.IsUnlocked(pending))
        {
            SkinSave.SetEquipped(pending);
            SkinSave.ClearPending();
            if (debugLogs) Debug.Log($"[PlayerSkinController] Pending committed: {pending}");
        }

        string equipped = SkinSave.GetEquipped();
        if (string.IsNullOrEmpty(equipped))
        {
            SkinSave.SetEquipped(defaultSkinId);
            equipped = defaultSkinId;
        }

        ApplySkin(equipped);
    }

    public void ApplySkin(string skinId)
    {
        if (string.IsNullOrEmpty(skinId)) return;
        if (map == null || map.Count == 0) BuildMap();

        // Default = animated
        if (skinId == defaultSkinId)
        {
            ApplyAnimated(baseController);
            if (debugLogs) Debug.Log($"[PlayerSkinController] Default applied: {skinId}");
            return;
        }

        if (!map.TryGetValue(skinId, out SkinVisual visual))
        {
            ApplyAnimated(baseController);
            return;
        }

        if (visual.overrideController != null)
        {
            ApplyAnimated(visual.overrideController);
            if (debugLogs) Debug.Log($"[PlayerSkinController] Override applied: {skinId}");
            return;
        }

        if (visual.staticSprite != null)
        {
            ApplyStatic(visual.staticSprite);
            if (debugLogs) Debug.Log($"[PlayerSkinController] Static applied: {skinId}");
            return;
        }

        ApplyAnimated(baseController);
    }

    private void ApplyStatic(Sprite sprite)
    {
        forceStatic = true;
        forcedSprite = sprite;

        if (targetAnimator != null)
        {
            if (baseController != null)
                targetAnimator.runtimeAnimatorController = baseController;

            targetAnimator.speed = 0f;
            targetAnimator.Rebind();
            targetAnimator.Update(0f);
        }

        targetRenderer.sprite = sprite;
    }

    private void ApplyAnimated(RuntimeAnimatorController controller)
    {
        forceStatic = false;
        forcedSprite = null;

        if (targetAnimator != null)
        {
            targetAnimator.speed = 1f;
            if (controller != null)
                targetAnimator.runtimeAnimatorController = controller;

            targetAnimator.Rebind();
            targetAnimator.Update(0f);
        }
    }
}
