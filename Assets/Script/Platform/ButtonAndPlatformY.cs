using UnityEngine;

public class ButtonPlatformY : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Sprite offSprite; // Sprite when button is off
    [SerializeField] private Sprite onSprite; // Sprite when button is on
    [SerializeField] private bool startOn = false; // Toggle to choose starting state (on or off)

    [Header("Platform Settings")]
    [SerializeField] private GameObject platform; // Reference to the platform GameObject
    [SerializeField] private float highPositionY; // Y position when platform is up
    [SerializeField] private float lowPositionY; // Y position when platform is down
    [SerializeField] private float moveSpeed = 5f; // Speed of direct movement

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D platformRigidbody;
    private bool isOn = false;
    private Vector2 targetPosition;

    void Awake()
    {
        // Initialize button components (self)
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on button GameObject!");
        }

        // Initialize platform components
        if (platform != null)
        {
            platformRigidbody = platform.GetComponent<Rigidbody2D>();
            if (platformRigidbody == null)
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

        // Set initial platform position based on startOn
        if (platform != null)
        {
            targetPosition = new Vector2(platform.transform.position.x, isOn ? highPositionY : lowPositionY);
        }
    }

    void Start()
    {
        // Ensure platform starts at the correct position
        if (platform != null)
        {
            platform.transform.position = new Vector2(platform.transform.position.x, isOn ? highPositionY : lowPositionY);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Trigger button toggle only when a "Player" tagged non-trigger Collider2D enters
        if (other.isTrigger == false && other.CompareTag("Player"))
        {
            ToggleButton();
            Debug.Log("Button toggled by Player!");
        }
    }

    public void ToggleButton()
    {
        isOn = !isOn;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isOn ? onSprite : offSprite;
        }
        targetPosition = new Vector2(platform.transform.position.x, isOn ? highPositionY : lowPositionY);
    }

    void FixedUpdate()
    {
        if (platformRigidbody == null) return;

        // Direct position-based movement
        float targetY = isOn ? highPositionY : lowPositionY;
        targetPosition = new Vector2(platform.transform.position.x, targetY);
        platformRigidbody.MovePosition(Vector2.MoveTowards(platformRigidbody.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }
}