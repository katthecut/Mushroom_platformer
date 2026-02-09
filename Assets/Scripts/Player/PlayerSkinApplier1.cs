using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class PlayerSkinApplier1 : MonoBehaviour
{
    [System.Serializable]
    public class SkinVisual
    {
        public string skinId;

        [Header("Static (no animations)")]
        public Sprite staticSprite;

        [Header("Animated (optional)")]
        [Tooltip("If set, this skin will use animations from this override controller.")]
        public AnimatorOverrideController overrideController;
    }

    [Header("Targets")]
    [Tooltip("If empty, auto-finds the first SpriteRenderer in children.")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Tooltip("If empty, auto-finds Animator on same object as renderer (or in parents).")]
    [SerializeField] private Animator targetAnimator;

    [Header("Base Animation Controller (default skin animations)")]
    [SerializeField] private RuntimeAnimatorController baseController;

    [Header("Skins")]
    [Tooltip("This id is used when nothing is equipped yet.")]
    [SerializeField] private string defaultSkinId = "default";
    [SerializeField] private SkinVisual[] skins;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private Dictionary<string, SkinVisual> map;

    // Static enforcing (prevents animator/other scripts from overriding)
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

        // Ensure the animator has a base controller for default animations
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
        // If this skin is static, enforce sprite every frame (Animator can overwrite during Update)
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
            Debug.LogError("[PlayerSkinApplier] No SpriteRenderer found. Assign Target Renderer or ensure it's on player/child.");
            return;
        }

        // Commit pending -> equipped after restart
        string pending = SkinSave.GetPending();
        if (!string.IsNullOrEmpty(pending) && SkinSave.IsUnlocked(pending))
        {
            SkinSave.SetEquipped(pending);
            SkinSave.ClearPending();

            if (debugLogs)
                Debug.Log($"[PlayerSkinApplier] Pending skin committed as equipped: {pending}");
        }

        // Ensure equipped exists
        string equipped = SkinSave.GetEquipped();
        if (string.IsNullOrEmpty(equipped))
        {
            SkinSave.SetEquipped(defaultSkinId);
            equipped = defaultSkinId;

            if (debugLogs)
                Debug.Log($"[PlayerSkinApplier] No equipped skin -> setting default: {defaultSkinId}");
        }

        ApplySkin(equipped);
    }

    public void ApplySkin(string skinId)
    {
        if (string.IsNullOrEmpty(skinId)) return;
        if (map == null || map.Count == 0) BuildMap();

        // Default skin: always animated using base controller
        if (skinId == defaultSkinId)
        {
            ApplyAnimatedController(baseController);
            if (debugLogs) Debug.Log($"[PlayerSkinApplier] Applied DEFAULT animated skin: {skinId}");
            return;
        }

        if (!map.TryGetValue(skinId, out SkinVisual visual))
        {
            if (debugLogs)
                Debug.LogWarning($"[PlayerSkinApplier] Skin '{skinId}' not found. Falling back to default.");

            ApplyAnimatedController(baseController);
            return;
        }

        // If override exists -> use animations from that override controller
        if (visual.overrideController != null)
        {
            ApplyAnimatedController(visual.overrideController);

            if (debugLogs)
                Debug.Log($"[PlayerSkinApplier] Applied OVERRIDE animations for skin: {skinId}");

            return;
        }

        // If no override -> apply static sprite WITHOUT animations
        if (visual.staticSprite != null)
        {
            ApplyStaticSprite(visual.staticSprite);

            if (debugLogs)
                Debug.Log($"[PlayerSkinApplier] Applied STATIC sprite (no animations) for skin: {skinId}");

            return;
        }

        // Nothing assigned -> fallback
        if (debugLogs)
            Debug.LogWarning($"[PlayerSkinApplier] Skin '{skinId}' has no overrideController/staticSprite. Falling back to default.");

        ApplyAnimatedController(baseController);
    }

    private void ApplyStaticSprite(Sprite sprite)
    {
        forceStatic = true;
        forcedSprite = sprite;

        // Freeze animator so it stops swapping sprites
        if (targetAnimator != null)
        {
            // Keep base controller but freeze playback so it can't overwrite the sprite
            if (baseController != null)
                targetAnimator.runtimeAnimatorController = baseController;

            targetAnimator.speed = 0f;
            targetAnimator.Rebind();
            targetAnimator.Update(0f);
        }

        targetRenderer.sprite = sprite;
    }

    private void ApplyAnimatedController(RuntimeAnimatorController controller)
    {
        // Stop forcing sprite
        forceStatic = false;
        forcedSprite = null;

        if (targetAnimator != null)
        {
            // Restore animation playback
            targetAnimator.speed = 1f;

            if (controller != null)
                targetAnimator.runtimeAnimatorController = controller;

            targetAnimator.Rebind();
            targetAnimator.Update(0f);
        }
    }
}
