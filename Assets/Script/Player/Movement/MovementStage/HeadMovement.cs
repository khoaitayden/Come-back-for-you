using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HeadMovement : MonoBehaviour
{
    [SerializeField] private float moveForce;
    [SerializeField] private Rigidbody2D headRb;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float horizontalInput = PlayerInputHandler.Instance.MoveInput.x;

        headRb.AddForce(new Vector2(horizontalInput * moveForce, 0f), ForceMode2D.Force);
    }
}