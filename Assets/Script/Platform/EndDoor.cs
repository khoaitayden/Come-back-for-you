using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float checkRadius;
    [SerializeField] private InputAction teleport;

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
            SceneManager.LoadScene("EndScene");
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

}