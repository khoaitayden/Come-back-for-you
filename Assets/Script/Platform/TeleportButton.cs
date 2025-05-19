using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class TeleportButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite; 
    [SerializeField] private bool startOnCooldown = false; 

    [Header("Teleport Settings")]
    [SerializeField] private List<GameObject> teleportTargets = new List<GameObject>(); 
    [SerializeField] private Transform teleportLocation; 
    [SerializeField] private float cooldownTime; 
    [SerializeField] private float checkRadius;
    [SerializeField] private InputAction teleport;

    private SpriteRenderer spriteRenderer;
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    void OnEnable()
    {
        teleport.Enable();
    }

    void OnDisable()
    {
        teleport.Disable();
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isOnCooldown = startOnCooldown;
        spriteRenderer.sprite = offSprite;
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
                spriteRenderer.sprite = offSprite; 
            }
        }
        else if (teleport.WasPressedThisFrame() && IsPlayerOnButton())
        {
            TeleportObjects();
        }
    }

    bool IsPlayerOnButton()
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

    void TeleportObjects()
    {
        if (teleportTargets.Count == 0 || !teleportLocation)
        {
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
            }
        }

        if (teleportedAny)
        {
            spriteRenderer.sprite = onSprite; 
            isOnCooldown = true;
            cooldownTimer = 0f;
        }
    }

    void OnDrawGizmos()
    {
        if (teleportLocation)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(teleportLocation.position, 0.3f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius); 
    }
}