using UnityEngine;

// Ensure the GameObject has a TrailRenderer component
[RequireComponent(typeof(TrailRenderer))]
public class TrailLine : MonoBehaviour
{
    // Reference to the TrailRenderer component
    public TrailRenderer tr;

    void Awake()
    {
        // Cache the TrailRenderer component
        tr = GetComponent<TrailRenderer>();
    }

    /// Starts rendering the trail.
    /// Clears any previous trail before starting a new one.
    public void RenderTrail()
    {
        tr.Clear();      // Remove old trail points
        tr.emitting = true;  // Enable trail rendering
    }

    // Stops rendering the trail.
    public void EndTrail()
    {
        tr.emitting = false; // Disable trail rendering
    }
}
