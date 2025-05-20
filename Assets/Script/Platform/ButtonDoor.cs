using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    [Header("Door Settings")]
    [SerializeField] private GameObject door;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource buttonClickSoundSource; 
    [SerializeField] private AudioSource doorOpenSoundSource;

    private SpriteRenderer spriteRenderer;
    private bool hasBeenPressed = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            spriteRenderer.sprite = offSprite;
        }
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
        if (buttonClickSoundSource != null)
        {
            buttonClickSoundSource.Play();
        }
        if (doorOpenSoundSource != null)
        {
            doorOpenSoundSource.Play();
        }
    }
}