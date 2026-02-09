using UnityEngine;

public static class Currency
{
    public const string TOTAL_KEY = "TOTAL_COLLECTED";

    public static int GetTotal()
    {
        return PlayerPrefs.GetInt(TOTAL_KEY, 0);
    }

    public static void SetTotal(int value)
    {
        PlayerPrefs.SetInt(TOTAL_KEY, Mathf.Max(0, value));
        PlayerPrefs.Save();
    }

    public static bool CanAfford(int cost)
    {
        return GetTotal() >= cost;
    }

    public static bool Spend(int cost)
    {
        int total = GetTotal();
        if (total < cost) return false;

        SetTotal(total - cost);
        return true;
    }
}
