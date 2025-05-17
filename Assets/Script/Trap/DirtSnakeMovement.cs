using UnityEngine;
using System.Collections.Generic;

public class SnakeMovement : MonoBehaviour
{
    [SerializeField] private List<GameObject> destinations = new List<GameObject>(); // List of destination GameObjects
    [SerializeField] private float moveSpeed = 5f; // Speed of the snake's head
    [SerializeField] private float reachThreshold = 0.5f; // Distance to consider destination reached
    [SerializeField] private Rigidbody2D headRigidbody; // Reference to the head's Rigidbody2D
    [SerializeField] private bool loopDestinations = false; // Toggle for looping back to first destination

    private int currentDestinationIndex = 0; // Tracks current target destination
    private Vector2 currentDestinationPosition;

    void Start()
    {
        if (headRigidbody == null)
        {
            headRigidbody = GetComponent<Rigidbody2D>();
            if (headRigidbody == null)
            {
                Debug.LogError("Head Rigidbody2D not assigned and not found on GameObject!");
            }
        }

        // Set initial destination if available
        if (destinations.Count > 0 && destinations[0] != null)
        {
            currentDestinationPosition = destinations[0].transform.position;
        }
    }

    void FixedUpdate()
    {
        if (destinations.Count == 0 || currentDestinationIndex >= destinations.Count)
        {
            // No destinations or all reached (and not looping), stop movement
            headRigidbody.linearVelocity = Vector2.zero;
            return;
        }

        // Check if current destination is null
        if (destinations[currentDestinationIndex] == null)
        {
            Debug.LogWarning("Destination at index " + currentDestinationIndex + " is null!");
            MoveToNextDestination();
            return;
        }

        // Calculate direction to current destination
        currentDestinationPosition = destinations[currentDestinationIndex].transform.position;
        Vector2 direction = (currentDestinationPosition - headRigidbody.position).normalized;

        // Move the head
        headRigidbody.linearVelocity = direction * moveSpeed;

        // Check if close enough to destination
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
                // Loop back to the first destination
                currentDestinationIndex = 0;
                currentDestinationPosition = destinations[0].transform.position;
            }
            else
            {
                // Stop movement if not looping
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
            Debug.LogWarning("Next destination is null, stopping or looping.");
            headRigidbody.linearVelocity = Vector2.zero;
        }
    }

    // Optional: Method to add a new destination at runtime
    public void AddDestination(GameObject newDestination)
    {
        if (newDestination != null)
        {
            destinations.Add(newDestination);
            // If this is the first destination, set it as the current target
            if (destinations.Count == 1)
            {
                currentDestinationIndex = 0;
                currentDestinationPosition = newDestination.transform.position;
            }
        }
        else
        {
            Debug.LogWarning("Attempted to add a null destination!");
        }
    }

    // Optional: Method to toggle looping at runtime
    public void SetLoopDestinations(bool shouldLoop)
    {
        loopDestinations = shouldLoop;
    }
}