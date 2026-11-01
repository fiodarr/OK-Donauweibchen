using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private Transform target;
    private PlayerInputHandler playerInput;
    [SerializeField] private float followSpeed = 5f, directionOffset = 5f;

    void Start()
    {
        target = player.transform;
        playerInput = player.GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        Vector2 targetPosition = (new Vector2(
            target.position.x,
            target.position.y
        ) + playerInput.MoveInput * directionOffset);
        
        Vector3 targetPosition3D = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition3D,
            followSpeed * Time.deltaTime
        );
    }
}