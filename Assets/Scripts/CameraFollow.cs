using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Batas Tepi Peta (World Coordinates)")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 5f);

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    private void Start()
    {
        if (target == null) return;

        Vector3 startPos = target.position + offset;

        if (useBounds && cam != null)
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            float minX = minBounds.x + halfWidth;
            float maxX = maxBounds.x - halfWidth;
            float minY = minBounds.y + halfHeight;
            float maxY = maxBounds.y - halfHeight;

            startPos.x = (minX > maxX)
                ? (minBounds.x + maxBounds.x) * 0.5f
                : Mathf.Clamp(startPos.x, minX, maxX);

            startPos.y = (minY > maxY)
                ? (minBounds.y + maxBounds.y) * 0.5f
                : Mathf.Clamp(startPos.y, minY, maxY);
        }

        transform.position = startPos;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (useBounds && cam != null)
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            float minX = minBounds.x + halfWidth;
            float maxX = maxBounds.x - halfWidth;
            float minY = minBounds.y + halfHeight;
            float maxY = maxBounds.y - halfHeight;

            desiredPosition.x = (minX > maxX)
                ? (minBounds.x + maxBounds.x) * 0.5f
                : Mathf.Clamp(desiredPosition.x, minX, maxX);

            desiredPosition.y = (minY > maxY)
                ? (minBounds.y + maxBounds.y) * 0.5f
                : Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minBounds.x + maxBounds.x) * 0.5f, (minBounds.y + maxBounds.y) * 0.5f, 0f);
        Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}