using UnityEngine;

public class PickupId : MonoBehaviour
{
    [SerializeField] private string id = "pickup";

    [SerializeField] private bool useNameAsId = true;

    public string Id => Normalize(useNameAsId ? gameObject.name : id);

    private static string Normalize(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        return s.Replace("(Clone)", "").Trim();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (useNameAsId)
        {
        }
        else
        {
            id = Normalize(id);
        }
    }
#endif
}
