using UnityEngine;

public class AutoMovingPlatformX : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private float moveDistance;
    [SerializeField] private float moveSpeed; 
    [SerializeField] private bool moveRightWhenStepped = true; 
    [SerializeField] private float moveDelay; 

    private Rigidbody2D platformRigidbody;
    private Vector2 targetPosition;
    private bool playerOnPlatform = false;
    private float delayTimer = 0f;
    private bool isDelayed = false;
    private float leftPositionX;
    private float rightPositionX;

    void Awake()
    {
        platformRigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        leftPositionX = transform.position.x;
        rightPositionX = leftPositionX + moveDistance;
        targetPosition = new Vector2(leftPositionX, transform.position.y);
        transform.position = targetPosition; 
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
            targetPosition = new Vector2(moveRightWhenStepped ? leftPositionX : rightPositionX, transform.position.y);
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

    void OnDrawGizmos()
    {
        float gizmoLeftX = transform.position.x;
        float gizmoRightX = gizmoLeftX + moveDistance;
        Vector3 leftPos = new Vector3(gizmoLeftX, transform.position.y, transform.position.z);
        Vector3 rightPos = new Vector3(gizmoRightX, transform.position.y, transform.position.z);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(leftPos, rightPos);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(leftPos, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rightPos, 0.2f);
    }
}