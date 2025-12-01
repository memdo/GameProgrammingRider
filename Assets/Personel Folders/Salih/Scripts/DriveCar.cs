using UnityEngine;

public class DriveCar : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _frontTireRB;
    [SerializeField] private Rigidbody2D _backTireRB;
    [SerializeField] private Rigidbody2D _carRb;
    [SerializeField] private float _speed = 150f;
    [SerializeField] private float _rotationSpeed = 100f;

    private float _moveInput;

    private void Update()
    {
        _moveInput = 0f;

        if (Input.GetKey("d"))
        {
            _moveInput = 1f;
        }
        else if (Input.GetKey("a"))
        {
            _moveInput = -1f;
        }
    }

    private void FixedUpdate()
    {
        // Directly set wheel rotation speed
        _frontTireRB.angularVelocity = -_moveInput * _speed;
        _backTireRB.angularVelocity = -_moveInput * _speed;

        // Apply torque to car body for rotation
        _carRb.AddTorque(_moveInput * _rotationSpeed * Time.fixedDeltaTime);
    }
}
