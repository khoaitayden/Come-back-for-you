using UnityEngine;

public class AutoMovingPlatformY : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private float highPositionY; 
    [SerializeField] private float lowPositionY; 
    [SerializeField] private float moveSpeed; 
    [SerializeField] private bool moveUpWhenStepped = true; 
    [SerializeField] private float moveDelay; 

    [Header("Sound Settings")]
    [SerializeField] private AudioSource moveSoundSource;

    private Rigidbody2D platformRigidbody;
    private Vector2 targetPosition;
    private bool playerOnPlatform = false;
    private float delayTimer = 0f;
    private bool isDelayed = false;
    private bool isMoving = false;

    void Awake()
    {
        platformRigidbody = GetComponent<Rigidbody2D>();
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
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = false;
            isDelayed = false;
            targetPosition = new Vector2(transform.position.x, moveUpWhenStepped ? lowPositionY : highPositionY);
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
                    targetPosition = new Vector2(transform.position.x, moveUpWhenStepped ? highPositionY : lowPositionY);
                }
            }
        }

        bool wasMoving = isMoving;
        isMoving = Vector2.Distance(platformRigidbody.position, targetPosition) > 0.01f;


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
        if (platformRigidbody == null) return;

        platformRigidbody.MovePosition(Vector2.MoveTowards(platformRigidbody.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }

    void OnDrawGizmos()
    {
        Vector3 lowPos = new Vector3(transform.position.x, lowPositionY, transform.position.z);
        Vector3 highPos = new Vector3(transform.position.x, highPositionY, transform.position.z);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(lowPos, highPos);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lowPos, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(highPos, 0.2f);
    }
}