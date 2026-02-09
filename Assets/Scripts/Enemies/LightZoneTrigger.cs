using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LightZoneTrigger : MonoBehaviour
{
    //moths that should react to this LightZone
    public EnemyMoth[] moths;

    //Debug
    public bool debugLogs = false;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (debugLogs) Debug.Log("LightZone - Player ENTER");

        foreach (EnemyMoth moth in moths)
        {
            if (moth != null)
                moth.OnPlayerEnteredLightZone(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (debugLogs) Debug.Log("LightZone - Player EXIT");

        foreach (EnemyMoth moth in moths)
        {
            if (moth != null)
                moth.OnPlayerExitedLightZone(other.transform);
        }
    }
}