using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundGenerator", menuName = "ToyHunt/RoundGenerator")]
public class RoundGenerator : ScriptableObject
{

    private System.Random rng = new System.Random();

    public List<ToyItem> BuildRound(int roundNumber, LevelType levelType)
    {
        int required = levelType.rows * levelType.cols;

        // Clone and shuffle types
        List<ToyType> types = new List<ToyType>(levelType.toyDatabase.variationTypes);
        Helpers.ShuffleList(types);

        List<ToyItem> result = new List<ToyItem>();

        switch (roundNumber)
        {
            case 1:
                BuildEasy(types, result, required);
                break;

            case 2:
                BuildWithDuplicateChance(types, result, required, 0.30f);
                break;

            case 3:
            default:
                BuildWithDuplicateChance(types, result, required, 0.50f);
                break;
        }

        Helpers.ShuffleList(result);
        return result.Take(required).ToList();
    }

    // --------------------------------------
    // EASY ROUND — FULLY RANDOM
    // --------------------------------------
    private void BuildEasy(List<ToyType> types, List<ToyItem> result, int needed)
    {
        List<ToyItem> all = new List<ToyItem>();

        foreach (var t in types)
        {
            if (t.items != null)
                all.AddRange(t.items);
        }

        Helpers.ShuffleList(all);

        for (int i = 0; i < needed && i < all.Count; i++)
            result.Add(all[i]);
    }

    // -------------------------------------
    // MEDIUM / HARD - DUPLICATE CHANCE
    // -------------------------------------
    private void BuildWithDuplicateChance(
        List<ToyType> types,
        List<ToyItem> result,
        int needed,
        float duplicateChance)
    {
        // First pass: select 1 item per type + duplicate chance
        foreach (var type in types)
        {
            if (result.Count >= needed)
                break;

            if (type.items == null || type.items.Count == 0)
                continue;

            // Always pick one
            ToyItem first = type.items[rng.Next(type.items.Count)];
            result.Add(first);

            if (result.Count >= needed)
                break;

            // Duplicate chance: pick another item from SAME TYPE
            if (type.items.Count > 1 && rng.NextDouble() < duplicateChance)
            {
                ToyItem second;

                do
                {
                    second = type.items[rng.Next(type.items.Count)];
                }
                while (second == first);

                result.Add(second);

                if (result.Count >= needed)
                    break;
            }
        }

        // ----------------------------------------------------
        // SECOND PASS: If still not enough items, fill randomly
        // from the entire database
        // ----------------------------------------------------
        if (result.Count < needed)
        {
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
