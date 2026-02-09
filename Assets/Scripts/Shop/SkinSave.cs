using UnityEngine;

public static class SkinSave
{
    private const string UNLOCK_PREFIX = "SKIN_UNLOCKED_";
    private const string EQUIPPED_KEY = "SKIN_EQUIPPED";
    private const string PENDING_KEY = "SKIN_PENDING"; // apply after restart

    public static bool IsUnlocked(string skinId)
    {
        return PlayerPrefs.GetInt(UNLOCK_PREFIX + skinId, 0) == 1;
    }

    public static void Unlock(string skinId)
    {
        PlayerPrefs.SetInt(UNLOCK_PREFIX + skinId, 1);
        PlayerPrefs.Save();
    }

    public static string GetEquipped()
    {
        return PlayerPrefs.GetString(EQUIPPED_KEY, "");
    }

    public static void SetEquipped(string skinId)
    {
        PlayerPrefs.SetString(EQUIPPED_KEY, skinId);
        PlayerPrefs.Save();
    }

    public static string GetPending()
    {
        return PlayerPrefs.GetString(PENDING_KEY, "");
    }

    public static void SetPending(string skinId)
    {
        PlayerPrefs.SetString(PENDING_KEY, skinId);
        PlayerPrefs.Save();
    }

    public static void ClearPending()
    {
        PlayerPrefs.DeleteKey(PENDING_KEY);
        PlayerPrefs.Save();
    }
}
