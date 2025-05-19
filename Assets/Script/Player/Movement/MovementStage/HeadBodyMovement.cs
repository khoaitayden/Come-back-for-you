using UnityEngine;
using UnityEngine.InputSystem;

public class HeadBodyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D headRb;
    [SerializeField] private Rigidbody2D bodyRb; 
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] private float headMoveForce;
    [SerializeField] private float jumpForce;
    [SerializeField] private Vector2 groundCheckSize; 
    [SerializeField] private float bodyInfluenceFactor; 
    [SerializeField] private float groundCheckDistance; 

    private Vector2 currentInput = Vector2.zero;
    private bool isJumping = false;
    private bool isGrounded = false;

    void Update()
    {
        if (PlayerInputHandler.Instance != null)
        {
            currentInput = PlayerInputHandler.Instance.MoveInput;
        }

        if (bodyRb != null)
        {
            isGrounded = Physics2D.BoxCast(
                bodyRb.position,
                groundCheckSize,
                bodyRb.rotation,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            );
        }

        // Jump check
        if (isGrounded && PlayerInputHandler.Instance != null && PlayerInputHandler.Instance.MoveInput.y > 0.1f)
        {
            isJumping = true;
        }
    }

    void FixedUpdate()
    {
        if (headRb == null) return;

        if (Mathf.Abs(currentInput.x) > 0.05f)
        {
            Vector2 moveForce = Vector2.right * currentInput.x * headMoveForce;
            headRb.AddForce(moveForce, ForceMode2D.Force);
            
            if (bodyRb != null) 
            {
                bodyRb.AddForce(moveForce * bodyInfluenceFactor, ForceMode2D.Force);
            }
        }

        // Jumping
        if (isJumping)
        {
            Vector2 jumpImpulse = Vector2.up * jumpForce;
            headRb.AddForce(jumpImpulse, ForceMode2D.Impulse);
            if (bodyRb != null) bodyRb.AddForce(jumpImpulse * bodyInfluenceFactor, ForceMode2D.Impulse);
            isJumping = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (bodyRb != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                bodyRb.position,
                Quaternion.Euler(0, 0, bodyRb.rotation),
                Vector3.one
            );
            Gizmos.DrawWireCube(
                Vector2.down * groundCheckDistance,
                new Vector3(groundCheckSize.x, groundCheckSize.y, 1)
            );
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}