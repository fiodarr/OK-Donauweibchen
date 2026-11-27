
using UnityEngine;

public class CrabAnimationEvents : MonoBehaviour
{
    [SerializeField] private Crab crab;

    public void WindUpEnded()
    {
        crab.WindUpEnded();
    }

    public void ActivateHitbox()
    {
        crab.ActivateHitbox();
    }
    public void ActivateJumpHitbox()
    {
        crab.ActivateJumpHitbox();
    }

    public void DeactivateJumpHitbox()
    {
        crab.DeactivateJumpHitbox();
    }

    public void DeactivateHitbox()
    {
        crab.DeactivateHitbox();
    }

    public void AttackEnd()
    {
        crab.AttackEnd();
    }

    public void JumpEnded()
    {
        crab.JumpEnded();
    }
}
