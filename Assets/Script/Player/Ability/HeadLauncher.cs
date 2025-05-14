using UnityEngine;
using UnityEngine.InputSystem;

public class HeadLauncher : MonoBehaviour
{
    public static bool headConnected = false;

    [Header("References")]
    [SerializeField] private Transform Head;
    [SerializeField] private Rigidbody2D headRb;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private ReconnectAbility connectAbility;

    [Header("Launch Settings")]
    [SerializeField] private float launchForce;
    private void Awake()
    {

    }
    private void OnEnable()
        {
            if (PlayerInputHandler.Instance != null)
            PlayerInputHandler.Instance.OnLaunchPressed += LaunchHeadInDirection;
        }

    private void OnDisable()
        {
            if (PlayerInputHandler.Instance != null)
            PlayerInputHandler.Instance.OnLaunchPressed -= LaunchHeadInDirection;
        }
    private void LaunchHeadInDirection()
    {
        if (!ReconnectAbility.connectedHead) return;
        connectAbility.DisconnectHead();
        Vector2 launchDirection = -(launchPoint.position - Head.position).normalized;
        LaunchHead(launchDirection);
    }

    public void LaunchHead(Vector2 launchDir)
    {
        Debug.Log("HeadLaunched");
        headRb.AddForce(launchDir.normalized * launchForce, ForceMode2D.Impulse);
    }
}