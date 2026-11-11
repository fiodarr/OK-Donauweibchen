using UnityEngine;

public class PopupTrigger: MonoBehaviour
{
    
    [SerializeField] protected PopupEvent eventToTrigger;
    

    protected void ShowPopup()
    {
        PopupManager.Instance.TriggerPopup(eventToTrigger);
    }
}
