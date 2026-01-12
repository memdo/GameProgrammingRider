using UnityEngine;
using UnityEngine.UI; // Required for UI Button control

public class DriveCar : MonoBehaviour
{
    [Header("Car Setup")]
    [SerializeField] private WheelJoint2D _backWheelJoint;
    [SerializeField] private WheelJoint2D _frontWheelJoint;
    [SerializeField] private Rigidbody2D _carRb;

    [Header("Physics Settings")]
    [SerializeField] private float _speed = 2000f;
    [SerializeField] private float _rotationSpeed = 800f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Boost Settings")]
    [SerializeField] private float _boostForce = 2500f;
    [SerializeField] private float _boostDuration = 2.0f;
    [SerializeField] private float _boostCooldown = 5.0f;

    [Header("Hill Climbing Stability")]
    // 1. Force that pushes car down into the track (Stickiness)
    [SerializeField] private float _downforce = 1000f;
    // 2. Lowers center of gravity to prevent flipping
    [SerializeField] private float _centerOfMassY = -1.0f;

    [Header("Flip Rewards")]
    [SerializeField] private int _flipCoinReward = 20; // Coins awarded for completing a 360-degree flip
    [SerializeField] private float _groundCheckDistance = 1.5f; // Distance to check for ground below car

    private float _moveInput;

    // Boost state variables
    private bool _isBoosting = false;
    private float _boostTimer = 0f;
    private float _boostCooldownTimer = 0f;

    // Flip detection variables
    private float _lastRotation = 0f;
    private float _totalRotation = 0f;
    private bool _isTrackingFlip = false;
    private bool _hasCompletedFlip = false; // Tracks if a flip was completed while in air
    private float _flipCompletionTime = 0f; // Time when flip was completed
    private const float LANDING_CHECK_DURATION = 3f; // How long to check for safe landing after flip
    //private float _lastVerticalVelocity = 0f; // Track vertical velocity for landing detection
    private int _stableFrames = 0; // Count frames with low vertical velocity
    private const int STABLE_FRAME_THRESHOLD = 5; // Number of frames needed to consider "landed"
    private const float MAX_LANDING_VELOCITY = 2f; // Maximum vertical velocity to consider "landed"
    
    // Consecutive flip tracking
    private int _consecutiveFlips = 0; // Number of consecutive flips completed
    
    // Death check after awarding coins
    private bool _isMonitoringDeath = false; // Track if we're monitoring for death after awarding coins
    private float _coinAwardTime = 0f; // Time when coins were awarded
    private const float DEATH_CHECK_DURATION = 0.5f; // How long to monitor for death after awarding coins

    // Store reference to the button
    private Button _boostButton; 

    private void Start()
    {
        // Lower the center of mass significantly
        _carRb.centerOfMass = new Vector2(0, _centerOfMassY);
        
        // Initialize flip tracking
        _lastRotation = _carRb.rotation;
        _totalRotation = 0f;
        _isTrackingFlip = false;
        _hasCompletedFlip = false;
        _flipCompletionTime = 0f;
        _consecutiveFlips = 0;

        // --- AUTOMATICALLY FIND THE BUTTON ---
        // 1. Find the GameObject named "BoostButton" in the scene
        GameObject btnObj = GameObject.Find("BoostButton");
        
        if (btnObj != null)
        {
            // 2. Get the Button component from it
            _boostButton = btnObj.GetComponent<Button>();
            
            if (_boostButton != null)
            {
                // 3. Tell the button to run "AttemptBoost" when clicked
                _boostButton.onClick.AddListener(AttemptBoost);
                Debug.Log("Boost Button found and connected automatically!");
            }
        }
        else
        {
            Debug.LogWarning("Could not find a GameObject named 'BoostButton' in the scene. Make sure you renamed it correctly!");
        }
    }

    private void Update()
    {
        _moveInput = 0f;

        // 1. Keyboard Input (PC)
        if (Input.GetKey(KeyCode.D)) _moveInput = 1f;
        else if (Input.GetKey(KeyCode.A)) _moveInput = -1f;

        // 2. Touch / Mouse Input (Mobile & Editor testing)
        if (Input.GetMouseButton(0))
        {
            // Check if touch is on the RIGHT half of the screen
            if (Input.mousePosition.x > Screen.width / 2)
            {
                _moveInput = 1f; // Gas (Same effect as 'D')
            }
            // Check if touch is on the LEFT half of the screen
            else
            {
                _moveInput = -1f; // Brake (Same effect as 'A')
            }
        }

        // 3. Boost Input (Space key)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttemptBoost();
        }

