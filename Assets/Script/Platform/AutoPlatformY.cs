using UnityEngine;

public class AutoMovingPlatformY : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private float highPositionY; 
    [SerializeField] private float lowPositionY; 
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private bool moveUpWhenStepped = true; 
    [SerializeField] private float moveDelay = 1f; 

    private Rigidbody2D platformRigidbody;
    private Vector2 targetPosition;
    private bool playerOnPlatform = false;
    private float delayTimer = 0f;
    private bool isDelayed = false;

    void Awake()
    {
        platformRigidbody = GetComponent<Rigidbody2D>();
        if (platformRigidbody == null)
        {
            Debug.LogError("Rigidbody2D not found on platform GameObject!");
        }

        targetPosition = new Vector2(transform.position.x, lowPositionY);
    }

    void Start()
    {

        transform.position = new Vector2(transform.position.x, lowPositionY);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
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

    // Visualize the travel distance in the Scene view
    void OnDrawGizmos()
    {
        // Define positions for low and high points
        Vector3 lowPos = new Vector3(transform.position.x, lowPositionY, transform.position.z);
        Vector3 highPos = new Vector3(transform.position.x, highPositionY, transform.position.z);

        // Draw a green line between low and high positions
        Gizmos.color = Color.green;
        Gizmos.DrawLine(lowPos, highPos);

        // Draw blue sphere at low position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lowPos, 0.2f);

        // Draw red sphere at high position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(highPos, 0.2f);

    }
}