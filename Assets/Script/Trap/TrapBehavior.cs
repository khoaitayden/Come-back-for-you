using UnityEngine;

public class TrapBehavior : MonoBehaviour
{
    [SerializeField] private Transform teleport; // Target transform to teleport the player to

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            TeleportPlayer(collision.collider.gameObject);
        }
    }

    void TeleportPlayer(GameObject player)
    {
        if (teleport != null)
        {
            player.transform.position = teleport.position;
        }
    }
}