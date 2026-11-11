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

        Debug.Log($"Parser instance: {this.GetInstanceID()}");
        Debug.Log($"Assigned handler: {inputHandler}");
    }
    
    public void Parse(Popup popup, TextAsset textAsset)
    {
        //Go through whole txt file
        //<ActionName> at the beginning (if there is such) is the closeAction to close the popup
        //regular text is being collected
        //then each [ActionName] is replaced by the action name for corresponding ActionData.ButtonName
        string content = textAsset.text;

        Match closeMatch = Regex.Match(content, @"^\s*<([^<>]+)>");

        if (closeMatch.Success)
        {
            string closeActionName = closeMatch.Groups[1].Value;

            InputReferenceData closeData = GetActionData(closeActionName);

            if (closeData != null)
            {
                popup.SetPopupCloseButton(closeData.actionReference);
            }
            else
            {
                Debug.LogWarning($"Unknown popup close action: {closeActionName}");
                popup.SetPopupCloseButton(null);
            }

            content = content.Substring(closeMatch.Length);
        }
        else
        {
            popup.SetPopupCloseButton(null);
        }

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
        Debug.Log($"Parser instance: {this.GetInstanceID()}");
        Debug.Log($"Components: {components}");
        Debug.Log($"Handler: {inputHandler}");
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
                        actionName = "Triangle";
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
                        actionName = "L2";
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
                        actionName = "R2";
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
                    default:
                        actionName = "<Unrecognized Control Scheme>";
                        break;
                }
                return new InputReferenceData(inputHandler.GetInputActions().Player.Heal, actionName);
            case "Dodge":
                switch (inputHandler.GetControlScheme())
                {
                    case PlayerInputHandler.ControlScheme.KeyboardMouse:
                        actionName = "Alt";
                        break;
                    case PlayerInputHandler.ControlScheme.Controller:
                        actionName = "X";
                        break;
                    default:
                        actionName = "<Unrecognized Control Scheme>";
                        break;
                }
                return new InputReferenceData(inputHandler.GetInputActions().Player.Evade, actionName);
            default:
                return null;
        }
    }
}
