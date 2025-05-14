using UnityEngine;

public class HeadBodyAndTwoArms : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D headRb;    
    [SerializeField] private Rigidbody2D bodyRb;   
    [SerializeField] private Rigidbody2D leftArmRb;
    [SerializeField] private Rigidbody2D rightArmRb;
    [SerializeField] private LayerMask groundLayer; 

    [Header("Movement Settings")]
    [SerializeField] private float spinTorque; 
    [SerializeField] private float jumpForce;  
    [SerializeField] private float headInfluence; 
    [SerializeField] private float bodyInfluence;
    [SerializeField] private float midAirControlForce; 

    [Header("Ground Check Settings")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(1.9f, 0.56f);
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Vector2 moveInput; 
    private bool wantsToJump;  
    private bool isGrounded;   
    private int groundedArmsCount; 

    void Start()
    {
    }

    void Update()
    {
        // Get player input
        moveInput = PlayerInputHandler.Instance != null ? PlayerInputHandler.Instance.MoveInput : Vector2.zero;

        // Check if arms are grounded
        bool leftArmGrounded = Physics2D.BoxCast(
            leftArmRb.position,
            groundCheckSize,
            leftArmRb.rotation,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        bool rightArmGrounded = Physics2D.BoxCast(
            rightArmRb.position,
            groundCheckSize,
            rightArmRb.rotation,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        // Count grounded arms
        groundedArmsCount = (leftArmGrounded ? 1 : 0) + (rightArmGrounded ? 1 : 0);
        isGrounded = groundedArmsCount > 0; // At least one arm grounded

        // Check for jump input
        wantsToJump = isGrounded && moveInput.y > 0.1f && Mathf.Abs(moveInput.x) < 0.05f;
    }

    void FixedUpdate()
    {
        if (headRb == null || leftArmRb == null || rightArmRb == null || bodyRb == null) return;

        // Spinning: Apply torque to arms to spin in the same direction
        if (Mathf.Abs(moveInput.x) > 0.05f)
        {
            float spinDirection = moveInput.x > 0 ? -1f : 1f; // Spin clockwise for right, counterclockwise for left
            leftArmRb.AddTorque(spinDirection * spinTorque, ForceMode2D.Force);
            rightArmRb.AddTorque(spinDirection * spinTorque, ForceMode2D.Force);
        }

        // Mid-air control: Apply small horizontal force to body when not grounded
        if (!isGrounded && Mathf.Abs(moveInput.x) > 0.05f)
        {
            Vector2 controlForce = Vector2.right * moveInput.x * midAirControlForce;
            bodyRb.AddForce(controlForce, ForceMode2D.Force);
        }

        // Jump: Apply upward force to both arms, scaled by number of grounded arms
        if (wantsToJump)
        {
            // Scale jump force: full strength with 2 arms, 2/3 strength with 1 arm
            float adjustedJumpForce = jumpForce * (groundedArmsCount == 2 ? 1f : 2f / 3f);

            Vector2 upwardForce = Vector2.up * adjustedJumpForce;
            leftArmRb.AddForce(upwardForce, ForceMode2D.Impulse);
            rightArmRb.AddForce(upwardForce, ForceMode2D.Impulse);
            headRb.AddForce(upwardForce * headInfluence, ForceMode2D.Impulse);
            bodyRb.AddForce(upwardForce * bodyInfluence, ForceMode2D.Impulse);
            wantsToJump = false;
        }
    }

    void OnDrawGizmos()
    {
        // Draw ground check boxes for both arms (green if grounded, red if not)
        if (leftArmRb != null)
        {
            Gizmos.color = Physics2D.BoxCast(
                leftArmRb.position,
                groundCheckSize,
                leftArmRb.rotation,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            ) ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                leftArmRb.position,
                Quaternion.Euler(0, 0, leftArmRb.rotation),
                Vector3.one
            );
            Gizmos.DrawWireCube(
                Vector2.down * groundCheckDistance,
                new Vector3(groundCheckSize.x, groundCheckSize.y, 1)
            );
        }

        if (rightArmRb != null)
        {
            Gizmos.color = Physics2D.BoxCast(
                rightArmRb.position,
                groundCheckSize,
                rightArmRb.rotation,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            ) ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                rightArmRb.position,
                Quaternion.Euler(0, 0, rightArmRb.rotation),
                Vector3.one
            );
            Gizmos.DrawWireCube(
                Vector2.down * groundCheckDistance,
                new Vector3(groundCheckSize.x, groundCheckSize.y, 1)
            );
        }
        Gizmos.matrix = Matrix4x4.identity;
    }

    public void SetArmRbs(Rigidbody2D newLeftArmRb, Rigidbody2D newRightArmRb)
    {
        leftArmRb = newLeftArmRb;
        rightArmRb = newRightArmRb;
    }
}