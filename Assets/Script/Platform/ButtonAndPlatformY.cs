using UnityEngine;

public class ButtonPlatformY : MonoBehaviour
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

    [Header("Sound Settings")]
    [SerializeField] private AudioSource buttonToggleSoundSource;
    [SerializeField] private AudioSource moveSoundSource;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D platformRigidbody;
    private bool isOn = false;
    private Vector2 targetPosition;
    private float delayTimer = 0f;
    private bool isDelayed = false;
    private bool targetIsOn = false;
    private float lowPositionY;
    private float highPositionY;
    private float autoOffTimer = 0f;
    private bool playerOnPlatform = false;
    private bool isMoving = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (platform)
        {
            platformRigidbody = platform.GetComponent<Rigidbody2D>();
        }
        isOn = startOn;
        if (spriteRenderer)
            spriteRenderer.sprite = isOn ? onSprite : offSprite;
    }

    void Start()
    {
        if (!platform) return;

        lowPositionY = platform.transform.position.y;
        highPositionY = lowPositionY + moveDistance;
        targetPosition = new Vector2(platform.transform.position.x, isOn ? highPositionY : lowPositionY);
        platform.transform.position = targetPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger && other.CompareTag("Player"))
        {
            ToggleButton();
            playerOnPlatform = true;
            if (buttonToggleSoundSource != null)
            {
                buttonToggleSoundSource.Play();
            }
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
                targetPosition = new Vector2(platform.transform.position.x, targetIsOn ? highPositionY : lowPositionY);
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
                targetPosition = new Vector2(platform.transform.position.x, lowPositionY);
                autoOffTimer = 0f;
            }
        }

        bool wasMoving = isMoving;
        isMoving = Vector2.Distance(platformRigidbody.position, targetPosition) > 0.01f;

        // Play sound only when moving
        if (moveSoundSource != null)
        {
            if (isMoving && !wasMoving)
            {
                moveSoundSource.Play();
            }
            else if (!isMoving && wasMoving)
            {
                moveSoundSource.Stop();
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

        float gizmoLowY = platform.transform.position.y;
        float gizmoHighY = gizmoLowY + moveDistance;
        Vector3 lowPos = new Vector3(platform.transform.position.x, gizmoLowY, platform.transform.position.z);
        Vector3 highPos = new Vector3(platform.transform.position.x, gizmoHighY, platform.transform.position.z);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(lowPos, highPos);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lowPos, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(highPos, 0.2f);
    }
}