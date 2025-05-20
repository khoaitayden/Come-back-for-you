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

    [Header("Input")]
    [SerializeField] private InputAction Launch;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource launchSoundSource;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        Launch.Enable();
        Launch.performed += LaunchHeadInDirection;
    }

    private void OnDisable()
    {
        Launch.performed -= LaunchHeadInDirection;
        Launch.Disable();
    }

    private void LaunchHeadInDirection(InputAction.CallbackContext ctx)
    {
        Debug.Log("HeadLaunched");
        if (!ReconnectAbility.connectedHead) return;
        connectAbility.DisconnectHead();
        Vector2 launchDirection = -(launchPoint.position - Head.position).normalized;
        LaunchHead(launchDirection);
    }

    public void LaunchHead(Vector2 launchDir)
    {
        Debug.Log("HeadLaunched");
        headRb.AddForce(launchDir.normalized * launchForce, ForceMode2D.Impulse);
        if (launchSoundSource != null)
        {
            launchSoundSource.Play();
        }
    }
}