
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingRodAimEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private Transform player;
    [SerializeField] private Camera gameplayCamera;

    [SerializeField] private RawImage overlay;
    [SerializeField] private Material overlayMaterial;

    [Header("Effect Settings")]
    [SerializeField, Range(0f, 1f)]
    private float darkness = 0.35f;

    [SerializeField, Range(1f, 180f)]
    private float coneAngle = 50f;

    [SerializeField]
    private float coneRange = 6f;

    [SerializeField]
    private float edgeSoftness = 3f;

    [SerializeField]
    private float rangeEdgeSoftness = 0.35f;

    [Header("Fade Settings")]
    [SerializeField]
    private float fadeInDuration = 0.4f;

    [SerializeField]
    private float fadeOutDuration = 0.12f;

    [Header("Wall Occlusion")]
    [SerializeField]
    private LayerMask wallLayer;

    [SerializeField, Range(16, 256)]
    private int rayCount = 128;

    [SerializeField, Range(64, 2048)]
    private int maskWidth = 256;

    [SerializeField, Range(0.01f, 1f)]
    private float vertexAngleOffset = 0.15f;

    [Header("Mask Update")]
    [SerializeField]
    private float positionUpdateThreshold = 0.01f;

    [SerializeField]
    private float directionUpdateThreshold = 0.5f;

    // Runtime objects
    private Material runtimeMaterial;
    private Texture2D visibilityTexture;
    private Color32[] visibilityPixels;
    private float[] rowCoverage;

    private Canvas overlayCanvas;
    private CanvasGroup overlayCanvasGroup;

    private readonly Vector3[] overlayCorners = new Vector3[4];

    // Reusable collections
    private readonly List<float> rayAngles = new List<float>(512);
    private readonly List<float> scanlineIntersections = new List<float>(512);

    private readonly List<Vector2> visibilityPolygon =
        new List<Vector2>(512);

    private readonly List<Vector2> maskPolygonPoints =
        new List<Vector2>(512);

    private Collider2D[] nearbyWalls = new Collider2D[128];

    // Cached mask state
    private Vector2 lastMaskPlayerPosition;
    private Vector2 lastMaskAimDirection;
    private bool hasPreviousMask;

    // Shader property IDs
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

    private static readonly int OverlaySizePixelsID =
        Shader.PropertyToID("_OverlaySizePixels");

    private static readonly int VisibilityMaskID =
        Shader.PropertyToID("_VisibilityMask");


    private void Awake()
    {
        if (inputHandler == null ||
            playerCombat == null ||
            player == null ||
            gameplayCamera == null ||
            overlay == null ||
            overlayMaterial == null)
        {
            Debug.LogError(
                "FishingRodAimEffect: One or more references are missing.",
                this
            );

            enabled = false;
            return;
        }

        overlayCanvas = overlay.canvas;

        if (overlayCanvas == null)
        {
            Debug.LogError(
                "FishingRodAimEffect: Overlay is not attached to a Canvas.",
                this
            );

            enabled = false;
            return;
        }

        overlayCanvasGroup =
            overlay.GetComponent<CanvasGroup>();

        if (overlayCanvasGroup == null)
        {
            overlayCanvasGroup =
                overlay.gameObject.AddComponent<CanvasGroup>();
        }

        Canvas.ForceUpdateCanvases();

        CreateRuntimeMaterial();
        CreateVisibilityTexture();

        overlay.texture = Texture2D.whiteTexture;
        overlay.color = Color.white;

        overlayCanvasGroup.alpha = 0f;
        overlayCanvasGroup.blocksRaycasts = false;
        overlayCanvasGroup.interactable = false;
    }


    private void CreateRuntimeMaterial()
    {
        runtimeMaterial = new Material(overlayMaterial);

        overlay.material = runtimeMaterial;

        runtimeMaterial.SetColor(
            OverlayColorID,
            new Color(0f, 0f, 0f, darkness)
        );
    }


    private void CreateVisibilityTexture()
    {
        Rect overlayRect = overlay.rectTransform.rect;

        float aspectRatio =
            overlayRect.width / Mathf.Max(overlayRect.height, 0.001f);

        int maskHeight = Mathf.Max(
            1,
            Mathf.RoundToInt(maskWidth / aspectRatio)
        );

        visibilityTexture = new Texture2D(
            maskWidth,
            maskHeight,
            TextureFormat.RGBA32,
            false
        );

        visibilityTexture.filterMode = FilterMode.Bilinear;
        visibilityTexture.wrapMode = TextureWrapMode.Clamp;

        visibilityPixels =
            new Color32[maskWidth * maskHeight];

        rowCoverage = new float[maskWidth];

        runtimeMaterial.SetTexture(
            VisibilityMaskID,
            visibilityTexture
        );

        System.Array.Clear(
            visibilityPixels,
            0,
            visibilityPixels.Length
        );

        visibilityTexture.SetPixels32(visibilityPixels);
        visibilityTexture.Apply(false);
    }


    private void LateUpdate()
    {
        bool shouldShow = playerCombat.IsAimingUtility;

        float targetAlpha = shouldShow ? 1f : 0f;

        float fadeDuration = shouldShow
            ? fadeInDuration
            : fadeOutDuration;

        if (fadeDuration <= 0f)
        {
            overlayCanvasGroup.alpha = targetAlpha;
        }
        else
        {
            overlayCanvasGroup.alpha = Mathf.MoveTowards(
                overlayCanvasGroup.alpha,
                targetAlpha,
                Time.unscaledDeltaTime / fadeDuration
            );
        }

        if (overlayCanvasGroup.alpha > 0.001f)
        {
            UpdateShader();
        }
    }


    private void UpdateShader()
    {
        Vector2 playerPosition = player.position;

        Vector2 aimDirectionWorld =
            inputHandler.AimDirection.normalized;

        if (aimDirectionWorld.sqrMagnitude < 0.0001f)
            aimDirectionWorld = Vector2.up;

        Vector3 playerScreenPosition =
            gameplayCamera.WorldToScreenPoint(player.position);

        Vector2 originUV =
            ScreenToOverlayUV(playerScreenPosition);

        Vector2 aimDirectionScreen =
            GetAimDirectionOnScreen(aimDirectionWorld);

        // Recalculate the mask only when necessary.
        bool positionChanged =
            !hasPreviousMask ||
            Vector2.Distance(
                playerPosition,
                lastMaskPlayerPosition
            ) > positionUpdateThreshold;

        bool directionChanged =
            !hasPreviousMask ||
            Vector2.Angle(
                aimDirectionWorld,
                lastMaskAimDirection
            ) > directionUpdateThreshold;

        if (positionChanged || directionChanged)
        {
            UpdateVisibilityMask(aimDirectionWorld);

            lastMaskPlayerPosition = playerPosition;
            lastMaskAimDirection = aimDirectionWorld;

            hasPreviousMask = true;
        }

        Vector2 overlaySize = GetOverlayScreenSize();

        float pixelsPerUnit =
            Screen.height / (2f * gameplayCamera.orthographicSize);

        // Convert range into the same pixel coordinate system
        // used by the RawImage.
        float overlayPixelsPerUnit =
            pixelsPerUnit * (overlaySize.y / Mathf.Max(Screen.height, 1f));

        runtimeMaterial.SetVector(
            OriginID,
            new Vector4(originUV.x, originUV.y, 0f, 0f)
        );

        runtimeMaterial.SetVector(
            AimDirectionID,
            new Vector4(
                aimDirectionScreen.x,
                aimDirectionScreen.y,
                0f,
                0f
            )
        );

        runtimeMaterial.SetVector(
            OverlaySizePixelsID,
            new Vector4(overlaySize.x, overlaySize.y, 0f, 0f)
        );

        runtimeMaterial.SetFloat(ConeAngleID, coneAngle);
        runtimeMaterial.SetFloat(EdgeSoftnessID, edgeSoftness);

        runtimeMaterial.SetFloat(
            RangePixelsID,
            coneRange * overlayPixelsPerUnit
        );

        runtimeMaterial.SetFloat(
            RangeSoftnessPixelsID,
            rangeEdgeSoftness * overlayPixelsPerUnit
        );

        runtimeMaterial.SetColor(
            OverlayColorID,
            new Color(0f, 0f, 0f, darkness)
        );
    }


    private Vector2 GetAimDirectionOnScreen(Vector2 worldDirection)
    {
        Vector3 screenOrigin =
            gameplayCamera.WorldToScreenPoint(player.position);

        Vector3 worldTarget =
            player.position + (Vector3)worldDirection;

        Vector3 screenTarget =
            gameplayCamera.WorldToScreenPoint(worldTarget);

        return ((Vector2)(screenTarget - screenOrigin)).normalized;
    }


    private void UpdateVisibilityMask(Vector2 aimDirectionWorld)
    {
        float halfAngle = coneAngle * 0.5f;

        rayAngles.Clear();
        visibilityPolygon.Clear();

        // Add evenly distributed baseline rays.
        int safeRayCount = Mathf.Max(2, rayCount);

        for (int i = 0; i < safeRayCount; i++)
        {
            float angle = Mathf.Lerp(
                -halfAngle,
                halfAngle,
                i / (float)(safeRayCount - 1)
            );

            rayAngles.Add(angle);
        }

        // Find nearby wall colliders without allocating an array
        // during normal updates.
        int wallCount;

        while (true)
        {
            wallCount = Physics2D.OverlapCircleNonAlloc(
                player.position,
                coneRange,
                nearbyWalls,
                wallLayer
            );

            // Grow the buffer only if it was insufficient.
            if (wallCount < nearbyWalls.Length)
                break;

            System.Array.Resize(
                ref nearbyWalls,
                nearbyWalls.Length * 2
            );
        }

        // Add rays aimed just before and after each wall vertex.
        for (int wallIndex = 0; wallIndex < wallCount; wallIndex++)
        {
            PolygonCollider2D polygon =
                nearbyWalls[wallIndex] as PolygonCollider2D;

            if (polygon == null)
                continue;

            for (int pathIndex = 0;
                 pathIndex < polygon.pathCount;
                 pathIndex++)
            {
                Vector2[] vertices =
                    polygon.GetPath(pathIndex);

                foreach (Vector2 localVertex in vertices)
                {
                    Vector2 worldVertex =
                        polygon.transform.TransformPoint(localVertex);

                    Vector2 toVertex =
                        worldVertex - (Vector2)player.position;

                    if (toVertex.sqrMagnitude < 0.0001f ||
                        toVertex.magnitude > coneRange)
                    {
                        continue;
                    }

                    float angle = Vector2.SignedAngle(
                        aimDirectionWorld,
                        toVertex.normalized
                    );

                    if (Mathf.Abs(angle) > halfAngle)
                        continue;

                    rayAngles.Add(
                        Mathf.Clamp(
                            angle - vertexAngleOffset,
                            -halfAngle,
                            halfAngle
                        )
                    );

                    rayAngles.Add(angle);

                    rayAngles.Add(
                        Mathf.Clamp(
                            angle + vertexAngleOffset,
                            -halfAngle,
                            halfAngle
                        )
                    );
                }
            }
        }

        // Sort all rays by their angle.
        rayAngles.Sort();

        // Cast rays and collect visibility polygon endpoints.
        foreach (float angle in rayAngles)
        {
            Vector2 direction =
                Quaternion.Euler(0f, 0f, angle) *
                aimDirectionWorld;

            RaycastHit2D hit = Physics2D.Raycast(
                player.position,
                direction,
                coneRange,
                wallLayer
            );

            Vector2 endpoint = hit.collider != null
                ? hit.point
                : (Vector2)player.position + direction * coneRange;

            visibilityPolygon.Add(endpoint);
        }

        // Clear the previous mask.
        System.Array.Clear(
            visibilityPixels,
            0,
            visibilityPixels.Length
        );

        // Convert the visibility polygon into RawImage texture space.
        Vector3 screenOrigin =
            gameplayCamera.WorldToScreenPoint(player.position);

        Vector2 origin =
            ScreenToTexturePosition(screenOrigin);

        maskPolygonPoints.Clear();
        maskPolygonPoints.Add(origin);

        foreach (Vector2 worldPoint in visibilityPolygon)
        {
            Vector3 screenPoint =
                gameplayCamera.WorldToScreenPoint(worldPoint);

            maskPolygonPoints.Add(
                ScreenToTexturePosition(screenPoint)
            );
        }

        // Fill the entire polygon using scanline rasterization.
        RasterizeVisibilityPolygon(
            maskPolygonPoints,
            visibilityTexture.width,
            visibilityTexture.height
        );

        visibilityTexture.SetPixels32(visibilityPixels);
        visibilityTexture.Apply(false);
    }


    private void RasterizeVisibilityPolygon(
        List<Vector2> points,
        int width,
        int height)
    {
        if (points.Count < 3)
            return;

        const int verticalSamples = 4;

        float minX = width;
        float maxX = 0f;
        float minY = height;
        float maxY = 0f;

        foreach (Vector2 point in points)
        {
            minX = Mathf.Min(minX, point.x);
            maxX = Mathf.Max(maxX, point.x);
            minY = Mathf.Min(minY, point.y);
            maxY = Mathf.Max(maxY, point.y);
        }

        int startX = Mathf.Clamp(
            Mathf.FloorToInt(minX),
            0,
            width - 1
        );

        int endX = Mathf.Clamp(
            Mathf.CeilToInt(maxX),
            0,
            width - 1
        );

        int startY = Mathf.Clamp(
            Mathf.FloorToInt(minY),
            0,
            height - 1
        );

        int endY = Mathf.Clamp(
            Mathf.CeilToInt(maxY),
            0,
            height - 1
        );

        for (int y = startY; y <= endY; y++)
        {
            System.Array.Clear(
                rowCoverage,
                startX,
                endX - startX + 1
            );

            for (int sy = 0; sy < verticalSamples; sy++)
            {
                float sampleY =
                    y + (sy + 0.5f) / verticalSamples;

                scanlineIntersections.Clear();

                // Find intersections between the scanline
                // and the polygon edges.
                for (int i = 0; i < points.Count; i++)
                {
                    Vector2 a = points[i];
                    Vector2 b = points[(i + 1) % points.Count];

                    if ((a.y <= sampleY && b.y > sampleY) ||
                        (b.y <= sampleY && a.y > sampleY))
                    {
                        float t =
                            (sampleY - a.y) / (b.y - a.y);

                        float intersectionX =
                            Mathf.Lerp(a.x, b.x, t);

                        scanlineIntersections.Add(intersectionX);
                    }
                }

                scanlineIntersections.Sort();

                // Fill between pairs of intersections.
                for (int i = 0;
                     i + 1 < scanlineIntersections.Count;
                     i += 2)
                {
                    float left = scanlineIntersections[i];
                    float right = scanlineIntersections[i + 1];

                    int firstX = Mathf.Max(
                        startX,
                        Mathf.FloorToInt(left)
                    );

                    int lastX = Mathf.Min(
                        endX,
                        Mathf.CeilToInt(right)
                    );

                    for (int x = firstX; x <= lastX; x++)
                    {
                        // Calculate horizontal pixel coverage.
                        float overlap = Mathf.Max(
                            0f,
                            Mathf.Min(x + 1f, right) -
                            Mathf.Max(x, left)
                        );

                        rowCoverage[x] +=
                            overlap / verticalSamples;
                    }
                }
            }

            // Write the completed scanline to the texture buffer.
            for (int x = startX; x <= endX; x++)
            {
                float coverage =
                    Mathf.Clamp01(rowCoverage[x]);

                byte value = (byte)Mathf.RoundToInt(
                    coverage * 255f
                );

                visibilityPixels[y * width + x] =
                    new Color32(value, value, value, 255);
            }
        }
    }


    private Canvas GetOverlayCanvas()
    {
        return overlayCanvas;
    }


    private Camera GetOverlayCamera()
    {
        if (GetOverlayCanvas().renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        return overlayCanvas.worldCamera != null
            ? overlayCanvas.worldCamera
            : gameplayCamera;
    }


    private Vector2 ScreenToOverlayUV(Vector3 screenPosition)
    {
        RectTransform rectTransform = overlay.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            screenPosition,
            GetOverlayCamera(),
            out Vector2 localPoint))
        {
            return Vector2.zero;
        }

        Rect rect = rectTransform.rect;

        return new Vector2(
            (localPoint.x - rect.xMin) / rect.width,
            (localPoint.y - rect.yMin) / rect.height
        );
    }


    private Vector2 ScreenToTexturePosition(Vector3 screenPosition)
    {
        Vector2 uv = ScreenToOverlayUV(screenPosition);

        return new Vector2(
            uv.x * visibilityTexture.width,
            uv.y * visibilityTexture.height
        );
    }


    private Vector2 GetOverlayScreenSize()
    {
        RectTransform rectTransform = overlay.rectTransform;

        rectTransform.GetWorldCorners(overlayCorners);

        Camera uiCamera = GetOverlayCamera();

        Vector2 bottomLeft =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera,
                overlayCorners[0]
            );

        Vector2 topLeft =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera,
                overlayCorners[1]
            );

        Vector2 bottomRight =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera,
                overlayCorners[3]
            );

        float width = Vector2.Distance(
            bottomLeft,
            bottomRight
        );

        float height = Vector2.Distance(
            bottomLeft,
            topLeft
        );

        return new Vector2(width, height);
    }


    // Call this when wall geometry changes while the player is aiming.
    public void InvalidateVisibilityMask()
    {
        hasPreviousMask = false;
    }


    private void OnEnable()
    {
        hasPreviousMask = false;
    }


    private void OnDisable()
    {
        if (overlayCanvasGroup != null)
            overlayCanvasGroup.alpha = 0f;

        hasPreviousMask = false;
    }


    private void OnDestroy()
    {
        if (visibilityTexture != null)
            Destroy(visibilityTexture);

        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }
}
