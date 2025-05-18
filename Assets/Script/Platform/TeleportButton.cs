using UnityEngine;
using System.Collections.Generic;

public class TeleportButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Sprite offSprite; // Sprite when button is off or on cooldown
    [SerializeField] private Sprite onSprite; // Sprite when button is clicked
    [SerializeField] private bool startOnCooldown = false; // Toggle to start on cooldown

    [Header("Teleport Settings")]
    [SerializeField] private List<GameObject> teleportTargets = new List<GameObject>(); // List of objects to teleport
    [SerializeField] private Transform teleportLocation; // Destination location
    [SerializeField] private float cooldownTime = 2f; // Cooldown duration in seconds

    private SpriteRenderer spriteRenderer;
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            Debug.LogError("SpriteRenderer not found on button GameObject!");
        }

        isOnCooldown = startOnCooldown;
        spriteRenderer.sprite = isOnCooldown ? offSprite : offSprite; // Start with off sprite
    }

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownTime)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
                spriteRenderer.sprite = offSprite; // Reset to off sprite after cooldown
                Debug.Log("Button cooldown finished!");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger && other.CompareTag("Player") && !isOnCooldown)
        {
            TeleportObjects();
            Debug.Log("Button triggered by Player!");
        }
    }

    void TeleportObjects()
    {
        if (teleportTargets.Count == 0 || !teleportLocation)
        {
            Debug.LogWarning("No teleport targets or location assigned!");
            return;
        }

        bool teleportedAny = false;
        for (int i = 0; i < teleportTargets.Count; i++)
        {
            GameObject target = teleportTargets[i];
            if (target)
            {
                target.transform.position = teleportLocation.position;
                teleportedAny = true;
                Debug.Log($"Teleported {target.name} to {teleportLocation.position}!");
            }
        }

        if (teleportedAny)
        {
            spriteRenderer.sprite = onSprite; // Switch to on sprite on click
            isOnCooldown = true;
            cooldownTimer = 0f;
        }
        else
        {
            Debug.LogWarning("No valid targets to teleport!");
        }
    }

    // Visualize the teleport location in the Scene view
    void OnDrawGizmos()
    {
        if (teleportLocation)
        {
            // Draw a yellow sphere at the teleport location
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(teleportLocation.position, 0.3f);

            // Display a label with the position
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(teleportLocation.position, $"Teleport Location: {teleportLocation.position:F2}");
            #endif
        }
    }
}