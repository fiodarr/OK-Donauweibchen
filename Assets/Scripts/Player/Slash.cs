using UnityEngine;
using System.Collections;

public class Slash : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] private int frontOrder = 99;
    [SerializeField] private int backOrder = -99;

    private Coroutine slashCoroutine;

    private void Awake()
    {
        spriteRenderer.enabled = false;
    }

    public virtual void DoSlash(Vector2 direction)
    {
        spriteRenderer.sortingOrder =
            direction.y > 0 ? backOrder : frontOrder;

        if (slashCoroutine != null)
            StopCoroutine(slashCoroutine);

        slashCoroutine = StartCoroutine(ShowSlash());
    }

    protected virtual IEnumerator ShowSlash()
    {
        spriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.enabled = false;
        slashCoroutine = null;
    }
}