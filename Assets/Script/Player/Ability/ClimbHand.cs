using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbWallAbility : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D leftHandRb;
    [SerializeField] private Rigidbody2D rightHandRb;
    [SerializeField] private Rigidbody2D bodyRb;
    [SerializeField] private LayerMask climbWallLayer;
    [SerializeField] private MovementStage movementStage; 

    [Header("Climb Settings")]
    [SerializeField] private float baseStickForce = 10f;
    [SerializeField] private float maxStickTime = 3f;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private float wallCheckDistance = 0.1f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpCooldownDuration = 1f;
    [SerializeField] private float jumpThreshold = 0.5f; 
    [SerializeField] private float stickCooldownAfterJump = 0.3f;

    [Header("Stage-Specific Settings")]
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseJumpForce = 15f;
    [SerializeField] private float oneArmMoveSpeed = 5f;
    [SerializeField] private float oneArmJumpForce = 15f;
    [SerializeField] private float twoArmsMoveSpeed = 6f;
    [SerializeField] private float twoArmsJumpForce = 18f;
    [SerializeField] private float fullBodyMoveSpeed = 7f;
    [SerializeField] private float fullBodyJumpForce = 20f;

    [Header("Input")]
    [SerializeField] private InputAction preJump;

    private bool isSticking = false;
    private float stickTimer = 0f;
    private float jumpCooldownTimer = 0f;
    private float stickCooldownTimer = 0f;
    private Vector2 wallNormal;
    private bool oneHandStuck = false;
    private float currentMoveSpeed;
    private float currentJumpForce;
    private bool shouldJump = false;
    private float jumpDirection;

    private void OnEnable()
    {
            preJump.Enable();
    }

    private void OnDisable()
    {
            preJump.Disable();
    }

    void Start()
    {
        UpdateMovementParameters();
    }

    void Update()
    {
        Vector2 moveInput = PlayerInputHandler.Instance != null ? PlayerInputHandler.Instance.MoveInput : Vector2.zero;

        if (stickCooldownTimer > 0f)
        {
            stickCooldownTimer -= Time.deltaTime;
        }

        UpdateMovementParameters();

        if (!isSticking && CanStickToWall() && stickCooldownTimer <= 0f)
        {
            isSticking = true;
            stickTimer = oneHandStuck ? maxStickTime / 2f : maxStickTime;
        }

        if (isSticking && jumpCooldownTimer <= 0f && Mathf.Abs(moveInput.x) > jumpThreshold && preJump != null && preJump.IsPressed())
        {
            shouldJump = true;
            jumpDirection = moveInput.x > 0 ? 1f : -1f; 
            isSticking = false;
            stickCooldownTimer = stickCooldownAfterJump;
        }

        if (isSticking)
        {
            stickTimer -= Time.deltaTime;
            if (stickTimer <= 0f || !CanStickToWall())
            {
                isSticking = false;
            }
        }
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (leftHandRb == null || rightHandRb == null || bodyRb == null) return;

        if (shouldJump)
        {
            ApplyJump(jumpDirection);
            shouldJump = false;
        }

        if (isSticking)
        {
            StickToWall();
            MoveAlongWall(PlayerInputHandler.Instance.MoveInput.y); 
        }
    }

    private void UpdateMovementParameters()
    {
        if (movementStage == null) return;

        switch (movementStage.CurrentStage)
        {
            case BodyPartStage.HeadOnly:
            case BodyPartStage.BodyConnected:
                currentMoveSpeed = baseMoveSpeed;
                currentJumpForce = baseJumpForce;
                break;
            case BodyPartStage.RightArmConnected:
                currentMoveSpeed = oneArmMoveSpeed;
                currentJumpForce = oneArmJumpForce;
                break;
            case BodyPartStage.TwoArmsConnected:
                currentMoveSpeed = twoArmsMoveSpeed;
                currentJumpForce = twoArmsJumpForce;
                break;
            case BodyPartStage.FullyConnected:
                currentMoveSpeed = fullBodyMoveSpeed;
                currentJumpForce = fullBodyJumpForce;
                break;
        }
    }

    private bool CanStickToWall()
    {
        RaycastHit2D leftHit = Physics2D.BoxCast(
            leftHandRb.position,
            wallCheckSize,
            leftHandRb.rotation,
            (leftHandRb.position - bodyRb.position).normalized,
            wallCheckDistance,
            climbWallLayer
        );
        RaycastHit2D rightHit = Physics2D.BoxCast(
            rightHandRb.position,
            wallCheckSize,
            rightHandRb.rotation,
            (rightHandRb.position - bodyRb.position).normalized,
            wallCheckDistance,
            climbWallLayer
        );

        oneHandStuck = (leftHit.collider != null) != (rightHit.collider != null); // True if only one hand hits

        if (leftHit.collider != null || rightHit.collider != null)
        {
            wallNormal = (leftHit.collider != null ? leftHit.normal : rightHit.normal).normalized;
            return true;
        }
        return false;
    }

    private void StickToWall()
    {
        if (oneHandStuck)
        {
            if (Physics2D.BoxCast(leftHandRb.position, wallCheckSize, leftHandRb.rotation, (leftHandRb.position - bodyRb.position).normalized, wallCheckDistance, climbWallLayer))
            {
                leftHandRb.AddForce(-wallNormal * baseStickForce);
            }
            else if (Physics2D.BoxCast(rightHandRb.position, wallCheckSize, rightHandRb.rotation, (rightHandRb.position - bodyRb.position).normalized, wallCheckDistance, climbWallLayer))
            {
                rightHandRb.AddForce(-wallNormal * baseStickForce);
            }
            bodyRb.AddForce(wallNormal * baseStickForce * 0.25f);
        }
        else
        {
            leftHandRb.AddForce(-wallNormal * baseStickForce);
            rightHandRb.AddForce(-wallNormal * baseStickForce);
            bodyRb.AddForce(wallNormal * baseStickForce * 0.5f);
        }
    }

    private void MoveAlongWall(float verticalInput)
    {
        if (Mathf.Abs(verticalInput) > 0.05f)
        {
            float adjustedInput = verticalInput;
            if (verticalInput > 0)
            {
                adjustedInput = wallNormal.x < 0 ? -1f : 1f;
            }
            else if (verticalInput < 0)
            {
                adjustedInput = wallNormal.x < 0 ? 1f : -1f;
            }

            Vector2 wallTangent = new Vector2(-wallNormal.y, wallNormal.x);
            Vector2 moveDirection = wallTangent * adjustedInput * currentMoveSpeed;
            bodyRb.AddForce(moveDirection, ForceMode2D.Force);
            if (!oneHandStuck || Physics2D.BoxCast(leftHandRb.position, wallCheckSize, leftHandRb.rotation, (leftHandRb.position - bodyRb.position).normalized, wallCheckDistance, climbWallLayer))
                leftHandRb.AddForce(moveDirection * 0.5f, ForceMode2D.Force);
            if (!oneHandStuck || Physics2D.BoxCast(rightHandRb.position, wallCheckSize, rightHandRb.rotation, (rightHandRb.position - bodyRb.position).normalized, wallCheckDistance, climbWallLayer))
                rightHandRb.AddForce(moveDirection * 0.5f, ForceMode2D.Force);
        }
    }

    private void ApplyJump(float direction)
    {
        if (wallNormal == Vector2.zero)
        {
            wallNormal = -Vector2.up;
        }

        Vector2 baseDirection = -wallNormal + Vector2.right * direction;
        Vector2 jumpDirection = baseDirection.magnitude > 0.1f ? (baseDirection.normalized + Vector2.up).normalized : Vector2.up;

        float adjustedJumpForce = oneHandStuck ? currentJumpForce / 3f : currentJumpForce;
        bodyRb.AddForce(jumpDirection * adjustedJumpForce, ForceMode2D.Impulse);
        jumpCooldownTimer = jumpCooldownDuration;
    }

    void OnDrawGizmos()
    {
        Vector2 checkDir = bodyRb != null ? (leftHandRb.position - bodyRb.position).normalized : Vector2.right;

        if (leftHandRb != null)
        {
            Gizmos.color = CanStickToWall() ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                leftHandRb.position,
                Quaternion.Euler(0, 0, leftHandRb.rotation),
                Vector3.one
            );
            Gizmos.DrawWireCube(
                checkDir * wallCheckDistance,
                new Vector3(wallCheckSize.x, wallCheckSize.y, 1)
            );
        }
        if (rightHandRb != null)
        {
            Gizmos.color = CanStickToWall() ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                rightHandRb.position,
                Quaternion.Euler(0, 0, rightHandRb.rotation),
                Vector3.one
            );
            Gizmos.DrawWireCube(
                checkDir * wallCheckDistance,
                new Vector3(wallCheckSize.x, wallCheckSize.y, 1)
            );
        }
        Gizmos.matrix = Matrix4x4.identity;
    }
}