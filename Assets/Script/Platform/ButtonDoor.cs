using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Sprite offSprite; // Sprite before button is pressed
    [SerializeField] private Sprite onSprite; // Sprite after button is pressed

    [Header("Door Settings")]
    [SerializeField] private GameObject door; // Reference to the door GameObject

    private SpriteRenderer spriteRenderer;
    private bool hasBeenPressed = false;

    void Awake()
    {
        // Initialize button components (self)
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            Debug.LogError("SpriteRenderer not found on button GameObject!");
        }

        // Validate door reference
        if (!door)
        {
            Debug.LogError("Door GameObject not assigned!");
        }

        // Set initial sprite
        spriteRenderer.sprite = offSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Trigger door disappearance only when a "Player" tagged non-trigger Collider2D enters
        if (!other.isTrigger && other.CompareTag("Player") && !hasBeenPressed)
        {
            ActivateButton();
            Debug.Log("Button pressed by Player, door disappears!");
        }
    }

    public void ActivateButton()
    {
        hasBeenPressed = true;
        if (spriteRenderer)
        {
            spriteRenderer.sprite = onSprite;
        }

        // Make the door disappear
        if (door)
        {
            door.SetActive(false);
        }
    }
}