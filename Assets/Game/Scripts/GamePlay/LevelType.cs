using UnityEngine;

[CreateAssetMenu(fileName = "LevelType", menuName = "ToyHunt/LevelType")]
public class LevelType : ScriptableObject
{
    [Header("Grid Settings")]
    public int rows = 5;
    public int cols = 7;

    [Header("Reference Database")]
    public ToyDatabase toyDatabase;

    [HideInInspector] public int totalGridSlots;
    [HideInInspector] public int totalAvailableItems;

    // Called from Editor script
    public void RecalculateCounts()
    {
        totalGridSlots = rows * cols;

        totalAvailableItems = 0;
        if (toyDatabase != null)
        {
            foreach (var type in toyDatabase.variationTypes)
                totalAvailableItems += type.items.Count;
        }
    }

    public bool IsValid()
    {
        // Must have enough unique items for grid
        return totalAvailableItems >= totalGridSlots;
    }
}