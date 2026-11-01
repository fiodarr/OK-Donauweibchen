using System.Collections;
using UnityEngine;

public class FlashController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;

    private Coroutine flashCoroutine;

    private static readonly int FlashAmount =
        Shader.PropertyToID("_Amount");

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    public void SetFlash(float amount)
    {
        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(FlashAmount, amount);
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }

    public void FlashOnce(float duration, float brightness)
    {
        StartFlash(FlashOnceCoroutine(duration,brightness));
    }

    public void FlashMultiple(int count, float duration, float interval, float brightness)
    {
        StartFlash(FlashMultipleCoroutine(count, duration, interval, brightness));
    }

    private void StartFlash(IEnumerator coroutine)
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(coroutine);
    }

    private IEnumerator FlashOnceCoroutine(float duration, float brightness)
    {
        SetFlash(brightness);

        yield return new WaitForSeconds(duration);

        SetFlash(0f);
        flashCoroutine = null;
    }

    private IEnumerator FlashMultipleCoroutine(
        int count, float duration, float interval, float brightness)
    {
        for (int i = 0; i < count; i++)
        {
            SetFlash(brightness);

            yield return new WaitForSeconds(duration);

            SetFlash(0f);

            if (i < count - 1)
                yield return new WaitForSeconds(interval);
        }

        flashCoroutine = null;
    }

    private void OnDisable()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        SetFlash(0f);
    }
}