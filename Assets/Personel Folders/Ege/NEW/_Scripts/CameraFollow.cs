using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // The car
    public Vector3 offset = new Vector3(5, 2, -10); // Adjust as needed
    public float smoothSpeed = 0.125f;

    void FixedUpdate()
    {
        if (target == null) return;

        // Only follow X position, maybe Y if you want jumping to show
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, offset.z);
        
        // Smooth slide to target
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}