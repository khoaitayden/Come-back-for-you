using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    [Header("Door Settings")]
    [SerializeField] private GameObject door;

    private SpriteRenderer spriteRenderer;
    private bool hasBeenPressed = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = offSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger && other.CompareTag("Player") && !hasBeenPressed)
        {
            ActivateButton();
        }
    }

    public void ActivateButton()
    {
        hasBeenPressed = true;
        if (spriteRenderer)
        {
            spriteRenderer.sprite = onSprite;
        }
        if (door)
        {
            door.SetActive(false);
        }
    }
}