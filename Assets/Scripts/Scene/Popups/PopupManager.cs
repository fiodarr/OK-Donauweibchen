using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopupManager: MonoBehaviour
{
    public PlayerInputHandler playerInputHandler;
    
    public static PopupManager Instance { get; private set; }
    [SerializeField] private PopupUI popupUI;
    private HashSet<String> shownPopups = new();
    private InputAction closeAction;

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
    private void Update()
    {
        if (closeAction == null) return;
        if (closeAction.WasPressedThisFrame() && gameObject.activeInHierarchy)
        {
            popupUI.ClosePopup();
            PopupManager.Instance.playerInputHandler.CallUpdate();
        }
    }

    public void TriggerPopup(PopupData data)
    {
        if (shownPopups.Contains(data.id)) return;

        shownPopups.Add(data.id);
        closeAction = data.closeInput;

        if (data.timer > 0f)
            popupUI.OpenTimedPopup(data.textAsset, data.isPaused, data.timer);
        else
            popupUI.OpenPopup(data.textAsset, data.isPaused);
    }
}
