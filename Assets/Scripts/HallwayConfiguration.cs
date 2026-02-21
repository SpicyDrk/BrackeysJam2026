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
    public string[] smallSignTexts = new string[4]; // Text for 4 signs

    [Header("Plant Configuration")]
    public GameObject[] plantPrefabs = new GameObject[4]; // 4 states: empty + 3 plants
    public int[] plantVariants = new int[2]; // Which plant state for each of 2 spawn points (0-3, 0 = empty)

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
        clone.plantVariants = (int[])plantVariants.Clone();
        
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

        // Check plants
        if (plantVariants.Length != other.plantVariants.Length) return false;
        for (int i = 0; i < plantVariants.Length; i++)
        {
            if (plantVariants[i] != other.plantVariants[i]) return false;
        }

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

        Debug.Log($"[HallwayConfig] Creating variation - changeType: {changeType}");

        switch (changeType)
        {
            case 0: // Change door
                int oldDoor = variation.doorVariant;
                variation.doorVariant = GetDifferentVariant(variation.doorVariant, variation.doorPrefabs.Length);
                Debug.Log($"[HallwayConfig] Changed DOOR: {oldDoor} -> {variation.doorVariant}");
                break;

            case 1: // Change a large sign
                int signIndex = Random.Range(0, variation.largeSignVariants.Length);
                int oldSign = variation.largeSignVariants[signIndex];
                variation.largeSignVariants[signIndex] = GetDifferentVariant(oldSign, variation.largeSignPrefabs.Length);
                Debug.Log($"[HallwayConfig] Changed LARGE SIGN [{signIndex}]: {oldSign} -> {variation.largeSignVariants[signIndex]}");
                break;

            case 2: // Change a small sign text
                int textIndex = Random.Range(0, variation.smallSignTexts.Length);
                string oldText = variation.smallSignTexts[textIndex];
                variation.smallSignTexts[textIndex] = GenerateDifferentSignText(oldText);
                Debug.Log($"[HallwayConfig] Changed SMALL SIGN [{textIndex}]: '{oldText}' -> '{variation.smallSignTexts[textIndex]}'");
                break;

            case 3: // Change a plant
                int plantIndex = Random.Range(0, variation.plantVariants.Length);
                int oldPlant = variation.plantVariants[plantIndex];
                variation.plantVariants[plantIndex] = GetDifferentVariant(oldPlant, variation.plantPrefabs.Length);
                Debug.Log($"[HallwayConfig] Changed PLANT [{plantIndex}]: {oldPlant} -> {variation.plantVariants[plantIndex]} (max: {variation.plantPrefabs.Length - 1})");
                break;
        }

        return variation;
    }

    /// <summary>
    /// Gets a random variant index that's different from the current one
    /// </summary>
    private static int GetDifferentVariant(int currentVariant, int totalVariants)
    {
        if (totalVariants <= 1)
        {
            Debug.LogWarning($"[HallwayConfig] Cannot get different variant - only {totalVariants} available!");
            return currentVariant;
        }

        // Use modulo trick to guarantee a different value
        // For example: if current=1 and total=4, we pick from [0,1,2] then add 1 and mod 4 to get [1,2,3,0]
        int offset = Random.Range(1, totalVariants);
        return (currentVariant + offset) % totalVariants;
    }

    /// <summary>
    /// Generates a random sign text that's different from the current one
    /// </summary>
    private static string GenerateDifferentSignText(string currentText)
    {
        string[] words = { "Entrance", "Parks Dept. >", "", "Dog Storage", "RESTRICTED", "Otter Zone", "B Batteries", "DiceySporks >" };
        
        if (words.Length <= 1)
        {
            return words[0];
        }

        // Keep generating until we get a different text
        string newText;
        int attempts = 0;
        do
        {
            newText = words[Random.Range(0, words.Length)];
            attempts++;
        } while (newText == currentText && attempts < 20);

        return newText;
    }
}
