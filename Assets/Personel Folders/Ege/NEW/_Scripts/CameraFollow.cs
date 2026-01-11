using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // The car
    public Vector3 offset = new Vector3(5, 2, -10); // Adjust as needed
    public float smoothTime = 0.1f; // Daha seri takip için değeri düşürdük
    private Vector3 _currentVelocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Hedef pozisyon (X ve Y takibi, Z sabit offsetsiz)
        // Not: Offset'i burada ekliyoruz.
        Vector3 desiredPosition = target.position + offset;
        
        // Z eksenini kameranın kendi Z'sinde tutmak istersen (genelde 2D'de -10):
        desiredPosition.z = transform.position.z; 

        // SmoothDamp: Fizik tabanlı hareketlerde titremeyi (stutter) önler
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothTime);
    }
}