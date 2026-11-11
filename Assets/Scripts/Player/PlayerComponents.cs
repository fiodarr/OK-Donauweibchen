using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerComponents : MonoBehaviour
{
    public PlayerInputHandler inputHandler;
    public PlayerInteractor interactor;
    public PlayerStatus status;
    public PlayerCombat combat;
    public PlayerController controller;
    public Lantern lantern;
    public CameraController cameraController;

    public void PickUpLantern()
    {
        lantern.enabled = true;
        lantern.PickUp();
    }
}