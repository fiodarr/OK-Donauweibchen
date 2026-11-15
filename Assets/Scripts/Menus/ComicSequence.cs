using UnityEngine;

public class ComicSequence : MonoBehaviour
{
    [SerializeField] private GameObject[] comicPanels;
    [SerializeField] private PlayerInputHandler inputHandler;
    private float size;
    private float currentPanel;

    private void Start()
    {
        size = comicPanels.Length;
    }
    
    
}
