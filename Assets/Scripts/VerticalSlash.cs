using UnityEngine;

public class VerticalSlash : Slash
{
    [SerializeField] private float directionOffset = 0.5f;

    public override void DoSlash(Vector2 direction)
    {
        spriteRenderer.flipX = direction.x > 0;

        transform.localPosition = (Vector3)(direction.normalized * directionOffset);

        base.DoSlash(direction);
    }
}