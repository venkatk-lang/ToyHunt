using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundGenerator", menuName = "ToyHunt/RoundGenerator")]
public class RoundGenerator : ScriptableObject
{

    private System.Random rng = new System.Random();

    public List<ToyItem> BuildRound(int roundNumber, LevelType levelType)
    {
        int requiredCount = levelType.rows*levelType.cols;
        List<ToyType> types = new List<ToyType>(levelType.toyDatabase.variationTypes);
        Helpers.ShuffleList(types);

        List<ToyItem> result = new List<ToyItem>();

        if (roundNumber == 1)
            BuildRoundEasy(types, result, requiredCount);
        else if (roundNumber == 2)
            BuildRoundMedium(types, result, requiredCount);
        else
            BuildRoundHard(types, result, requiredCount);

        Helpers.ShuffleList(result);

        return result.GetRange(0, Mathf.Min(result.Count, requiredCount));
    }
    public List<ToyItem> BuildTutorialRound(LevelType levelType,int requiredCount)
    {
        
        List<ToyType> types = new List<ToyType>(levelType.toyDatabase.variationTypes);
        Helpers.ShuffleList(types);
        List<ToyItem> result = new List<ToyItem>();
        BuildRoundEasy(types, result, requiredCount);
        Helpers.ShuffleList(result);

        return result.GetRange(0, Mathf.Min(result.Count, requiredCount));
    }
    private void BuildRoundEasy(List<ToyType> types, List<ToyItem> result, int needed)
    {
        foreach (var type in types)
        {
            if (result.Count >= needed) break;
            if (type.items.Count == 0) continue;

            result.Add(type.items[rng.Next(type.items.Count)]);
        }
    }

    private void BuildRoundMedium(List<ToyType> types, List<ToyItem> result, int needed)
    {
        foreach (var type in types)
        {
            if (result.Count >= needed) break;
            if (type.items.Count == 0) continue;

            // Always add 1 item
            ToyItem first = type.items[rng.Next(type.items.Count)];
            result.Add(first);

            // 30% chance add second (if available)
            if (type.items.Count > 1 && rng.NextDouble() < 0.3f && result.Count < needed)
            {
                ToyItem second = type.items[rng.Next(type.items.Count)];
                if (second != first)
                    result.Add(second);
            }
        }
    }

    private void BuildRoundHard(List<ToyType> types, List<ToyItem> result, int needed)
    {
        foreach (var type in types)
        {
            List<ToyItem> list = new List<ToyItem>(type.items);
            Helpers.ShuffleList(list);

            foreach (var t in list)
            {
                if (result.Count >= needed) break;
                result.Add(t);
            }
        }
    }
}
