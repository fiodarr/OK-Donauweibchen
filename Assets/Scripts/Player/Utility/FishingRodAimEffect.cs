using UnityEngine;
using UnityEngine.UI;

public class FishingRodAimEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Transform player;
    [SerializeField] private Camera gameplayCamera;

    [SerializeField] private RawImage overlay;
    [SerializeField] private Material overlayMaterial;

    [Header("Effect Settings")]
    [SerializeField, Range(0f, 1f)] private float darkness = 0.35f;

    [SerializeField, Range(1f, 180f)] private float coneAngle = 50f;
    [SerializeField] private float coneRange = 6f;

    [SerializeField] private float edgeSoftness = 3f;
    [SerializeField] private float rangeEdgeSoftness = 0.35f;

    private CanvasGroup canvasGroup;
    private Material runtimeMaterial;

    private static readonly int OverlayColorID =
        Shader.PropertyToID("_OverlayColor");

    private static readonly int OriginID =
        Shader.PropertyToID("_OriginUV");

    private static readonly int AimDirectionID =
        Shader.PropertyToID("_AimDirection");

    private static readonly int ConeAngleID =
        Shader.PropertyToID("_ConeAngle");

    private static readonly int EdgeSoftnessID =
        Shader.PropertyToID("_EdgeSoftness");

    private static readonly int RangePixelsID =
        Shader.PropertyToID("_RangePixels");

    private static readonly int RangeSoftnessPixelsID =
        Shader.PropertyToID("_RangeSoftnessPixels");


    private void Awake()
    {
        if (gameplayCamera == null)
            gameplayCamera = Camera.main;

        if (inputHandler == null || player == null ||
            gameplayCamera == null || overlay == null ||
            overlayMaterial == null)
        {
            Debug.LogError("FishingRodAimEffect: Missing references!", this);
            enabled = false;
            return;
        }

        canvasGroup = overlay.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = overlay.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        overlay.texture = Texture2D.whiteTexture;
        overlay.color = Color.white;
        overlay.raycastTarget = false;

        // Create an independent material instance.
        runtimeMaterial = new Material(overlayMaterial);
        overlay.material = runtimeMaterial;

        runtimeMaterial.SetColor(
            OverlayColorID,
            new Color(0f, 0f, 0f, darkness)
        );
    }

    private void OnEnable()
    {
        if (inputHandler == null) return;

        inputHandler.UtilityPressed += StartEffect;
        inputHandler.UtilityReleased += StopEffect;
    }

    private void OnDisable()
    {
        if (inputHandler == null) return;

        inputHandler.UtilityPressed -= StartEffect;
        inputHandler.UtilityReleased -= StopEffect;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }

    private void LateUpdate()
    {
        if (canvasGroup == null || canvasGroup.alpha <= 0f)
            return;

        UpdateShader();
    }

    private void StartEffect()
    {
        canvasGroup.alpha = 1f;
        UpdateShader();
    }

    private void StopEffect()
    {
        canvasGroup.alpha = 0f;
    }

    private void UpdateShader()
    {
        // 1. Get the player's screen position.
        Vector3 playerScreenPosition =
            gameplayCamera.WorldToScreenPoint(player.position);

        Vector2 originUV = new Vector2(
            playerScreenPosition.x / Screen.width,
            playerScreenPosition.y / Screen.height
        );

        // 2. Get the aiming direction in screen space.
        Vector2 aimDirection = GetAimDirectionOnScreen();

        // 3. Calculate pixels per world unit.
        float pixelsPerUnit =
            Screen.height / (2f * gameplayCamera.orthographicSize);

        // 4. Update shader parameters.
        runtimeMaterial.SetVector(
            OriginID,
            new Vector4(originUV.x, originUV.y, 0f, 0f)
        );

        runtimeMaterial.SetVector(
            AimDirectionID,
            new Vector4(aimDirection.x, aimDirection.y, 0f, 0f)
        );

        runtimeMaterial.SetFloat(ConeAngleID, coneAngle);
        runtimeMaterial.SetFloat(EdgeSoftnessID, edgeSoftness);

        runtimeMaterial.SetFloat(
            RangePixelsID,
            coneRange * pixelsPerUnit
        );

        runtimeMaterial.SetFloat(
            RangeSoftnessPixelsID,
            rangeEdgeSoftness * pixelsPerUnit
        );
    }

    private Vector2 GetAimDirectionOnScreen()
    {
        Vector2 aimDirection = inputHandler.AimDirection;

        // Mouse aiming already uses screen-space direction.
        if (inputHandler.GetControlScheme() ==
            PlayerInputHandler.ControlScheme.KeyboardMouse)
        {
            return aimDirection.sqrMagnitude > 0.0001f
                ? aimDirection.normalized
                : Vector2.right;
        }

        // Controller aiming uses a world-space direction.
        // Convert it into screen-space direction.
        Vector3 screenOrigin =
            gameplayCamera.WorldToScreenPoint(player.position);

        Vector3 screenTarget =
            gameplayCamera.WorldToScreenPoint(
                player.position + (Vector3)aimDirection
            );

        Vector2 screenDirection = screenTarget - screenOrigin;

        return screenDirection.sqrMagnitude > 0.0001f
            ? screenDirection.normalized
            : Vector2.right;
    }
}