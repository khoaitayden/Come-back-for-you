using UnityEngine;

public class AutoMovingPlatformX : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private float rightPositionX; // X position when platform moves right
    [SerializeField] private float leftPositionX; // X position when platform moves left
    [SerializeField] private float moveSpeed = 5f; // Speed of direct movement
    [SerializeField] private bool moveRightWhenStepped = true; // Toggle to choose right or left when player steps on
    [SerializeField] private float moveDelay = 1f; // Delay before platform moves after player steps on

    private Rigidbody2D platformRigidbody;
    private Vector2 targetPosition;
    private bool playerOnPlatform = false;
    private float delayTimer = 0f;
    private bool isDelayed = false;

    void Awake()
    {
        // Initialize platform components
        platformRigidbody = GetComponent<Rigidbody2D>();
        if (platformRigidbody == null)
        {
            Debug.LogError("Rigidbody2D not found on platform GameObject!");
        }

        // Set initial position to left
        targetPosition = new Vector2(leftPositionX, transform.position.y);
    }

    void Start()
    {
        // Ensure platform starts at left position
        transform.position = new Vector2(leftPositionX, transform.position.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Start delay when player collides with platform
        if (collision.collider.CompareTag("Player") && !playerOnPlatform)
        {
            playerOnPlatform = true;
            isDelayed = true;
            delayTimer = 0f;
            Debug.Log("Player collided with platform, starting delay before moving!");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Move platform to opposite position immediately when player steps off
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = false;
            isDelayed = false;
            targetPosition = new Vector2(moveRightWhenStepped ? leftPositionX : rightPositionX, transform.position.y);
            Debug.Log("Player stepped off platform, returning to " + (moveRightWhenStepped ? "left" : "right") + " position!");
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
                    Debug.Log("Delay finished, moving platform " + (moveRightWhenStepped ? "right" : "left") + "!");
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (platformRigidbody == null) return;

        // Direct position-based movement
        platformRigidbody.MovePosition(Vector2.MoveTowards(platformRigidbody.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }
}