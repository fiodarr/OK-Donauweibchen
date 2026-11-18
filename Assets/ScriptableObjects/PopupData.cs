using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewPopupData", menuName = "PopupData")]
public class PopupData: ScriptableObject

{
    public TextAsset textAsset;
    public bool isPaused;
    public float timer;

    public InputActionReference closeInput;
    public String id;
}