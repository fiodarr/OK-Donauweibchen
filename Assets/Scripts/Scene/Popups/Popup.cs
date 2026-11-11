using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Popup: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private PopupParser parser;
    private bool isPaused = false;
    
    private InputAction closeAction;
    private string popupText;
    
    private void Update()
    {
        if (closeAction == null) return;
        if (closeAction.WasPressedThisFrame() && gameObject.activeInHierarchy)
        {
            ClosePopup();
            PopupManager.Instance.playerInputHandler.CallUpdate();
        }
    }
    
    public void OpenPopup(TextAsset textAsset, bool pauseGame)
    {
        parser.Parse(this, textAsset);
        gameObject.SetActive(true);
        if (pauseGame)
        {
            GameManager.Instance.SetState(GameState.Popup);
            isPaused = true;
        }
    }

    public void OpenTimedPopup(TextAsset textAsset, bool pauseGame, float timer)
    {
        OpenPopup(textAsset, pauseGame);
        StartCoroutine(CloseTimedPopup(timer));
    }

    private IEnumerator CloseTimedPopup(float timer)
    {
        yield return new WaitForSeconds(timer);
        ClosePopup();
    }
    
    private void ClosePopup()
    {
        gameObject.SetActive(false);
        if (isPaused)
        {
            GameManager.Instance.SetState(GameState.Gameplay);
            isPaused = false;
        }
    }

    public void SetPopupText(string newText)
    {
        popupText = newText;
        text.text = popupText;
    }

    public void SetPopupCloseButton(InputAction newCloseAction)
    {
        closeAction = newCloseAction;
        
    }
}
