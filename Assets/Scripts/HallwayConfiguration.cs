using UnityEngine;

/// <summary>
/// Defines how a hallway segment should be configured with props.
/// Can be created as a ScriptableObject or used as a data class.
/// </summary>
[System.Serializable]
public class HallwayConfiguration
{
    [Header("Door Configuration")]
    public GameObject[] doorPrefabs = new GameObject[2]; // 2 door variants
    public int doorVariant = 0; // Which door to spawn (0-1)

    [Header("Large Sign Configuration")]
    public GameObject[] largeSignPrefabs; // Multiple sign variants
    public int[] largeSignVariants = new int[2]; // Which variant for each of 2 spawn points

    [Header("Small Sign Configuration")]
    public GameObject smallSignPrefab; // Base small sign prefab
    public string[] smallSignTexts = new string[3]; // Text for 3 signs

    [Header("Plant Configuration")]
    public GameObject[] plantPrefabs = new GameObject[4]; // 4 states: empty + 3 plants
    public int plantVariant = 0; // Which plant state (0-3, 0 = empty)

    /// <summary>
    /// Creates a copy of this configuration
    /// </summary>
    public HallwayConfiguration Clone()
    {
        HallwayConfiguration clone = new HallwayConfiguration();
        
        clone.doorPrefabs = (GameObject[])doorPrefabs.Clone();
        clone.doorVariant = doorVariant;
        
        clone.largeSignPrefabs = (GameObject[])largeSignPrefabs.Clone();
        clone.largeSignVariants = (int[])largeSignVariants.Clone();
        
        clone.smallSignPrefab = smallSignPrefab;
        clone.smallSignTexts = (string[])smallSignTexts.Clone();
        
        clone.plantPrefabs = (GameObject[])plantPrefabs.Clone();
        clone.plantVariant = plantVariant;
        
        return clone;
    }

    /// <summary>
    /// Compares this configuration to another to see if they're identical
    /// </summary>
    public bool IsIdenticalTo(HallwayConfiguration other)
    {
        if (other == null) return false;

        // Check door
        if (doorVariant != other.doorVariant) return false;

        // Check large signs
        if (largeSignVariants.Length != other.largeSignVariants.Length) return false;
        for (int i = 0; i < largeSignVariants.Length; i++)
        {
            if (largeSignVariants[i] != other.largeSignVariants[i]) return false;
        }

        // Check small sign texts
        if (smallSignTexts.Length != other.smallSignTexts.Length) return false;
        for (int i = 0; i < smallSignTexts.Length; i++)
        {
            if (smallSignTexts[i] != other.smallSignTexts[i]) return false;
        }

        // Check plant
        if (plantVariant != other.plantVariant) return false;

        return true;
    }

    /// <summary>
    /// Generates a random variation that's different from the reference
    /// </summary>
    public static HallwayConfiguration CreateRandomVariation(HallwayConfiguration reference)
    {
        HallwayConfiguration variation = reference.Clone();

        // Randomly pick ONE thing to change
        int changeType = Random.Range(0, 4);

        switch (changeType)
        {
            case 0: // Change door
                variation.doorVariant = (variation.doorVariant + 1) % variation.doorPrefabs.Length;
                break;

            case 1: // Change a large sign
                int signIndex = Random.Range(0, variation.largeSignVariants.Length);
                variation.largeSignVariants[signIndex] = Random.Range(0, variation.largeSignPrefabs.Length);
                break;

            case 2: // Change a small sign text
                int textIndex = Random.Range(0, variation.smallSignTexts.Length);
                variation.smallSignTexts[textIndex] = GenerateRandomSignText();
                break;

            case 3: // Change plant
                variation.plantVariant = Random.Range(0, variation.plantPrefabs.Length);
                break;
        }

        return variation;
    }

    private static string GenerateRandomSignText()
    {
        string[] words = { "EXIT", "ENTRANCE", "AUTHORIZED", "PERSONNEL", "ONLY", "AREA", "RESTRICTED", "ZONE", "A", "B", "C" };
        return words[Random.Range(0, words.Length)];
    }
}
