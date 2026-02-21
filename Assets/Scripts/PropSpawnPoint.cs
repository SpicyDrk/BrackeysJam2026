using UnityEngine;

/// <summary>
/// Marks a location where a prop can spawn in a hallway segment.
/// </summary>
public class PropSpawnPoint : MonoBehaviour
{
    public enum PropType
    {
        Door,
        LargeSign,
        SmallSign,
        Plant
    }

    [Header("Spawn Point Configuration")]
    public PropType propType;
    public int variantIndex = 0; // Which variant is currently spawned (0 = none/default)

    [Header("Runtime")]
    private GameObject spawnedProp;

    public void SpawnProp(GameObject propPrefab)
    {
        ClearProp();

        if (propPrefab != null)
        {
            spawnedProp = Instantiate(propPrefab, transform.position, transform.rotation, transform);
        }
    }

    public void ClearProp()
    {
        if (spawnedProp != null)
        {
            DestroyImmediate(spawnedProp);
            spawnedProp = null;
        }
    }

    public GameObject GetSpawnedProp() => spawnedProp;

    private void OnDrawGizmos()
    {
        Color gizmoColor = propType switch
        {
            PropType.Door => Color.blue,
            PropType.LargeSign => Color.yellow,
            PropType.SmallSign => Color.green,
            PropType.Plant => Color.magenta,
            _ => Color.white
        };

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, 
            $"{propType}\nVariant: {variantIndex}");
#endif
    }
}
