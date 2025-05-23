using UnityEngine;
using System.Collections.Generic;

public class SnakeMovement : MonoBehaviour
{
    [SerializeField] private List<GameObject> destinations = new List<GameObject>();
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float reachThreshold = 0.5f;
    [SerializeField] private Rigidbody2D headRigidbody;
    [SerializeField] private bool loopDestinations = false;

    private int currentDestinationIndex = 0;
    private Vector2 currentDestinationPosition;

    void Start()
    {
        if (headRigidbody == null)
        {
            headRigidbody = GetComponent<Rigidbody2D>();
        }
        if (destinations.Count > 0 && destinations[0] != null)
        {
            currentDestinationPosition = destinations[0].transform.position;
        }
    }

    void FixedUpdate()
    {
        if (destinations.Count == 0 || currentDestinationIndex >= destinations.Count)
        {
            headRigidbody.linearVelocity = Vector2.zero;
            return;
        }

        if (destinations[currentDestinationIndex] == null)
        {
            MoveToNextDestination();
            return;
        }

        currentDestinationPosition = destinations[currentDestinationIndex].transform.position;
        Vector2 direction = (currentDestinationPosition - headRigidbody.position).normalized;
        headRigidbody.linearVelocity = direction * moveSpeed;

        float distance = Vector2.Distance(headRigidbody.position, currentDestinationPosition);
        if (distance < reachThreshold)
        {
            MoveToNextDestination();
        }
    }

    private void MoveToNextDestination()
    {
        currentDestinationIndex++;
        if (currentDestinationIndex >= destinations.Count)
        {
            if (loopDestinations && destinations.Count > 0)
            {
                currentDestinationIndex = 0;
                currentDestinationPosition = destinations[0].transform.position;
            }
            else
            {
                headRigidbody.linearVelocity = Vector2.zero;
                return;
            }
        }

        if (destinations[currentDestinationIndex] != null)
        {
            currentDestinationPosition = destinations[currentDestinationIndex].transform.position;
        }
        else
        {
            headRigidbody.linearVelocity = Vector2.zero;
        }
    }

    public void AddDestination(GameObject newDestination)
    {
        if (newDestination != null)
        {
            destinations.Add(newDestination);
            if (destinations.Count == 1)
            {
                currentDestinationIndex = 0;
                currentDestinationPosition = newDestination.transform.position;
            }
        }
    }

    public void SetLoopDestinations(bool shouldLoop)
    {
        loopDestinations = shouldLoop;
    }
}