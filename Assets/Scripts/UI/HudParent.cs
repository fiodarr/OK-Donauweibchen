using UnityEngine;

public class HudParent: MonoBehaviour
{
    [SerializeField] private GameObject[] hudObjects;
    public static HudParent Instance {get; private set;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("HudParent: Duplicate instance of HudParent in: " + gameObject.name);
            Destroy(this);
        }
    }

    public void ShowHud()
    {
        foreach (var obj in hudObjects)
        {
            obj.SetActive(true);
        }
    }

    public void HideHud()
    {
        foreach (var obj in hudObjects)
        {
            obj.SetActive(false);
        }
    }
}
