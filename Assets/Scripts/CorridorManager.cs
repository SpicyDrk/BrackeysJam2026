using UnityEngine;
using System.Collections.Generic;

public class CorridorManager : MonoBehaviour
{
    public static CorridorManager Instance { get; private set; }

    [Header("Corridor Setup")]
    [SerializeField] private CorridorSegment segmentPrefab;
    [SerializeField] private int activeSegmentCount = 5; // How many segments exist at once
    [SerializeField] private Transform player;

    [Header("Anomaly Configuration")]
    [SerializeField] private float anomalyChance = 0.5f; // 50% chance
    [SerializeField] private bool debugMode = true;

    private Queue<CorridorSegment> activeSegments = new Queue<CorridorSegment>();
    private CorridorSegment currentSegment; // The segment the player is currently in
    private Vector3 nextSegmentPosition;
    private Quaternion nextSegmentRotation;
    private int totalSegmentsSpawned = 0;

    // State tracking
    private bool currentLoopHasAnomaly = false;
    private int loopCount = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Auto-find player if not assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    private void Start()
    {
        InitializeCorridors();
    }

    private void InitializeCorridors()
    {
        if (segmentPrefab == null)
        {
            Debug.LogError("CorridorManager: No segment prefab assigned!");
            return;
        }

        // Start position and rotation
        nextSegmentPosition = transform.position;
        nextSegmentRotation = transform.rotation;

        // Spawn initial segments
        for (int i = 0; i < activeSegmentCount; i++)
        {
            SpawnSegment();
        }

        // Set first segment as current
        if (activeSegments.Count > 0)
        {
            currentSegment = activeSegments.Peek();
        }

        Log("Corridor system initialized with " + activeSegmentCount + " segments");
    }

    private void SpawnSegment()
    {
        CorridorSegment segment = Instantiate(segmentPrefab, nextSegmentPosition, nextSegmentRotation, transform);
        segment.SetSegmentIndex(totalSegmentsSpawned);
        segment.name = $"CorridorSegment_{totalSegmentsSpawned}";
        
        activeSegments.Enqueue(segment);
        totalSegmentsSpawned++;

        // Calculate next spawn position (at the end of this segment)
        nextSegmentPosition = segment.GetEndPosition();
        nextSegmentRotation = segment.transform.rotation;

        Log($"Spawned segment {segment.GetSegmentIndex()} at {nextSegmentPosition}");
    }

    public void OnPlayerEnteredSegment(CorridorSegment enteredSegment)
    {
        // Ignore if this is the same segment we're already in
        if (enteredSegment == currentSegment)
            return;

        Log($"Player entered segment {enteredSegment.GetSegmentIndex()}");

        // Update current segment
        currentSegment = enteredSegment;

        // Recycle the oldest segment (the one behind the player)
        RecycleSegment();
    }

    private void RecycleSegment()
    {
        if (activeSegments.Count == 0) return;

        // Remove the segment from the back of the queue
        CorridorSegment oldSegment = activeSegments.Dequeue();
        
        Log($"Recycling segment {oldSegment.GetSegmentIndex()}");

        // Clear any anomaly from the old segment
        oldSegment.ClearAnomaly();

        // Prepare new loop state
        PrepareNextLoop();

        // Reposition segment to the front
        oldSegment.transform.position = nextSegmentPosition;
        oldSegment.transform.rotation = nextSegmentRotation;
        oldSegment.SetSegmentIndex(totalSegmentsSpawned);
        oldSegment.name = $"CorridorSegment_{totalSegmentsSpawned}";

        // Apply anomaly to new segment if needed
        oldSegment.SetAnomaly(currentLoopHasAnomaly);

        // Add back to queue
        activeSegments.Enqueue(oldSegment);
        totalSegmentsSpawned++;

        // Update next spawn position
        nextSegmentPosition = oldSegment.GetEndPosition();
        nextSegmentRotation = oldSegment.transform.rotation;

        Log($"Segment repositioned to {nextSegmentPosition}. Has anomaly: {currentLoopHasAnomaly}");
    }

    private void PrepareNextLoop()
    {
        loopCount++;

        // Determine if next loop has anomaly
        float roll = Random.value;
        currentLoopHasAnomaly = roll < anomalyChance;

        Log($"Loop {loopCount}: Anomaly = {currentLoopHasAnomaly} (rolled {roll:F2})");
    }

    public bool CurrentLoopHasAnomaly() => currentLoopHasAnomaly;

    public int GetLoopCount() => loopCount;

    private void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log($"[CorridorManager] {message}");
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize next spawn position
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(nextSegmentPosition, 0.5f);
        Gizmos.DrawLine(nextSegmentPosition, nextSegmentPosition + Vector3.up * 2f);
    }
}
