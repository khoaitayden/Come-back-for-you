using UnityEngine;

public class AutoMovingPlatformX : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private float moveDistance = 5f; // Distance to move right from initial position
    [SerializeField] private float moveSpeed = 5f; // Speed of movement
    [SerializeField] private bool moveRightWhenStepped = true; // Move right or left when stepped on
    [SerializeField] private float moveDelay = 1f; // Delay before moving

    private Rigidbody2D platformRigidbody;
    private Vector2 targetPosition;
    private bool playerOnPlatform = false;
    private float delayTimer = 0f;
    private bool isDelayed = false;
    private float leftPositionX;
    private float rightPositionX;

    void Awake()
    {
        // Initialize platform components
        platformRigidbody = GetComponent<Rigidbody2D>();
        if (!platformRigidbody)
        {
            Debug.LogError("Rigidbody2D not found on platform GameObject!");
        }
    }

    void Start()
    {
        // Set left position to initial X, right position as offset
        leftPositionX = transform.position.x;
        rightPositionX = leftPositionX + moveDistance;
        targetPosition = new Vector2(leftPositionX, transform.position.y);
        transform.position = targetPosition; // Ensure exact start position
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !playerOnPlatform)
        {
            playerOnPlatform = true;
            isDelayed = true;
            delayTimer = 0f;
            Debug.Log("Player stepped on platform, starting delay!");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = false;
            isDelayed = false;
            targetPosition = new Vector2(moveRightWhenStepped ? leftPositionX : rightPositionX, transform.position.y);
            Debug.Log($"Player stepped off, moving to {(moveRightWhenStepped ? "left" : "right")} position!");
        }
    }

    void Update()
    {
        if (isDelayed)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= moveDelay)
            {
                isDelayed = false;
                if (playerOnPlatform)
                {
                    targetPosition = new Vector2(moveRightWhenStepped ? rightPositionX : leftPositionX, transform.position.y);
                    Debug.Log($"Delay finished, moving {(moveRightWhenStepped ? "right" : "left")}!");
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (platformRigidbody)
        {
            platformRigidbody.MovePosition(Vector2.MoveTowards(platformRigidbody.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
        }
    }

    // Visualize the travel distance in the Scene view
    void OnDrawGizmos()
    {
        // Use current position for X and Y, calculate positions based on moveDistance
        float gizmoLeftX = transform.position.x;
        float gizmoRightX = gizmoLeftX + moveDistance;
        Vector3 leftPos = new Vector3(gizmoLeftX, transform.position.y, transform.position.z);
        Vector3 rightPos = new Vector3(gizmoRightX, transform.position.y, transform.position.z);

        // Draw a green line between left and right positions
        Gizmos.color = Color.green;
        Gizmos.DrawLine(leftPos, rightPos);

        // Draw blue sphere at left position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(leftPos, 0.2f);

        // Draw red sphere at right position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rightPos, 0.2f);

    }
}