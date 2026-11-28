using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopupParser: MonoBehaviour
{
    [SerializeField] private PlayerComponents components;
    private PlayerInputHandler inputHandler;

    private void Awake()
    {
        inputHandler = components.inputHandler;
    }
    
    public void Parse(PopupUI popup, TextAsset textAsset)
    {
        //Go through whole txt file
        //<ActionName> at the beginning (if there is such) is the closeAction to close the popup
        //regular text is being collected
        //then each [ActionName] is replaced by the action name for corresponding ActionData.ButtonName
        string content = textAsset.text;

        content = Regex.Replace(content, @"\[([^\[\]]+)\]", match =>
        {
            string actionName = match.Groups[1].Value;

            InputReferenceData actionData = GetActionData(actionName);

            if (actionData == null)
            {
                Debug.LogWarning($"Unknown input action: {actionName}");
                return match.Value;
            }

            return actionData.buttonName;
        });

        popup.SetPopupText(content);
    }

    private InputReferenceData GetActionData(string input)
    {
        //TODO: Interchange between names for PS, Xbox, or PC input
        string actionName;
        switch (input)
        {
            case "Attack":
                switch (inputHandler.GetControlScheme())
                {
                    case PlayerInputHandler.ControlScheme.KeyboardMouse:
                        actionName = "LMB";
                        break;
                    case PlayerInputHandler.ControlScheme.Controller:
                        actionName = "Square";
                        break;
                    case PlayerInputHandler.ControlScheme.XboxController:
                        actionName = "X";
                        break;
                    default:
                        actionName = "<Unrecognized Control Scheme>";
                        break;
                }
                return new InputReferenceData(inputHandler.GetInputActions().Player.Attack, actionName);
            case "Interact":
                switch (inputHandler.GetControlScheme())
                {
                    case PlayerInputHandler.ControlScheme.KeyboardMouse:
                        actionName = "E";
                        break;
                    case PlayerInputHandler.ControlScheme.Controller:
                        actionName = "Cross";
                        break;
                    case PlayerInputHandler.ControlScheme.XboxController:
                        actionName = "A";
                        break;
                    default:
                        actionName = "<Unrecognized Control Scheme>";
                        break;
                }
                return new InputReferenceData(inputHandler.GetInputActions().Player.Interact, actionName);
            case "Utility":
                switch (inputHandler.GetControlScheme())
                {
                    case PlayerInputHandler.ControlScheme.KeyboardMouse:
                        actionName = "RMB";
                        break;
                    case PlayerInputHandler.ControlScheme.Controller:
                        actionName = "Right Stick";
                        break;
                    case PlayerInputHandler.ControlScheme.XboxController:
                        actionName = "Right Stick";
                        break;
                    default:
                        actionName = "<Unrecognized Control Scheme>";
                        break;
                }
                return new InputReferenceData(inputHandler.GetInputActions().Player.UtilityAttack, actionName);
            case "Kick":
                switch (inputHandler.GetControlScheme())
                {
                    case PlayerInputHandler.ControlScheme.KeyboardMouse:
                        actionName = "Space";
                        break;
                    case PlayerInputHandler.ControlScheme.Controller:
                        actionName = "L2";
                        break;
                    case PlayerInputHandler.ControlScheme.XboxController:
                        actionName = "LT";
                        break;
                    default:
                        actionName = "<Unrecognized Control Scheme>";
                        break;
                }
                return new InputReferenceData(inputHandler.GetInputActions().Player.SecondaryAttack, actionName);
            case "Heal":
                switch (inputHandler.GetControlScheme())
                {
                    case PlayerInputHandler.ControlScheme.KeyboardMouse:
                        actionName = "Q";
                        break;
                    case PlayerInputHandler.ControlScheme.Controller:
                        actionName = "Circle";
                        break;
                    case PlayerInputHandler.ControlScheme.XboxController:
                        actionName = "B";
                        break;
                    default:
                        actionName = "<Unrecognized Control Scheme>";
                        break;
                }
                return new InputReferenceData(inputHandler.GetInputActions().Player.Heal, actionName);
            case "Dodge":
                switch (inputHandler.GetControlScheme())
                {
                    case PlayerInputHandler.ControlScheme.KeyboardMouse:
                        actionName = "Shift";
                        break;
                    case PlayerInputHandler.ControlScheme.Controller:
                        actionName = "R2";
                        break;
                    case PlayerInputHandler.ControlScheme.XboxController:
                        actionName = "RT";
                        break;
                    default:
                        actionName = "<Unrecognized Control Scheme>";
                        break;
                }
                return new InputReferenceData(inputHandler.GetInputActions().Player.Evade, actionName);
            case "UtilityStart":
                switch (inputHandler.GetControlScheme())
                {
                    case PlayerInputHandler.ControlScheme.KeyboardMouse:
                        actionName = "Hold RMB";
                        break;
                    case PlayerInputHandler.ControlScheme.Controller:
                        actionName = "Aim with Right Stick";
                        break;
                    case PlayerInputHandler.ControlScheme.XboxController:
                        actionName = "Aim with Right Stick";
                        break;
                    default:
                        actionName = "<Unrecognized Control Scheme>";
                        break;
                }
                return new InputReferenceData(inputHandler.GetInputActions().Player.UtilityAttack, actionName);
            case "UtilityRotation":
                switch (inputHandler.GetControlScheme())
                {
                    case PlayerInputHandler.ControlScheme.KeyboardMouse:
                        actionName = "Mouse";
                        break;
                    case PlayerInputHandler.ControlScheme.Controller:
                        actionName = "Right Stick";
                        break;
                    case PlayerInputHandler.ControlScheme.XboxController:
                        actionName = "Right Stick";
                        break;
                    default:
                        actionName = "<Unrecognized Control Scheme>";
                        break;
                }
                return new InputReferenceData(inputHandler.GetInputActions().Player.UtilityAttack, actionName);
            default:
                return null;
        }
    }
}