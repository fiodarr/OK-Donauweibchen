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

    private int currentCharges;
    private float originalIntensity;
    private float originalGlobalIntensity;
    private Coroutine fadeCoroutine;

    public int CurrentCharges => currentCharges;
    public int MaxCharges => maxCharges;

    private PlayerComponents playerComponents;

    private void OnEnable()
    {
        playerComponents.inputHandler.Heal += Use;
    }

    private void OnDisable()
    {
        playerComponents.inputHandler.Heal -= Use;
    }

    private void Awake()
    {
        currentCharges = maxCharges;
        playerComponents = GetComponentInParent<PlayerComponents>();

        if (lanternLight != null)
            originalIntensity = lanternLight.intensity;

        if (globalLight != null)
            originalGlobalIntensity = globalLight.intensity;
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

    public void FadeOut()
    {
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

        while (elapsed < Mathf.Max(fadeDuration, globalFadeDuration))
        {
            elapsed += Time.deltaTime;

            if (lanternLight != null)
            {
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                lanternLight.intensity = Mathf.Lerp(startLantern, 0f, t);
            }

            if (globalLight != null)
            {
                float t = Mathf.Clamp01(elapsed / globalFadeDuration);
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
}