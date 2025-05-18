using UnityEngine;

public class ButtonPlatformX : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Sprite offSprite; // Sprite when button is off
    [SerializeField] private Sprite onSprite; // Sprite when button is on
    [SerializeField] private bool startOn = false; // Toggle to choose starting state (on or off)

    [Header("Platform Settings")]
    [SerializeField] private GameObject platform; // Reference to the platform GameObject
    [SerializeField] private float moveDistance = 5f; // Distance to move right from initial position
    [SerializeField] private float moveSpeed = 5f; // Speed of movement
    [SerializeField] private float moveDelay = 1f; // Delay before platform moves after button toggle
    [SerializeField] private bool shouldAutoOff = true; // Toggle to enable/disable auto-off
    [SerializeField] private float autoOffDelay = 2f; // Time before platform returns to off state

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D platformRigidbody;
    private bool isOn = false;
    private Vector2 targetPosition;
    private float delayTimer = 0f;
    private bool isDelayed = false;
    private bool targetIsOn = false; // Tracks the target state after delay
    private float leftPositionX;
    private float rightPositionX;
    private float autoOffTimer = 0f;
    private bool playerOnPlatform = false;

    void Awake()
    {
        // Initialize button components (self)
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            Debug.LogError("SpriteRenderer not found on button GameObject!");
        }

        // Initialize platform components
        if (platform)
        {
            platformRigidbody = platform.GetComponent<Rigidbody2D>();
            if (!platformRigidbody)
            {
                Debug.LogError("Rigidbody2D not found on platform GameObject!");
            }
        }
        else
        {
            Debug.LogError("Platform GameObject not assigned!");
        }

        // Set initial state based on startOn toggle
        isOn = startOn;
        spriteRenderer.sprite = isOn ? onSprite : offSprite;
    }

    void Start()
    {
        if (!platform) return;

        // Set left position to initial X, right position as offset
        leftPositionX = platform.transform.position.x;
        rightPositionX = leftPositionX + moveDistance;
        targetPosition = new Vector2(isOn ? rightPositionX : leftPositionX, platform.transform.position.y);
        platform.transform.position = targetPosition; // Ensure exact start position
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Trigger button toggle only when a "Player" tagged non-trigger Collider2D enters
        if (!other.isTrigger && other.CompareTag("Player"))
        {
            ToggleButton();
            playerOnPlatform = true;
            Debug.Log("Button toggled by Player!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Start auto-off timer when player steps off
        if (!other.isTrigger && other.CompareTag("Player"))
        {
            playerOnPlatform = false;
            if (shouldAutoOff && isOn)
            {
                autoOffTimer = 0f;
                Debug.Log($"Player stepped off, starting auto-off timer of {autoOffDelay} seconds!");
            }
        }
    }

    public void ToggleButton()
    {
        isOn = !isOn;
        if (spriteRenderer)
        {
            spriteRenderer.sprite = isOn ? onSprite : offSprite;
        }

        // Start delay before moving platform
        isDelayed = true;
        delayTimer = 0f;
        targetIsOn = isOn;
        Debug.Log($"Button toggled to {(isOn ? "on" : "off")}, starting delay of {moveDelay} seconds before platform moves!");
    }

    void Update()
    {
        if (isDelayed)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= moveDelay)
            {
                isDelayed = false;
                targetPosition = new Vector2(targetIsOn ? rightPositionX : leftPositionX, platform.transform.position.y);
                Debug.Log($"Delay finished, moving platform to {(targetIsOn ? "right" : "left")} position!");
            }
        }

        // Handle auto-off timer only after player steps off
        if (!playerOnPlatform && shouldAutoOff && isOn)
        {
            autoOffTimer += Time.deltaTime;
            if (autoOffTimer >= autoOffDelay)
            {
                isOn = false;
                if (spriteRenderer)
                {
                    spriteRenderer.sprite = offSprite;
                }
                targetPosition = new Vector2(leftPositionX, platform.transform.position.y);
                Debug.Log("Platform auto-returned to off state!");
                autoOffTimer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (!platformRigidbody) return;

        // Direct position-based movement
        platformRigidbody.MovePosition(Vector2.MoveTowards(platformRigidbody.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }

    // Visualize the platform's travel distance in the Scene view
    void OnDrawGizmos()
    {
        if (!platform) return;

        // Use platform's initial position for left, calculate right position
        float gizmoLeftX = platform.transform.position.x;
        float gizmoRightX = gizmoLeftX + moveDistance;
        Vector3 leftPos = new Vector3(gizmoLeftX, platform.transform.position.y, platform.transform.position.z);
        Vector3 rightPos = new Vector3(gizmoRightX, platform.transform.position.y, platform.transform.position.z);

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