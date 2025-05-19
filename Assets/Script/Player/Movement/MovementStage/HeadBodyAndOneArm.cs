using UnityEngine;

public class HeadBodyAndOneArm : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D headRb;
    [SerializeField] private Rigidbody2D bodyRb;
    [SerializeField] private Rigidbody2D armRb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Movement Settings")]
    [SerializeField] private float spinTorque ;
    [SerializeField] private float jumpForce ;
    [SerializeField] private float headInfluence ;
    [SerializeField] private float bodyInfluence ;
    [SerializeField] private float midAirControlForce ;

    [Header("Ground Check Settings")]
    [SerializeField] private Vector2 groundCheckSize ;
    [SerializeField] private float groundCheckDistance ;

    [Header("Wall Check Settings")]
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float wallJumpForceReduction;

    private Vector2 moveInput;
    private bool wantsToJump;
    private bool isGrounded;
    private bool isTouchingWall;

    void Update()
    {
        GetPlayerInput();
        CheckGrounded();
        CheckWallTouch();
        CheckJumpInput();
    }

    void FixedUpdate()
    {
        if (headRb == null || armRb == null) return;

        HandleHorizontalMovement();
        HandleMidAirControl();
        HandleJump();
    }

    private void GetPlayerInput()
    {
        moveInput = PlayerInputHandler.Instance != null ? PlayerInputHandler.Instance.MoveInput : Vector2.zero;
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.BoxCast(
            armRb.position,
            groundCheckSize,
            armRb.rotation,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    private void CheckWallTouch()
    {
        isTouchingWall = 
        Physics2D.Raycast(armRb.position, Vector2.right, wallCheckDistance, wallLayer) 
        || 
        Physics2D.Raycast(armRb.position, Vector2.left, wallCheckDistance, wallLayer);
    }

    private void CheckJumpInput()
    {
        wantsToJump = isGrounded && moveInput.y > 0.1f && Mathf.Abs(moveInput.x) < 0.05f;
    }

    private void HandleHorizontalMovement()
    {
        if (Mathf.Abs(moveInput.x) > 0.05f)
        {
            float torque = -moveInput.x * spinTorque;
            armRb.AddTorque(torque, ForceMode2D.Force);

            Vector2 followForce = Vector2.right * moveInput.x * spinTorque * 0.1f;
            headRb.AddForce(followForce * headInfluence, ForceMode2D.Force);
            if (bodyRb != null)
            {
                bodyRb.AddForce(followForce * bodyInfluence, ForceMode2D.Force);
            }
        }
    }

    private void HandleMidAirControl()
    {
        if (!isGrounded && Mathf.Abs(moveInput.x) > 0.05f)
        {
            Vector2 controlForce = Vector2.right * moveInput.x * midAirControlForce;
            bodyRb.AddForce(controlForce, ForceMode2D.Force);
        }
    }

    private void HandleJump()
    {
        if (wantsToJump)
        {
            float adjustedJumpForce = jumpForce;
            if (isTouchingWall)
            {
                adjustedJumpForce *= wallJumpForceReduction;
            }

            Vector2 upwardForce = Vector2.up * adjustedJumpForce;
            armRb.AddForce(upwardForce, ForceMode2D.Impulse);
            headRb.AddForce(upwardForce * headInfluence, ForceMode2D.Impulse);
            if (bodyRb != null)
            {
                bodyRb.AddForce(upwardForce * bodyInfluence, ForceMode2D.Impulse);
            }

            wantsToJump = false;
        }
    }

    private void DrawGroundCheckGizmo()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.matrix = Matrix4x4.TRS(
            armRb.position,
            Quaternion.Euler(0, 0, armRb.rotation),
            Vector3.one
        );
        Gizmos.DrawWireCube(
            Vector2.down * groundCheckDistance,
            new Vector3(groundCheckSize.x, groundCheckSize.y, 1)
        );
        Gizmos.matrix = Matrix4x4.identity;
    }

    private void DrawWallCheckGizmo()
    {
        Gizmos.color = isTouchingWall ? Color.red : Color.yellow;
        Gizmos.DrawRay(armRb.position, Vector2.right * wallCheckDistance);
        Gizmos.DrawRay(armRb.position, Vector2.left * wallCheckDistance);
    }

    void OnDrawGizmos()
    {
        if (armRb != null)
        {
            DrawGroundCheckGizmo();
            DrawWallCheckGizmo();
        }
    }

    public void SetArmRb(Rigidbody2D newArmRb)
    {
        armRb = newArmRb;
    }
}