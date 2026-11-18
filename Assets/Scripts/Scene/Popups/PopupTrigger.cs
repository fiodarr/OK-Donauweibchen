using UnityEngine;

public class PopupTrigger: MonoBehaviour
{
    
    [SerializeField] protected PopupData popupData;
    

    protected void ShowPopup()
    {
        PopupManager.Instance.TriggerPopup(popupData);
    }
}
