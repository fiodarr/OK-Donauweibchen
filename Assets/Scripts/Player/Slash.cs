using UnityEngine;
using System.Collections;

public class Slash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int frontOrder = 99;
    [SerializeField] private int backOrder = -99;

    private Coroutine slashCoroutine;

    private void Awake()
    {
        spriteRenderer.enabled = false;
    }

    public void DoSlash()
    {
        float angle = Mathf.Repeat(transform.eulerAngles.z, 360f);

        spriteRenderer.sortingOrder =
            (angle > 180f && angle < 360f) ? frontOrder : backOrder;

        if (slashCoroutine != null)
            StopCoroutine(slashCoroutine);

        slashCoroutine = StartCoroutine(ShowSlash());
    }

    private IEnumerator ShowSlash()
    {
        spriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.enabled = false;
        slashCoroutine = null;
    }
}