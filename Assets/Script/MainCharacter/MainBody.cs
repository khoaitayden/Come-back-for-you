using UnityEngine;
using UnityEngine.InputSystem;

public class MainBody : MonoBehaviour
{
    public static bool headConnected = false;

    [Header("References")]
    [SerializeField] private Transform Head;
    [SerializeField] private Transform Body;
    [SerializeField] private Head head;
    [SerializeField] private Rigidbody2D headRb;
    [SerializeField] private SpringJoint2D springJoint;

    [Header("Connection Settings")]
    public float snapDistance = 1.2f;
    public float reconnectDelay = 1f;
    private bool canReconnect = true;

    [Header("Input")]
    [SerializeField] private InputAction launchAction;

    private void Awake()
    {
        InitializeSpringJoint();
    }

    private void OnEnable()
    {
        launchAction.Enable();
        launchAction.performed += LaunchHead;
    }

    private void OnDisable()
    {
        launchAction.Disable();
        launchAction.performed -= LaunchHead;
    }

    private void FixedUpdate()
    {
        TryConnectHead();
    }

    private void InitializeSpringJoint()
    {
        if (springJoint != null)
        {
            springJoint.enabled = false;
            springJoint.connectedBody = null;
        }
    }
    private void TryConnectHead()
    {
        if (!headConnected && Vector2.Distance(Head.position, Body.position) < snapDistance && canReconnect)
        {
            ConnectHead();
        }
    }
    private void ConnectHead()
    {
        Debug.Log("Head connected");
        headConnected = true;
        Head.SetParent(transform);
        AttachSpringJoint();
    }

    private void AttachSpringJoint()
    {
        if (springJoint != null && headRb != null)
        {
            springJoint.connectedBody = headRb;
            springJoint.enabled = true;
        }
    }

    public void LaunchHead(InputAction.CallbackContext ctx)
    {
        if (!headConnected) return;

        DisconnectHead();
        LaunchHeadInDirection();
        StartReconnectCooldown();
    }

    private void DisconnectHead()
    {
        headConnected = false;
        if (springJoint != null)
        {
            springJoint.connectedBody = null;
            springJoint.enabled = false;
        }
    }

    private void LaunchHeadInDirection()
    {
        float direction = Body.localScale.x > 0 ? 1 : -1;
        head.LaunchHead(new Vector2(direction, 1f));
    }

    private void StartReconnectCooldown()
    {
        canReconnect = false;
        StartCoroutine(ReconnectCooldown());
    }

    private System.Collections.IEnumerator ReconnectCooldown()
    {
        yield return new WaitForSeconds(reconnectDelay);
        canReconnect = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            head.OnGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            head.OnGround = false;
        }
    }
}