using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopupUI: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private PopupParser parser;
    private bool isPaused = false;

    private string popupText;
    
    Coroutine timedPopupCoroutine = null;
    

    public void OpenPopup(TextAsset textAsset, bool pauseGame)
    {
        gameObject.SetActive(true);
        parser.Parse(this, textAsset);
        if (pauseGame)
        {
            GameManager.Instance.SetState(GameState.RestSpot);
            isPaused = true;
        }
    }

    public void OpenTimedPopup(TextAsset textAsset, bool pauseGame, float timer)
    {
        OpenPopup(textAsset, pauseGame);
        if (timedPopupCoroutine != null)
        {
            StopCoroutine(timedPopupCoroutine);
        }
        timedPopupCoroutine = StartCoroutine(CloseTimedPopup(timer));
    }

    private IEnumerator CloseTimedPopup(float timer)
    {
        yield return new WaitForSeconds(timer);
        ClosePopup();
    }
    
    public void ClosePopup()
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
}
