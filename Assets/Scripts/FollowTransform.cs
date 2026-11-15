
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
        transform.rotation = target.rotation;
    }
}
