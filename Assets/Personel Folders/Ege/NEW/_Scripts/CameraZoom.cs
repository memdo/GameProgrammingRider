using UnityEngine;
public class CameraZoom : MonoBehaviour
{
    [Header("Assign Your Player Here")]
    public Transform player;

    public float zoomSpeed = 2f;
    public float moveSpeed = 3f;
    public float targetSize = 3f;

    private float defaultSize = 5f;
    private bool zooming = false;

    void Start()
    {
        defaultSize = Camera.main.orthographicSize;
    }

    void Update()
    {
        if (zooming)
        {
            // --- ZOOM ---
            Camera.main.orthographicSize =
                Mathf.MoveTowards(Camera.main.orthographicSize, targetSize, zoomSpeed * Time.unscaledDeltaTime);

            // --- MOVE CAMERA TOWARD PLAYER ---
            if (player != null)
            {
                Vector3 targetPos = new Vector3(player.position.x, player.position.y, transform.position.z);

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.unscaledDeltaTime
                );
            }
        }
    }

    public void StartZoom()
    {
        zooming = true;
    }
}
