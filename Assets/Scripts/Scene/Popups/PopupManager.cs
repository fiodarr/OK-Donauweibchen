using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager: MonoBehaviour
{
    public PlayerInputHandler playerInputHandler;
    
    public static PopupManager Instance { get; private set; }

    [Serializable]
    public class PopupData
    {
        public PopupEvent popupEvent;
        public Popup popup;
        public TextAsset textAsset;
        public bool isPause;
        public float timer;
    }
    
    [SerializeField] private List<PopupData> popups;
    private HashSet<PopupEvent> shownPopups = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[PopupManager] Awake");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void TriggerPopup(PopupEvent popupEvent)
    {
        if (shownPopups.Contains(popupEvent)) return;

        PopupData data = popups.Find(
            p => p.popupEvent == popupEvent
        );

        if (data == null) return;

        shownPopups.Add(popupEvent);

        if (data.timer > 0f)
            data.popup.OpenTimedPopup(data.textAsset, data.isPause, data.timer);
        else
            data.popup.OpenPopup(data.textAsset, data.isPause);
    }
}
