using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInput : MonoBehaviour
{
    public void Pause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (PauseManager.Instance != null)
            PauseManager.Instance.TogglePause();
    }
}
