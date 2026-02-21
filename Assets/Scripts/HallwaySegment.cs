using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a single hallway segment with curves at both ends and spawn points for props.
/// Structure: Entry Curve → Straight Section (with props) → Exit Curve
/// </summary>
public class HallwaySegment : MonoBehaviour
{
    [Header("Segment Configuration")]
    [Tooltip("Total length including both curves")]
    public float segmentLength = 10f;
    [Tooltip("Where this segment connects to the previous (after entry curve)")]
    public Transform segmentStart;
    [Tooltip("Where this segment connects to the next (after exit curve)")]
    public Transform segmentEnd;

    [Header("Spawn Points - Place in Straight Section")]
    [Tooltip("Single door spawn point")]
    public PropSpawnPoint doorSpawn;
    [Tooltip("2 large sign spawn points")]
    public PropSpawnPoint[] largeSignSpawns = new PropSpawnPoint[2];
    [Tooltip("4 small sign spawn points (vertical stack)")]
    public PropSpawnPoint[] smallSignSpawns = new PropSpawnPoint[4];
    [Tooltip("2 plant spawn points")]
    public PropSpawnPoint[] plantSpawns = new PropSpawnPoint[2];

    [Header("Current Configuration")]
    public HallwayConfiguration currentConfig;

    private void OnValidate()
    {
        // Auto-find spawn points if not assigned
        if (doorSpawn == null || largeSignSpawns.Length == 0)
        {
            AutoAssignSpawnPoints();
        }

        // Calculate segment length
        if (segmentStart != null && segmentEnd != null)
        {
            segmentLength = Vector3.Distance(segmentStart.position, segmentEnd.position);
        }
    }

    private void AutoAssignSpawnPoints()
    {
        List<PropSpawnPoint> allSpawns = GetComponentsInChildren<PropSpawnPoint>().ToList();

        doorSpawn = allSpawns.FirstOrDefault(s => s.propType == PropSpawnPoint.PropType.Door);
        largeSignSpawns = allSpawns.Where(s => s.propType == PropSpawnPoint.PropType.LargeSign).ToArray();
        smallSignSpawns = allSpawns.Where(s => s.propType == PropSpawnPoint.PropType.SmallSign).ToArray();
        plantSpawns = allSpawns.Where(s => s.propType == PropSpawnPoint.PropType.Plant).ToArray();
    }

    public void ApplyConfiguration(HallwayConfiguration config)
    {
        currentConfig = config;

        Debug.Log($"[HallwaySegment] Applying configuration:");
        Debug.Log($"  Door: variant {config.doorVariant}");
        Debug.Log($"  Large Signs: [{string.Join(", ", config.largeSignVariants)}]");
        Debug.Log($"  Small Signs: [{string.Join(", ", config.smallSignTexts)}]");
        Debug.Log($"  Plants: [{string.Join(", ", config.plantVariants)}]");

        // Apply door variant
        if (doorSpawn != null && config.doorVariant >= 0 && config.doorVariant < config.doorPrefabs.Length)
        {
            doorSpawn.SpawnProp(config.doorPrefabs[config.doorVariant]);
            doorSpawn.variantIndex = config.doorVariant;
        }

        // Apply large signs
        for (int i = 0; i < largeSignSpawns.Length && i < config.largeSignVariants.Length; i++)
        {
            int variantIndex = config.largeSignVariants[i];
            if (variantIndex >= 0 && variantIndex < config.largeSignPrefabs.Length)
            {
                largeSignSpawns[i].SpawnProp(config.largeSignPrefabs[variantIndex]);
                largeSignSpawns[i].variantIndex = variantIndex;
            }
        }

        // Apply small signs (procedural text)
        for (int i = 0; i < smallSignSpawns.Length && i < config.smallSignTexts.Length; i++)
        {
            if (config.smallSignPrefab != null)
            {
                // Use SpawnProp to properly track and clear old signs
                smallSignSpawns[i].SpawnProp(config.smallSignPrefab);
                
                // Set text on the newly spawned sign
                GameObject signObj = smallSignSpawns[i].GetSpawnedProp();
                if (signObj != null)
                {
                    var textMesh = signObj.GetComponentInChildren<TMPro.TextMeshPro>();
                    if (textMesh != null)
                    {
                        textMesh.text = config.smallSignTexts[i];
                    }
                    else
                    {
                        var legacyText = signObj.GetComponentInChildren<TextMesh>();
                        if (legacyText != null)
                        {
                            legacyText.text = config.smallSignTexts[i];
                        }
                    }
                }
            }
        }

        // Apply plants
        for (int i = 0; i < plantSpawns.Length && i < config.plantVariants.Length; i++)
        {
            int variantIndex = config.plantVariants[i];
            if (variantIndex >= 0 && variantIndex < config.plantPrefabs.Length)
            {
                GameObject plantPrefab = config.plantPrefabs[variantIndex];
                string plantName = plantPrefab != null ? plantPrefab.name : "null";
                Debug.Log($"[HallwaySegment] Spawning plant [{i}]: variant {variantIndex} ({plantName})");
                plantSpawns[i].SpawnProp(plantPrefab);
                plantSpawns[i].variantIndex = variantIndex;
            }
            else
            {
                Debug.LogWarning($"[HallwaySegment] Plant [{i}]: Invalid variant {variantIndex} (valid range: 0-{config.plantPrefabs.Length - 1})");
            }
        }
    }

    public void ClearAllProps()
    {
        doorSpawn?.ClearProp();
        
        foreach (var spawn in largeSignSpawns)
            spawn?.ClearProp();
        
        foreach (var spawn in smallSignSpawns)
            spawn?.ClearProp();
        
        foreach (var spawn in plantSpawns)
            spawn?.ClearProp();
    }

    public Vector3 GetStartPosition() => segmentStart != null ? segmentStart.position : transform.position;
    public Vector3 GetEndPosition() => segmentEnd != null ? segmentEnd.position : transform.position + transform.forward * segmentLength;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 start = GetStartPosition();
        Vector3 end = GetEndPosition();
        
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(start, 0.5f);
        Gizmos.DrawWireSphere(end, 0.5f);
    }
}
