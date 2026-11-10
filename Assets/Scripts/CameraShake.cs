using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private bool useUnscaledTime = true;

    private Coroutine shakeRoutine;

    private float currentDuration;
    private float currentMagnitude;

    private Vector3 shakeOffset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void LateUpdate()
    {
        transform.position += shakeOffset;
    }

    public void Shake(float duration, float magnitude)
    {
        currentDuration = Mathf.Max(currentDuration, duration);
        currentMagnitude = Mathf.Max(currentMagnitude, magnitude);

        if (shakeRoutine == null)
            shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < currentDuration)
        {
            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float y = Random.Range(-1f, 1f) * currentMagnitude;

            shakeOffset = new Vector3(x, y, 0f);

            elapsed += useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            yield return null;
        }

        shakeOffset = Vector3.zero;

        currentDuration = 0f;
        currentMagnitude = 0f;
        shakeRoutine = null;
    }

    private void OnDisable()
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }

        shakeOffset = Vector3.zero;
        currentDuration = 0f;
        currentMagnitude = 0f;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}