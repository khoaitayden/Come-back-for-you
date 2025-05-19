using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float checkRadius = 0.5f; // Radius to check for player contact
    [SerializeField] private InputAction teleport; // Input action for teleport/door activation (e.g., 'T' key)

    void OnEnable()
    {
        teleport.Enable();
    }

    void OnDisable()
    {
        teleport.Disable();
    }

    void Update()
    {
        if (teleport.WasPressedThisFrame() && IsPlayerTouching())
        {
            Debug.Log("Door triggered by Player with 'T' key! Loading EndScene...");
            SceneManager.LoadScene("EndScene"); // Load the EndScene
        }
    }

    bool IsPlayerTouching()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius);
        foreach (Collider2D hit in hits)
        {
            if (!hit.isTrigger && hit.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, checkRadius); // Show detection radius
    }
}