using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundGenerator", menuName = "ToyHunt/RoundGenerator")]
public class RoundGenerator : ScriptableObject
{

    private System.Random rng = new System.Random();

    public List<ToyItem> BuildRound(int roundNumber, LevelType levelType)
    {
        int needed = levelType.rows * levelType.cols;

        // Copy & shuffle type list
        List<ToyType> types = new List<ToyType>(levelType.toyDatabase.variationTypes);
        Helpers.ShuffleList(types);

        List<ToyItem> result = new List<ToyItem>();

        switch (roundNumber)
        {
            case 1:
                BuildEasy(types, result, needed);
                break;

            case 2:
                BuildMedium(types, result, needed);
                break;

            case 3:
            default:
                BuildHard(types, result, needed);
                break;
        }

        // Final shuffle so duplicates mix in the grid
        Helpers.ShuffleList(result);

        return result.Take(needed).ToList();
    }

    // ----------------------------------------------------------
    // EASY — 10% chance for a second item from same type
    // ----------------------------------------------------------
    private void BuildEasy(List<ToyType> types, List<ToyItem> result, int needed)
    {
        foreach (var type in types)
        {
            if (result.Count >= needed) break;
            if (type.items == null || type.items.Count == 0) continue;

            // First pick
            ToyItem first = type.items[rng.Next(type.items.Count)];
            result.Add(first);
            if (result.Count >= needed) break;

            // 10% chance for second
            if (type.items.Count > 1 && rng.NextDouble() < 0.10)
            {
                ToyItem second = GetRandomDifferent(type.items, first);
                result.Add(second);
            }
        }

        FillIfNeeded(types, result, needed);
    }

    // ----------------------------------------------------------
    // MEDIUM — 70% chance for second item from same type
    // ----------------------------------------------------------
    private void BuildMedium(List<ToyType> types, List<ToyItem> result, int needed)
    {
        foreach (var type in types)
        {
            if (result.Count >= needed) break;
            if (type.items == null || type.items.Count == 0) continue;

            // First item
            ToyItem first = type.items[rng.Next(type.items.Count)];
            result.Add(first);
            if (result.Count >= needed) break;

            // 70% chance for second
            if (type.items.Count > 1 && rng.NextDouble() < 0.70)
            {
                ToyItem second = GetRandomDifferent(type.items, first);
                result.Add(second);
            }
        }

        FillIfNeeded(types, result, needed);
    }

    // ----------------------------------------------------------
    // HARD — Always 2 items (if possible), 70% chance for 3rd
    // ----------------------------------------------------------
    private void BuildHard(List<ToyType> types, List<ToyItem> result, int needed)
    {
        foreach (var type in types)
        {
            if (result.Count >= needed) break;
            if (type.items == null || type.items.Count == 0) continue;

            // First item (always)
            ToyItem first = type.items[rng.Next(type.items.Count)];
            result.Add(first);
            if (result.Count >= needed) break;

            // Always choose second if possible
            if (type.items.Count > 1)
            {
                ToyItem second = GetRandomDifferent(type.items, first);
                result.Add(second);
                if (result.Count >= needed) break;

                // 70% chance for third (if exists)
                if (type.items.Count > 2 && rng.NextDouble() < 0.70)
                {
                    ToyItem third = GetRandomDifferent(type.items, first, second);
                    result.Add(third);
                }
            }
        }

        FillIfNeeded(types, result, needed);
    }

    // ----------------------------------------------------------
    // HELPERS
    // ----------------------------------------------------------
    private ToyItem GetRandomDifferent(List<ToyItem> items, params ToyItem[] exclude)
    {
        ToyItem pick;
        do
        {
            pick = items[rng.Next(items.Count)];
        }
        while (exclude.Contains(pick));
        return pick;
    }

    private void FillIfNeeded(List<ToyType> types, List<ToyItem> result, int needed)
    {
        if (result.Count >= needed) return;

        // Flatten full DB
        List<ToyItem> all = new List<ToyItem>();
        foreach (var t in types)
            if (t.items != null)
                all.AddRange(t.items);

        Helpers.ShuffleList(all);

        int index = 0;
        while (result.Count < needed && index < all.Count)
        {
            result.Add(all[index]);
            index++;
        }
    }
    public List<ToyItem> BuildTutorialRound(LevelType levelType, int requiredCount)
    {

        List<ToyType> types = new List<ToyType>(levelType.toyDatabase.variationTypes);
        Helpers.ShuffleList(types);
        List<ToyItem> result = new List<ToyItem>();
        BuildEasy(types, result, requiredCount);
        Helpers.ShuffleList(result);

        return result.GetRange(0, Mathf.Min(result.Count, requiredCount));
    }
}
