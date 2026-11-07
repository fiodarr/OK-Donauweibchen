using UnityEngine;
using UnityEngine.InputSystem;

public class Popup: MonoBehaviour
{
    private InputActionReference closeAction;
    private string popupText;
    
    private void Update()
    {
        if (closeAction.action.WasPressedThisFrame())
        {
            ClosePopup();
        }
    }
    
    public void OpenPopup(TextAsset textAsset)
    {
        closeAction.action.Enable();
        gameObject.SetActive(true);
    }
    
    private void ClosePopup()
    {
        closeAction.action.Disable();
        gameObject.SetActive(false);
    }
}