        // --- UPDATE BOOST TIMERS ---
        if (_boostCooldownTimer > 0f)
        {
            _boostCooldownTimer -= Time.deltaTime;
        }

        if (_isBoosting)
        {
            _boostTimer -= Time.deltaTime;
            if (_boostTimer <= 0f)
            {
                _isBoosting = false;
                _boostCooldownTimer = _boostCooldown; // Start cooldown immediately after boost finishes
            }
        }

        // --- [NEW PART] VISUAL COOLDOWN MANAGEMENT ---
        // If the button was found, update its look
        if (_boostButton != null)
        {
            // If currently boosting OR waiting for cooldown...
            if (_isBoosting || _boostCooldownTimer > 0f)
            {
                // Make button unclickable and look "disabled" (usually grey/transparent)
                _boostButton.interactable = false;
            }
            else
            {
                // Make button clickable and look normal (bright)
                _boostButton.interactable = true;
            }
        }
        // ----------------------------------------------
    }

    // Public method for the UI Button to call
    public void AttemptBoost()
    {
        if (_boostCooldownTimer <= 0f && !_isBoosting)
        {
            _isBoosting = true;
            _boostTimer = _boostDuration;
            // Debug.Log("Boost activated!"); 
        }
    }

    private void FixedUpdate()
    {
        // Use both wheel check and raycast for more reliable ground detection
        bool wheelGrounded = IsWheelTouchingGround(_frontWheelJoint) || IsWheelTouchingGround(_backWheelJoint);
        bool raycastGrounded = IsGroundedByRaycast();
        bool isGrounded = wheelGrounded || raycastGrounded;

        // Check if we completed a flip and need to verify safe landing
        if (_hasCompletedFlip)
        {
            float timeSinceFlip = Time.time - _flipCompletionTime;
            float verticalVelocity = Mathf.Abs(_carRb.linearVelocity.y);
            
            // Check if vertical velocity is low (indicating landing)
            if (verticalVelocity < MAX_LANDING_VELOCITY)
            {
                _stableFrames++;
                Debug.Log($"Low vertical velocity detected: {verticalVelocity:F2} m/s. Stable frames: {_stableFrames}/{STABLE_FRAME_THRESHOLD}");
            }
            else
            {
                _stableFrames = 0; // Reset if velocity increases
            }
            
            // If velocity has been stable for enough frames, consider it a safe landing
            if (_stableFrames >= STABLE_FRAME_THRESHOLD && timeSinceFlip < LANDING_CHECK_DURATION)
            {
                Debug.Log($"SAFE LANDING VERIFIED! Car has been stable for {_stableFrames} frames ({timeSinceFlip:F2}s after flip). Vertical velocity: {verticalVelocity:F2} m/s. Awarding coins!");
                AwardFlipCoins();
                _hasCompletedFlip = false;
                _flipCompletionTime = 0f;
                _stableFrames = 0;
                // Reset consecutive flips after awarding coins
                _consecutiveFlips = 0;
            }
            // If too much time has passed, the flip doesn't count
            else if (timeSinceFlip >= LANDING_CHECK_DURATION)
            {
                Debug.Log($"Flip completion expired (took {timeSinceFlip:F2}s). No coins awarded. Final velocity: {verticalVelocity:F2} m/s");
                _hasCompletedFlip = false;
                _flipCompletionTime = 0f;
                _stableFrames = 0;
                // Reset consecutive flips if landing check expires
                _consecutiveFlips = 0;
            }
            else
            {
                // Only log occasionally to avoid spam
                if (Time.frameCount % 30 == 0)
                {
                    Debug.Log($"Waiting for landing... Time: {timeSinceFlip:F2}s, VelY: {verticalVelocity:F2} m/s, Stable: {_stableFrames}/{STABLE_FRAME_THRESHOLD}");
                }
            }
        }
        else
        {
            _stableFrames = 0; // Reset when not checking for landing
        }

        // Check if enough time has passed since awarding coins (stop monitoring if player survived)
        if (_isMonitoringDeath)
        {
            float timeSinceAward = Time.time - _coinAwardTime;
            
            // If enough time has passed without death, stop monitoring
            if (timeSinceAward >= DEATH_CHECK_DURATION)
            {
                Debug.Log($"Death check passed! Player survived {DEATH_CHECK_DURATION}s after flip reward. Coins are safe.");
                _isMonitoringDeath = false;
            }
        }

        // 1. APPLY DOWNFORCE (The "Sticky" Fix)
        if (isGrounded)
        {
            _carRb.AddRelativeForce(Vector2.down * _downforce * Time.fixedDeltaTime);
        }

        // 2. DRIVE
        JointMotor2D motor = new JointMotor2D
        {
            motorSpeed = -_moveInput * _speed,
            maxMotorTorque = 100000
        };
        _backWheelJoint.motor = motor;
        _frontWheelJoint.motor = motor;

        // 3. APPLY BOOST FORCE (Forward push)
        if (_isBoosting && _moveInput > 0) // Only apply boost when moving forward
        {
            _carRb.AddRelativeForce(Vector2.right * _boostForce * Time.fixedDeltaTime);
        }

        // 4. AIR CONTROL (Flip)
        if (!isGrounded)
        {
            if (_moveInput != 0)
            {
                _carRb.AddTorque(_moveInput * _rotationSpeed * Time.fixedDeltaTime);
            }
            
            TrackFlipRotation();
        }
        else
        {
            // Stabilize rotation on ground
            _carRb.angularVelocity = Mathf.Lerp(_carRb.angularVelocity, 0, Time.fixedDeltaTime * 10f);
            
            // Reset flip tracking when grounded (but only if we haven't completed a flip)
            if (_isTrackingFlip && !_hasCompletedFlip)
            {
                _isTrackingFlip = false;
                _totalRotation = 0f;
                // Reset consecutive flips if landing without completing a flip (breaks the chain)
                if (_consecutiveFlips > 0)
                {
                    Debug.Log($"Landing without completing flip. Resetting consecutive flip counter (was {_consecutiveFlips}).");
                    _consecutiveFlips = 0;
                }
            }
            _lastRotation = _carRb.rotation;
        }
    }

    private void TrackFlipRotation()
    {
        float currentRotation = _carRb.rotation;
        
        // Calculate the rotation difference, handling wrap-around
        float rotationDelta = Mathf.DeltaAngle(_lastRotation, currentRotation);
        
        // Start tracking if we detect significant rotation
        if (Mathf.Abs(rotationDelta) > 5f && !_isTrackingFlip)
        {
            _isTrackingFlip = true;
            Debug.Log($"Flip tracking started. Rotation delta: {rotationDelta}");
        }
        
        if (_isTrackingFlip)
        {
            _totalRotation += rotationDelta;
            
            // Debug rotation progress
            if (Mathf.Abs(_totalRotation) % 90f < 5f && Mathf.Abs(_totalRotation) > 5f)
            {
                Debug.Log($"Flip progress: {Mathf.Abs(_totalRotation):F1} degrees");
            }
            
            // Check if we've completed a full 360-degree flip (either direction)
            if (Mathf.Abs(_totalRotation) >= 360f)
            {
                CompleteFlip();
            }
        }
        
        _lastRotation = currentRotation;
    }

    private void CompleteFlip()
    {
        // Increment consecutive flip counter
        _consecutiveFlips++;
        
        // Mark that a flip was completed (will award coins when player lands safely)
        _hasCompletedFlip = true;
        _flipCompletionTime = Time.time;
        Debug.Log($"360-degree flip #{_consecutiveFlips} completed at time {_flipCompletionTime:F2}! Waiting for safe landing (will check for {LANDING_CHECK_DURATION}s)...");
        
        // Reset tracking for next flip (but keep consecutive count)
        _totalRotation = 0f;
        _isTrackingFlip = false;
    }

    private void AwardFlipCoins()
    {
        // Award coins for completing the flip and landing safely
        if (GameManager.Instance != null)
        {
            // Calculate total reward: base reward multiplied by number of consecutive flips
            int totalReward = _flipCoinReward * _consecutiveFlips;
            int coinsBefore = GameManager.Instance.CurrentRunCoins;
            
            Debug.Log($"Attempting to award coins for {_consecutiveFlips} consecutive flip(s). Base reward: {_flipCoinReward}, Total: {totalReward} coins. Current coins: {coinsBefore}");
            
            GameManager.Instance.AddCoins(totalReward);
            
            int coinsAfter = GameManager.Instance.CurrentRunCoins;
            Debug.Log($"360-degree flip(s) completed and landed safely! Awarded {totalReward} coins ({_consecutiveFlips} flip(s) × {_flipCoinReward} base). (Coins: {coinsBefore} -> {coinsAfter})");
            
            // Play coin sound effect (same as regular coin collection)
            GameManager.Instance.PlayCoinSound();
            
            // Start monitoring for death - if player dies within DEATH_CHECK_DURATION, revoke coins
            _isMonitoringDeath = true;
            _coinAwardTime = Time.time;
            Debug.Log($"Starting death monitoring for {DEATH_CHECK_DURATION}s. If player dies, coins will be revoked.");
        }
        else
        {
            Debug.LogError("GameManager.Instance is null. Cannot award flip coins. Make sure GameManager exists in the scene and is initialized.");
        }
    }

    private void RevokeFlipCoins()
    {
        // Remove coins if player died shortly after being awarded them
        if (GameManager.Instance != null)
        {
            int coinsBefore = GameManager.Instance.CurrentRunCoins;
            
            // Calculate the total reward that was given (base reward × consecutive flips)
            int totalRewardGiven = _flipCoinReward * _consecutiveFlips;
            
            // Remove the flip reward amount (but don't go below 0)
            int coinsToRemove = Mathf.Min(totalRewardGiven, coinsBefore);
            GameManager.Instance.AddCoins(-coinsToRemove);
            
            int coinsAfter = GameManager.Instance.CurrentRunCoins;
            Debug.Log($"Revoked {coinsToRemove} coins due to death after {_consecutiveFlips} flip(s). (Coins: {coinsBefore} -> {coinsAfter})");
        }
    }

    private bool IsWheelTouchingGround(WheelJoint2D wheel)
    {
        if (wheel == null)
        {
            return false;
        }
        
        if (wheel.connectedBody == null)
        {
            return false;
        }
        
        Collider2D wheelCollider = wheel.connectedBody.GetComponent<Collider2D>();
        if (wheelCollider == null)
        {
            return false;
        }
        
        bool touching = wheelCollider.IsTouchingLayers(_groundLayer);
        return touching;
    }

    private bool IsGroundedByRaycast()
    {
        // Cast a ray downward from the car's position to detect ground
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = Vector2.down;
        
        // Cast multiple rays for better detection (center, left, right)
        RaycastHit2D hitCenter = Physics2D.Raycast(rayOrigin, rayDirection, _groundCheckDistance, _groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(rayOrigin + Vector2.left * 0.5f, rayDirection, _groundCheckDistance, _groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rayOrigin + Vector2.right * 0.5f, rayDirection, _groundCheckDistance, _groundLayer);
        
        bool grounded = hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null;
        
        return grounded;
    }

    public void ActivateMoonGravity(float duration)
    {
        StopCoroutine("MoonGravityRoutine"); // Eğer zaten aktifse süreyi sıfırlamak için önce durdur
        StartCoroutine(MoonGravityRoutine(duration));
    }
    private System.Collections.IEnumerator MoonGravityRoutine(float duration)
    {
        // 1. Mevcut fizik değerlerini sakla (eski haline getirebilmek için)
        float originalGravity = _carRb.gravityScale;
        float originalDownforce = _downforce;
        // 2. "Ay Modu" ayarlarını uygula
        _carRb.gravityScale = 0.2f; // Yer çekimini çok azalt (süzülmesi için)
        _downforce = 0f;            // Yere yapışma kuvvetini kapat (yoksa havaya kalkamaz)
        // 3. Aracı hafifçe havaya fırlat (Hoplatma efekti)
        // Impulse modu ile anlık bir itme kuvveti uyguluyoruz. Değeri kütleye göre ayarladım.
        Vector2 jumpDirection = new Vector2(0.7f, 0.5f); // Hem yukarı hem ileri
        _carRb.AddForce(jumpDirection * _carRb.mass * 2f, ForceMode2D.Impulse);
        Debug.Log("Ay Yer Çekimi Aktif! (Süre: " + duration + " sn)");
        // 4. Belirlenen süre kadar bekle
        yield return new WaitForSeconds(duration);
        // 5. Değerleri normale döndür
        _carRb.gravityScale = originalGravity;
        _downforce = originalDownforce;

        if (_carRb.linearVelocity.y > 0)
        {
            // Dikey hızı sıfırla veya çok azalt ki düşmeye başlasın
            Vector2 stopUpward = _carRb.linearVelocity;
            stopUpward.y = stopUpward.y * 0.1f; // Hızı %90 kes
            _carRb.linearVelocity = stopUpward;
        }

        Debug.Log("Ay Yer Çekimi Sona Erdi.");
    }


    private void OnDestroy()
    {
        // Clean up the button listener to avoid errors when car is destroyed
        if (_boostButton != null)
        {
            _boostButton.onClick.RemoveListener(AttemptBoost);
        }

        // If player dies while we're monitoring for death after awarding coins, revoke them
        if (_isMonitoringDeath)
        {
            float timeSinceAward = Time.time - _coinAwardTime;
            if (timeSinceAward < DEATH_CHECK_DURATION)
            {
                Debug.Log($"Player died {timeSinceAward:F2}s after flip reward! Removing coins...");
                RevokeFlipCoins();
            }
        }
    }
}