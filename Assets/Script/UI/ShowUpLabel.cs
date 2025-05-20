using UnityEngine;

public class ShowLabelOnProximity : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject label;
    [SerializeField] private float showDistance;

    private void Update()
    {
        if (player == null || label == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= showDistance)
        {
            label.SetActive(true);
        }
        else
        {
            label.SetActive(false);
        }
    }
}