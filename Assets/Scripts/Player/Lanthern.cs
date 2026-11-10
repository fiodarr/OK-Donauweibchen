
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lantern : MonoBehaviour
{
    [Header("Lantern Settings")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private float healAmount = 40f;

    [Header("Light")]
    [SerializeField] private Light2D lanternLight;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float globalFadeDuration = 3f;

    [Header("Hit Blink")]
    [SerializeField] private int blinkCount = 1;
    [SerializeField] private float blinkInterval = 0.08f;

    [SerializeField] private float blinkFadeDuration = 0.08f;
    private int currentCharges;
    private float originalIntensity;
    private float originalGlobalIntensity;

    private Coroutine fadeCoroutine;
    private Coroutine blinkCoroutine;

    private PlayerComponents playerComponents;

    public int CurrentCharges => currentCharges;
    public int MaxCharges => maxCharges;

    private void Awake()
    {
        playerComponents = GetComponentInParent<PlayerComponents>();

        currentCharges = maxCharges;

        if (lanternLight != null)
            originalIntensity = lanternLight.intensity;

        if (globalLight != null)
            originalGlobalIntensity = globalLight.intensity;
    }

    private void OnEnable()
    {
        if (playerComponents != null)
            playerComponents.inputHandler.Heal += Use;
    }

    private void OnDisable()
    {
        if (playerComponents != null)
            playerComponents.inputHandler.Heal -= Use;
    }

    public void Use()
    {
        if (currentCharges <= 0)
        {
            Debug.Log("Lantern is empty!");
            return;
        }

        PlayerStatus status = playerComponents.status;

        if (status.IsDead || status.GetHealth() >= status.GetMaxHealth())
            return;

        status.Heal(healAmount);
        Debug.Log("healed");
        currentCharges--;
    }

    public void Refill()
    {
        currentCharges = maxCharges;
    }

    public void Blink()
    {

        if (blinkCoroutine != null)
            return;

        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            float elapsed = 0f;

            float startLantern = lanternLight != null ? lanternLight.intensity : 0f;
            float startGlobal = globalLight != null ? globalLight.intensity : 0f;

            while (elapsed < blinkFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / blinkFadeDuration);
                t = Mathf.SmoothStep(0f, 1f, t);

                if (lanternLight != null)
                    lanternLight.intensity = Mathf.Lerp(startLantern, originalIntensity * 0.3f, t);

                if (globalLight != null)
                    globalLight.intensity = Mathf.Lerp(startGlobal, originalGlobalIntensity * 0.6f, t);

                yield return null;
            }

            elapsed = 0f;

            while (elapsed < blinkFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / blinkFadeDuration);
                t = Mathf.SmoothStep(0f, 1f, t);

                if (lanternLight != null)
                    lanternLight.intensity = Mathf.Lerp(originalIntensity * 0.3f, originalIntensity, t);

                if (globalLight != null)
                    globalLight.intensity = Mathf.Lerp(originalGlobalIntensity * 0.6f, originalGlobalIntensity, t);

                yield return null;
            }
        }

        if (lanternLight != null)
            lanternLight.intensity = originalIntensity;

        if (globalLight != null)
            globalLight.intensity = originalGlobalIntensity;

        blinkCoroutine = null;
    }

    public void FadeOut()
    {
        StopBlink();

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeLight());
    }

    private IEnumerator FadeLight()
    {
        float elapsed = 0f;

        float startLantern = lanternLight != null
            ? lanternLight.intensity : 0f;

        float startGlobal = globalLight != null
            ? globalLight.intensity : 0f;

        float duration = Mathf.Max(fadeDuration, globalFadeDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (lanternLight != null)
            {
                float t = fadeDuration > 0f
                    ? Mathf.Clamp01(elapsed / fadeDuration) : 1f;

                lanternLight.intensity = Mathf.Lerp(startLantern, 0f, t);
            }

            if (globalLight != null)
            {
                float t = globalFadeDuration > 0f
                    ? Mathf.Clamp01(elapsed / globalFadeDuration) : 1f;

                globalLight.intensity = Mathf.Lerp(startGlobal, 0f, t);
            }

            yield return null;
        }

        if (lanternLight != null)
            lanternLight.intensity = 0f;

        if (globalLight != null)
            globalLight.intensity = 0f;

        fadeCoroutine = null;
    }

    public void RestoreLight()
    {
        StopBlink();

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (lanternLight != null)
            lanternLight.intensity = originalIntensity;

        if (globalLight != null)
            globalLight.intensity = originalGlobalIntensity;
    }

    private void StopBlink()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }
}
