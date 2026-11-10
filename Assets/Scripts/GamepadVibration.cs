using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadVibration : MonoBehaviour
{
    public static GamepadVibration Instance { get; private set; }

    private Coroutine vibrationRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Vibrate(float intensity, float duration)
    {
        if (Gamepad.current == null)
            return;

        if (vibrationRoutine != null)
            StopCoroutine(vibrationRoutine);

        vibrationRoutine = StartCoroutine(VibrationRoutine(intensity, duration));
    }

    private IEnumerator VibrationRoutine(float intensity, float duration)
    {
        Gamepad.current.SetMotorSpeeds(intensity, intensity);

        yield return new WaitForSecondsRealtime(duration);

        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0f, 0f);

        vibrationRoutine = null;
    }

    private void OnDisable()
    {
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0f, 0f);
    }
}