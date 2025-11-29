using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundGenerator", menuName = "ToyHunt/RoundGenerator")]
public class RoundGenerator : ScriptableObject
{
    public ToyDatabase toyDatabase;

    System.Random rng = new System.Random();

    public List<ToyItem> BuildRound(int roundNumber)
    {
        // Make sure dictionary exists
        var groups = toyDatabase.GetDictionary();

        List<ToyItem> result = new List<ToyItem>();

        if (roundNumber == 1)
            BuildRoundEasy(groups, result);
        else if (roundNumber == 2)
            BuildRoundMedium(groups, result);
        else
            BuildRoundHard(groups, result);

        return result.Take(35).ToList();
    }

    // ------------------ ROUND 1 ------------------
    private void BuildRoundEasy(Dictionary<ToyVariationType, List<ToyItem>> groups, List<ToyItem> result)
    {
        // Take 1 unique toy from as many groups as possible
        var keys = groups.Keys.ToList();
        Shuffle(keys);

        foreach (var type in keys)
        {
            if (result.Count >= 35) break;
            var list = groups[type];
            if (list.Count == 0) continue;
            result.Add(list[rng.Next(list.Count)]);
        }
    }

    // ------------------ ROUND 2 ------------------
    private void BuildRoundMedium(Dictionary<ToyVariationType, List<ToyItem>> groups, List<ToyItem> result)
    {
        var keys = groups.Keys.ToList();
        Shuffle(keys);

        foreach (var type in keys)
        {
            if (result.Count >= 35) break;

            var list = groups[type];
            if (list.Count == 0) continue;

            // Always add one
            var first = list[rng.Next(list.Count)];
            result.Add(first);

            // 30% chance add a second variant (pair)
            if (list.Count > 1 && rng.NextDouble() < 0.30 && result.Count < 35)
            {
                var second = list.FirstOrDefault(t => t != first);
                if (second != null)
                    result.Add(second);
            }
        }
    }

    // ------------------ ROUND 3 ------------------
    private void BuildRoundHard(Dictionary<ToyVariationType, List<ToyItem>> groups, List<ToyItem> result)
    {
        var keys = groups.Keys.ToList();
        Shuffle(keys);

        foreach (var type in keys)
        {
            if (result.Count >= 35) break;
            var list = groups[type];
            Shuffle(list);

            foreach (var toy in list)
            {
                if (result.Count >= 35) break;
                result.Add(toy);
            }
        }
    }

    // ------------------ SHUFFLE ------------------
    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int k = rng.Next(i + 1);
            (list[i], list[k]) = (list[k], list[i]);
        }
    }
}
