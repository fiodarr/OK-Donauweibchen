using TMPro;
using UnityEngine;


public class DescriptionBox: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI description;

    public void ChangeName(string newName)
    {
        name.text = newName;
    }
    
    public void ChangeDescription(string desc)
    {
        description.text = desc;
    }
}
