using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerComponents : MonoBehaviour
{
    [NonSerialized] public PlayerInputHandler inputHandler;
    [NonSerialized] public PlayerInteractor interactor;
    [NonSerialized] public PlayerStatus status;
    [NonSerialized] public PlayerCombat combat;
    [NonSerialized] public PlayerController controller;

    [NonSerialized] public Lantern lantern;

    public CameraController cameraController;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        interactor = GetComponent<PlayerInteractor>();
        status = GetComponent<PlayerStatus>();
        combat = GetComponent<PlayerCombat>();
        controller = GetComponent<PlayerController>();
        lantern = GetComponentInChildren<Lantern>();
    }
}