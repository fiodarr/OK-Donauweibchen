using UnityEngine.InputSystem;

public class InputReferenceData
{
    public InputAction actionReference;
    public string buttonName;

    public InputReferenceData(InputAction actionReference, string buttonName)
    {
        this.actionReference = actionReference;
        this.buttonName = buttonName;
    }
}
