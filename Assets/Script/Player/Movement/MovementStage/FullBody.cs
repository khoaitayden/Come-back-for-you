using UnityEngine;

public class FullBody : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D headRb;   
    [SerializeField] private Rigidbody2D bodyRb;
    [SerializeField] private Rigidbody2D leftArmRb;
    [SerializeField] private Rigidbody2D rightArmRb;
    [SerializeField] private Rigidbody2D leftLegRb;
    [SerializeField] private Rigidbody2D rightLegRb;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] private float stepFrequency = 0.5f; 
    [SerializeField] private float stepForce = 10f;      
    [SerializeField] private float legSwingTorque = 5f;  
    [SerializeField] private float stabilizationForce = 2f; 
    [SerializeField] private float balanceTorque = 3f;  
    [SerializeField] private float armBalanceTorque = 1.5f; 
    [SerializeField] private float legUprightTorque = 15f; 

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 15f;         // Force to propel the body upward
    [SerializeField] private float legPushForce = 3f;      // Upward force on legs to push during jump
    [SerializeField] private float airControlForce = 5f;   // Horizontal force for mid-air control

    [Header("Fall Settings")]
    [SerializeField] private float normalGravityScale = 1f; // Default gravity scale
    [SerializeField] private float fallGravityScale = 3f;  // Increased gravity scale for faster fall

    [Header("Ground Check Settings")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.73f, 0.14f); 
    [SerializeField] private float groundCheckDistance = 0.01f; 

    private Vector2 moveInput; 
    private float stepTimer;   
    private bool isLeftLegStep;
    private bool isLegGrounded;
    private bool bothLegsGrounded;
    private bool wantsToJump;          // Tracks if player wants to jump

    void Start()
    {
        stepTimer = stepFrequency; 
        isLeftLegStep = true;    
        SetGravityScale(normalGravityScale); // Initialize with normal gravity
    }

    void Update()
    {
        moveInput = PlayerInputHandler.Instance != null ? PlayerInputHandler.Instance.MoveInput : Vector2.zero;

        bool leftLegGrounded = Physics2D.BoxCast(
            leftLegRb.position,
            groundCheckSize,
            leftLegRb.rotation,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        bool rightLegGrounded = Physics2D.BoxCast(
            rightLegRb.position,
            groundCheckSize,
            rightLegRb.rotation,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        isLegGrounded = leftLegGrounded || rightLegGrounded; 
        bothLegsGrounded = leftLegGrounded && rightLegGrounded; // Require both legs grounded for jump

        // Detect jump input (e.g., "W" key or up axis)
        if (moveInput.y > 0.5f && bothLegsGrounded && !wantsToJump)
        {
            wantsToJump = true;
        }

        // Update step timer for walking
        if (Mathf.Abs(moveInput.x) > 0.05f && isLegGrounded)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                stepTimer = stepFrequency; 
                isLeftLegStep = !isLeftLegStep; 
            }
        }
    }

    void FixedUpdate()
    {
        if (headRb == null || bodyRb == null || leftArmRb == null || rightArmRb == null || leftLegRb == null || rightLegRb == null) return;

        MaintainBalance();
        HandleJump();
        HandleMidAirControl();
        HandleWalking();
        KeepLegsUpright();

        // Adjust gravity scale based on ground contact
        if (!isLegGrounded)
        {
            SetGravityScale(fallGravityScale);
        }
        else if (bothLegsGrounded)
        {
            SetGravityScale(normalGravityScale);
        }
    }

    private void SetGravityScale(float scale)
    {
        bodyRb.gravityScale = scale;
        leftLegRb.gravityScale = scale;
        rightLegRb.gravityScale = scale;
    }

    private void MaintainBalance()
    {
        float bodyAngle = bodyRb.rotation;
        float balanceCorrection = -Mathf.Clamp(bodyAngle, -45f, 45f) * balanceTorque;
        bodyRb.AddTorque(balanceCorrection, ForceMode2D.Force);

        bodyRb.AddForce(Vector2.up * stabilizationForce, ForceMode2D.Force);

        float armBalance = Mathf.Clamp(bodyAngle, -30f, 30f) * armBalanceTorque;
        leftArmRb.AddTorque(-armBalance, ForceMode2D.Force);
        rightArmRb.AddTorque(armBalance, ForceMode2D.Force);
    }

    private void HandleJump()
    {
        if (wantsToJump && bothLegsGrounded)
        {
            // Execute jump: apply upward force to body
            bodyRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Apply upward force to legs to simulate pushing off
            leftLegRb.AddForce(Vector2.up * legPushForce, ForceMode2D.Impulse);
            rightLegRb.AddForce(Vector2.up * legPushForce, ForceMode2D.Impulse);

            // Apply a small upward force to head for natural motion
            headRb.AddForce(Vector2.up * (jumpForce * 0.5f), ForceMode2D.Impulse);

            wantsToJump = false; // Reset jump state
        }
    }

    private void HandleMidAirControl()
    {
        // Allow horizontal control mid-air if not grounded
        if (!isLegGrounded && Mathf.Abs(moveInput.x) > 0.05f)
        {
            Vector2 airControl = Vector2.right * moveInput.x * airControlForce;
            bodyRb.AddForce(airControl, ForceMode2D.Force);
        }
    }

    private void HandleWalking()
    {
        if (Mathf.Abs(moveInput.x) > 0.05f && isLegGrounded)
        {
            Rigidbody2D steppingLeg = isLeftLegStep ? leftLegRb : rightLegRb;
            Rigidbody2D supportingLeg = isLeftLegStep ? rightLegRb : leftLegRb;

            bool steppingLegGrounded = Physics2D.BoxCast(
                steppingLeg.position,
                groundCheckSize,
                steppingLeg.rotation,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            );

            float stepDirection = moveInput.x > 0 ? -1f : 1f;
            steppingLeg.AddTorque(stepDirection * legSwingTorque, ForceMode2D.Force);

            Vector2 walkForce = Vector2.right * moveInput.x * stepForce;
            bodyRb.AddForce(walkForce, ForceMode2D.Force);

            if (!steppingLegGrounded && Physics2D.BoxCast(
                supportingLeg.position,
                groundCheckSize,
                supportingLeg.rotation,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            ))
            {
                supportingLeg.AddForce(Vector2.down * 3f, ForceMode2D.Force); 
            }

            bodyRb.AddTorque(-moveInput.x * 0.5f, ForceMode2D.Force);
        }
    }

    private void KeepLegsUpright()
    {
        foreach (Rigidbody2D leg in new[] { leftLegRb, rightLegRb })
        {
            float legAngle = leg.rotation;
            if (Mathf.Abs(legAngle) > 10f)
            {
                float correction = -Mathf.Sign(legAngle) * legUprightTorque * Time.fixedDeltaTime;
                leg.AddTorque(correction, ForceMode2D.Force);
            }
            Vector2 toBody = (bodyRb.position - leg.position).normalized;
            leg.AddForce(toBody * 0.5f, ForceMode2D.Force);
        }
    }

    void OnDrawGizmos()
    {
        if (leftLegRb != null)
        {
            Gizmos.color = Physics2D.BoxCast(
                leftLegRb.position,
                groundCheckSize,
                leftLegRb.rotation,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            ) ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                leftLegRb.position,
                Quaternion.Euler(0, 0, leftLegRb.rotation),
                Vector3.one
            );
            Gizmos.DrawWireCube(
                Vector2.down * groundCheckDistance,
                new Vector3(groundCheckSize.x, groundCheckSize.y, 1)
            );
        }

        if (rightLegRb != null)
        {
            Gizmos.color = Physics2D.BoxCast(
                rightLegRb.position,
                groundCheckSize,
                rightLegRb.rotation,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            ) ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                rightLegRb.position,
                Quaternion.Euler(0, 0, rightLegRb.rotation),
                Vector3.one
            );
            Gizmos.DrawWireCube(
                Vector2.down * groundCheckDistance,
                new Vector3(groundCheckSize.x, groundCheckSize.y, 1)
            );
        }
        Gizmos.matrix = Matrix4x4.identity;
    }
}