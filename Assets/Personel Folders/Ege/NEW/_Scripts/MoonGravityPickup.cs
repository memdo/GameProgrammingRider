using UnityEngine;

public class MoonGravityPickup : MonoBehaviour
{
    [Tooltip("Özelliðin kaç saniye süreceði")]
    [SerializeField] private float duration = 0f; // 2 saniye havada kalsýn

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Çarpan objenin kendisinde veya ebeveynlerinde 'DriveCar' scripti var mý diye bak
        DriveCar car = other.GetComponentInParent<DriveCar>();

        if (car != null)
        {
            // Arabayý bulduk, özelliði aktifleþtir
            car.ActivateMoonGravity(duration);

            // Efekti verdikten sonra bu objeyi yok et (toplanmýþ gibi görünsün)
            Destroy(gameObject);
        }
    }
}