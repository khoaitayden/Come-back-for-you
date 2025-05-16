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
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float legPushForce = 3f;
    [SerializeField] private float airControlForce = 5f;
    [SerializeField] private float jumpCooldownDuration = 0.5f;

    [Header("Fall Settings")]
    [SerializeField] private float normalGravityScale = 1f;
    [SerializeField] private float fallGravityScale = 3f;

    [Header("Ground Check Settings")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.73f, 0.14f);
    [SerializeField] private float groundCheckDistance = 0.01f;

    private Vector2 moveInput;
    private float stepTimer;
    private bool isLeftLegStep;
    private bool isLegGrounded;
    private bool bothLegsGrounded;
    private bool wantsToJump;
    private float jumpCooldownTimer;

    void Start()
    {
        stepTimer = stepFrequency;
        isLeftLegStep = true;
        jumpCooldownTimer = 0f;
        SetGravityScale(normalGravityScale);
    }

    void Update()
    {
        UpdateInput();
        UpdateGroundedStates();
        UpdateJumpRequest();
        UpdateJumpCooldown();
        UpdateStepTimer();
    }

    void FixedUpdate()
    {
        if (!AllRbsAssigned()) return;

        MaintainBalance();
        HandleJump();
        HandleMidAirControl();
        HandleWalking();
        KeepLegsUpright();
        AdjustGravityScale();
    }

    private void UpdateInput()
    {
        moveInput = PlayerInputHandler.Instance != null ? PlayerInputHandler.Instance.MoveInput : Vector2.zero;
    }

    private void UpdateGroundedStates()
    {
        bool leftLegGrounded = IsLegGrounded(leftLegRb);
        bool rightLegGrounded = IsLegGrounded(rightLegRb);
        isLegGrounded = leftLegGrounded || rightLegGrounded;
        bothLegsGrounded = leftLegGrounded && rightLegGrounded;
    }

    private bool IsLegGrounded(Rigidbody2D legRb)
    {
        return Physics2D.BoxCast(
            legRb.position,
            groundCheckSize,
            legRb.rotation,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    private void UpdateJumpRequest()
    {
        if (moveInput.y > 0.5f && bothLegsGrounded && jumpCooldownTimer <= 0f && !wantsToJump)
        {
            wantsToJump = true;
        }
    }

    private void UpdateJumpCooldown()
    {
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
    }

    private void UpdateStepTimer()
    {
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
            bodyRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            leftLegRb.AddForce(Vector2.up * legPushForce, ForceMode2D.Impulse);
            rightLegRb.AddForce(Vector2.up * legPushForce, ForceMode2D.Impulse);
            headRb.AddForce(Vector2.up * (jumpForce * 0.5f), ForceMode2D.Impulse);

            jumpCooldownTimer = jumpCooldownDuration;
            wantsToJump = false;
        }
    }

    private void HandleMidAirControl()
    {
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

            bool steppingLegGrounded = IsLegGrounded(steppingLeg);

            float stepDirection = moveInput.x > 0 ? -1f : 1f;
            steppingLeg.AddTorque(stepDirection * legSwingTorque, ForceMode2D.Force);

            Vector2 walkForce = Vector2.right * moveInput.x * stepForce;
            bodyRb.AddForce(walkForce, ForceMode2D.Force);

            if (!steppingLegGrounded && IsLegGrounded(supportingLeg))
            {
                supportingLeg.AddForce(Vector2.down * 3f, ForceMode2D.Force);
            }

            bodyRb.AddTorque(-moveInput.x * 0.5f, ForceMode2D.Force);
        }
    }

    private void KeepLegsUpright()
    {
        KeepLegUpright(leftLegRb);
        KeepLegUpright(rightLegRb);
    }

    private void KeepLegUpright(Rigidbody2D leg)
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

    private void AdjustGravityScale()
    {
        if (!isLegGrounded)
            SetGravityScale(fallGravityScale);
        else if (bothLegsGrounded)
            SetGravityScale(normalGravityScale);
    }

    private void SetGravityScale(float scale)
    {
        bodyRb.gravityScale = scale;
        leftLegRb.gravityScale = scale;
        rightLegRb.gravityScale = scale;
    }

    private bool AllRbsAssigned()
    {
        return headRb && bodyRb && leftArmRb && rightArmRb && leftLegRb && rightLegRb;
    }

    void OnDrawGizmos()
    {
        DrawLegGizmo(leftLegRb);
        DrawLegGizmo(rightLegRb);
        Gizmos.matrix = Matrix4x4.identity;
    }

    private void DrawLegGizmo(Rigidbody2D legRb)
    {
        if (legRb == null) return;
        Gizmos.color = IsLegGrounded(legRb) ? Color.green : Color.red;
        Gizmos.matrix = Matrix4x4.TRS(
            legRb.position,
            Quaternion.Euler(0, 0, legRb.rotation),
            Vector3.one
        );
        Gizmos.DrawWireCube(
            Vector2.down * groundCheckDistance,
            new Vector3(groundCheckSize.x, groundCheckSize.y, 1)
        );
    }
}