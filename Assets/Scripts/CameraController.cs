using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerComponents playerComponents;
    private Transform target;
    [SerializeField] private float followSpeed = 5f, directionOffset = 5f;

    void Start()
    {
        target = playerComponents.transform;
    }

    void Update()
    {

        Vector2 moveOffset = playerComponents.inputHandler.MoveInput * directionOffset;
        if (playerComponents.status.movementLocked)
        {
            moveOffset = Vector2.zero;
        }
        Vector2 targetPosition = (new Vector2(
            target.position.x,
            target.position.y
        ) + moveOffset);

        Vector3 targetPosition3D = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition3D,
            followSpeed * Time.deltaTime
        );
    }
}