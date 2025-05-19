using UnityEngine;

public class ButtonPlatformX : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Sprite offSprite; 
    [SerializeField] private Sprite onSprite;
    [SerializeField] private bool startOn = false; 

    [Header("Platform Settings")]
    [SerializeField] private GameObject platform; 
    [SerializeField] private float moveDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveDelay;
    [SerializeField] private bool shouldAutoOff = true; 
    [SerializeField] private float autoOffDelay;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D platformRigidbody;
    private bool isOn = false;
    private Vector2 targetPosition;
    private float delayTimer = 0f;
    private bool isDelayed = false;
    private bool targetIsOn = false; 
    private float leftPositionX;
    private float rightPositionX;
    private float autoOffTimer = 0f;
    private bool playerOnPlatform = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (platform)
        {
            platformRigidbody = platform.GetComponent<Rigidbody2D>();
        }
        isOn = startOn;
        spriteRenderer.sprite = isOn ? onSprite : offSprite;
    }

    void Start()
    {
        if (!platform) return;

        leftPositionX = platform.transform.position.x;
        rightPositionX = leftPositionX + moveDistance;
        targetPosition = new Vector2(isOn ? rightPositionX : leftPositionX, platform.transform.position.y);
        platform.transform.position = targetPosition; 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger && other.CompareTag("Player"))
        {
            ToggleButton();
            playerOnPlatform = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.isTrigger && other.CompareTag("Player"))
        {
            playerOnPlatform = false;
            if (shouldAutoOff && isOn)
            {
                autoOffTimer = 0f;
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

        isDelayed = true;
        delayTimer = 0f;
        targetIsOn = isOn;
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
            }
        }

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
                autoOffTimer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (!platformRigidbody) return;

        platformRigidbody.MovePosition(Vector2.MoveTowards(platformRigidbody.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }

    void OnDrawGizmos()
    {
        if (!platform) return;

        float gizmoLeftX = platform.transform.position.x;
        float gizmoRightX = gizmoLeftX + moveDistance;
        Vector3 leftPos = new Vector3(gizmoLeftX, platform.transform.position.y, platform.transform.position.z);
        Vector3 rightPos = new Vector3(gizmoRightX, platform.transform.position.y, platform.transform.position.z);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(leftPos, rightPos);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(leftPos, 0.2f);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rightPos, 0.2f);

    }
}