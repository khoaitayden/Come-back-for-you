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
    // Overall walking speed
    [SerializeField] private float stepFrequency = 0.5f; 
    [SerializeField] private float stepForce = 10f;      
    [SerializeField] private float legSwingTorque = 5f;  
    [SerializeField] private float stabilizationForce = 2f; 
    [SerializeField] private float balanceTorque = 3f;  
    [SerializeField] private float armBalanceTorque = 1.5f; 
    [SerializeField] private float legUprightTorque = 15f; 

    [Header("Ground Check Settings")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.73f, 0.14f); 
    [SerializeField] private float groundCheckDistance = 0.01f; 

    private Vector2 moveInput; 
    private float stepTimer;   
    private bool isLeftLegStep;
    private bool isLegGrounded;

    void Start()
    {

        stepTimer = stepFrequency; 
        isLeftLegStep = true;    
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

        // Update step timer
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
        HandleWalking();
        KeepLegsUpright();
    }

    private void MaintainBalance()
    {
        float bodyAngle = bodyRb.rotation;
        float balanceCorrection = -Mathf.Clamp(bodyAngle, -45f, 45f) * balanceTorque; // Limit correction range
        bodyRb.AddTorque(balanceCorrection, ForceMode2D.Force);

        bodyRb.AddForce(Vector2.up * stabilizationForce, ForceMode2D.Force);

        // Use arms for counterbalance
        float armBalance = Mathf.Clamp(bodyAngle, -30f, 30f) * armBalanceTorque;
        leftArmRb.AddTorque(-armBalance, ForceMode2D.Force);
        rightArmRb.AddTorque(armBalance, ForceMode2D.Force);
    }

    private void HandleWalking()
    {
        if (Mathf.Abs(moveInput.x) > 0.05f && isLegGrounded)
        {
            Rigidbody2D steppingLeg = isLeftLegStep ? leftLegRb : rightLegRb;
            Rigidbody2D supportingLeg = isLeftLegStep ? rightLegRb : leftLegRb;

            // Check if the stepping leg is grounded
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

            // Adjust body angle
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