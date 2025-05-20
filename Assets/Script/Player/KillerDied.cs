using UnityEngine;

public class KillerDied : MonoBehaviour
{
    [SerializeField] private Rigidbody2D killerBody;
    [SerializeField] private Rigidbody2D killerHead;
    [SerializeField] private Rigidbody2D killerRightArm;
    [SerializeField] private Rigidbody2D killerLeftArm;
    [SerializeField] private Rigidbody2D killerRightLeg;
    [SerializeField] private Rigidbody2D killerLeftLeg;
    [SerializeField] private float force;
    [SerializeField] private SpringJoint2D head;
    [SerializeField] private SpringJoint2D rightArm;
    [SerializeField] private SpringJoint2D leftArm;
    [SerializeField] private SpringJoint2D rightLeg;
    [SerializeField] private SpringJoint2D leftLeg;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            DisconnectJoints();
            ShootBodyParts();
        }
    }
    void DisconnectJoints()
    {
        head.enabled = false;
        rightArm.enabled = false;
        leftArm.enabled = false;
        rightLeg.enabled = false;
        leftLeg.enabled = false;
    }
    void ShootBodyParts()
    {
        killerBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        killerHead.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        killerRightArm.AddForce(Vector2.right * force, ForceMode2D.Impulse);
        killerLeftArm.AddForce(Vector2.left * force, ForceMode2D.Impulse);
        killerRightLeg.AddForce(Vector2.right * force, ForceMode2D.Impulse);
        killerLeftLeg.AddForce(Vector2.left * force, ForceMode2D.Impulse);
    }
}
