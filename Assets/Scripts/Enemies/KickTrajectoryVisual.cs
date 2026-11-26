using UnityEngine;

public class KickTrajectoryVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer circlePrefab;
    [SerializeField] private SpriteRenderer impactPrefab;

    [SerializeField] private float circleSpacing = 0.25f;
    [SerializeField] private int maxCircles = 30;
    [SerializeField] private float startSize = 0.18f;
    [SerializeField] private float endSize = 0.06f;
    [SerializeField] private float impactSize = 0.3f;

    [SerializeField] private LayerMask obstacleLayers;

    private SpriteRenderer[] circles;
    private SpriteRenderer impactCircle;

    private void Awake()
    {
        circles = new SpriteRenderer[maxCircles];

        for (int i = 0; i < maxCircles; i++)
        {
            circles[i] = Instantiate(circlePrefab, transform);
            circles[i].gameObject.SetActive(false);
        }

        impactCircle = Instantiate(impactPrefab, transform);
        impactCircle.gameObject.SetActive(false);
    }

    public void Draw(Vector2 origin, Vector2 direction, float maxDistance)
    {
        direction.Normalize();

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            maxDistance,
            obstacleLayers
        );

        bool blocked = hit.collider != null;

        float drawDistance = blocked ? hit.distance : maxDistance;

        int visibleCircles = Mathf.Min(
            Mathf.FloorToInt(drawDistance / circleSpacing),
            maxCircles
        );

        for (int i = 0; i < maxCircles; i++)
        {
            if (i >= visibleCircles)
            {
                circles[i].gameObject.SetActive(false);
                continue;
            }

            float t = (float)i / Mathf.Max(visibleCircles - 1, 1);

            float currentDistance = (i + 1) * circleSpacing;
            float size = Mathf.Lerp(startSize, endSize, t);

            circles[i].transform.position =
                origin + direction * currentDistance;

            circles[i].transform.localScale = Vector3.one * size;
            circles[i].gameObject.SetActive(true);
        }

        impactCircle.gameObject.SetActive(blocked);

        if (blocked)
        {
            impactCircle.transform.position = hit.point;
            impactCircle.transform.localScale = Vector3.one * impactSize;
        }
    }

    public void Hide()
    {
        foreach (SpriteRenderer circle in circles)
            circle.gameObject.SetActive(false);

        impactCircle.gameObject.SetActive(false);
    }
}