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
    [SerializeField] private float baseStickForce;
    [SerializeField] private float maxStickTime;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private float wallCheckDistance;

    [Header("Jump Settings")]
    [SerializeField] private float jumpCooldownDuration;
    [SerializeField] private float jumpThreshold; 
    [SerializeField] private float postJumpStickCooldownDuration; 

    [Header("Stage-Specific Settings")]
    [SerializeField] private float baseJumpForce;
    [SerializeField] private float oneArmJumpForce;
    [SerializeField] private float twoArmsJumpForce;
    [SerializeField] private float fullBodyJumpForce;

    [Header("Input")]
    [SerializeField] private InputAction preJump;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource stickSoundSource;

    private bool isSticking = false;
    private float stickTimer;
    private float jumpCooldownTimer;
    private float postJumpStickCooldownTimer;
    private Vector2 wallNormal;
    private bool oneHandStuck = false;
    private float currentJumpForce;
    private bool shouldJumpHorizontal = false;
    private bool shouldJumpVertical = false;
    private Vector2 jumpDirection;
    private bool wasSticking = false;

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

        UpdateMovementParameters();

        if (!isSticking && CanStickToWall() && postJumpStickCooldownTimer <= 0f)
        {
            isSticking = true;
            stickTimer = oneHandStuck ? maxStickTime / 2f : maxStickTime;
        }

        if (isSticking && jumpCooldownTimer <= 0f && preJump != null && preJump.IsPressed() && Mathf.Abs(moveInput.x) > jumpThreshold)
        {
            shouldJumpHorizontal = true;
            jumpDirection = new Vector2(moveInput.x > 0 ? 1f : -1f, 0f); 
            isSticking = false;
            postJumpStickCooldownTimer = postJumpStickCooldownDuration; 
        }

        if (isSticking && jumpCooldownTimer <= 0f && preJump != null && preJump.IsPressed() && Mathf.Abs(moveInput.y) > jumpThreshold)
        {
            shouldJumpVertical = true;
            jumpDirection = new Vector2(0f, moveInput.y > 0 ? 1f : -1f); 
            isSticking = false;
            postJumpStickCooldownTimer = postJumpStickCooldownDuration; 
        }

        if (isSticking)
        {
            stickTimer -= Time.deltaTime;
            if (stickTimer <= 0f || !CanStickToWall())
            {
                isSticking = false;
            }
        }

        if (isSticking && !wasSticking && stickSoundSource != null)
        {
            stickSoundSource.Play();
        }
        wasSticking = isSticking; 

        if (postJumpStickCooldownTimer > 0f)
        {
            postJumpStickCooldownTimer -= Time.deltaTime;
        }

        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (leftHandRb == null || rightHandRb == null || bodyRb == null) return;

        if (shouldJumpHorizontal || shouldJumpVertical)
        {
            ApplyJump(jumpDirection, shouldJumpVertical);
            shouldJumpHorizontal = false;
            shouldJumpVertical = false;
        }

        if (isSticking)
        {
            StickToWall();
        }
    }

    private void UpdateMovementParameters()
    {
        if (movementStage == null) return;

        switch (movementStage.CurrentStage)
        {
            case BodyPartStage.HeadOnly:
            case BodyPartStage.BodyConnected:
                currentJumpForce = baseJumpForce;
                break;
            case BodyPartStage.RightArmConnected:
                currentJumpForce = oneArmJumpForce;
                break;
            case BodyPartStage.TwoArmsConnected:
                currentJumpForce = twoArmsJumpForce;
                break;
            case BodyPartStage.FullyConnected:
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

    private void ApplyJump(Vector2 direction, bool isVerticalJump)
    {
        if (wallNormal == Vector2.zero)
        {
            wallNormal = -Vector2.up;
        }

        Vector2 baseDirection = -wallNormal + direction;
        Vector2 jumpDirection = baseDirection.magnitude > 0.1f ? baseDirection.normalized : Vector2.up;

        if (isVerticalJump)
        {
            jumpDirection = new Vector2(0f, direction.y > 0 ? 1f : -1f).normalized + Vector2.up * 0.5f;
        }

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