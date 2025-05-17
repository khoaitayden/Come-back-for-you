using UnityEngine;

public class AutoMovingPlatformY : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private float highPositionY; // Y position when platform moves up
    [SerializeField] private float lowPositionY; // Y position when platform is down
    [SerializeField] private float moveSpeed = 5f; // Speed of direct movement
    [SerializeField] private bool moveUpWhenStepped = true; // Toggle to choose up or down when player steps on
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

        // Set initial position to low
        targetPosition = new Vector2(transform.position.x, lowPositionY);
    }

    void Start()
    {
        // Ensure platform starts at low position
        transform.position = new Vector2(transform.position.x, lowPositionY);
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
            targetPosition = new Vector2(transform.position.x, moveUpWhenStepped ? lowPositionY : highPositionY);
            Debug.Log("Player stepped off platform, returning to " + (moveUpWhenStepped ? "low" : "high") + " position!");
        }
    }

    void Update()
    {
        // Handle delay timer
        if (isDelayed)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= moveDelay)
            {
                isDelayed = false;
                if (playerOnPlatform)
                {
                    targetPosition = new Vector2(transform.position.x, moveUpWhenStepped ? highPositionY : lowPositionY);
                    Debug.Log("Delay finished, moving platform " + (moveUpWhenStepped ? "up" : "down") + "!");
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