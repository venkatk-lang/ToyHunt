using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundGenerator", menuName = "ToyHunt/RoundGenerator")]
public class RoundGenerator : ScriptableObject
{
    public ToyDatabase toyDatabase;

    System.Random rng = new System.Random();
    private int requiredCount = 35;

    public List<ToyItem> BuildRound(int roundNumber,int _requiredCount)
    {
        requiredCount = _requiredCount;
        var groups = toyDatabase.GetDictionary();
        List<ToyItem> result = new List<ToyItem>();

        if (roundNumber == 1)
            BuildRoundEasy(groups, result);
        else if (roundNumber == 2)
            BuildRoundMedium(groups, result);
        else
            BuildRoundHard(groups, result);

      
        Helpers.ShuffleList(result);
        return result.Take(requiredCount).ToList();
    }

    private void BuildRoundEasy(Dictionary<ToyVariationType, List<ToyItem>> groups, List<ToyItem> result)
    {
        var keys = groups.Keys.ToList();
        Helpers.ShuffleList(keys);

        foreach (var type in keys)
        {
            if (result.Count >= requiredCount) break;

            var list = groups[type];
            if (list.Count == 0) continue;

         
            result.Add(list[rng.Next(list.Count)]);
        }


        if (result.Count < requiredCount)
        {
            var all = toyDatabase.allToys.ToList();
            Helpers.ShuffleList(all);

            foreach (var t in all)
            {
                if (result.Count >= requiredCount) break;
                if (!result.Contains(t))
                    result.Add(t);
            }
        }
    }


    private void BuildRoundMedium(Dictionary<ToyVariationType, List<ToyItem>> groups, List<ToyItem> result)
    {
        var keys = groups.Keys.ToList();
        Helpers.ShuffleList(keys);

        foreach (var type in keys)
        {
            if (result.Count >= requiredCount) break;

            var list = groups[type];
            if (list.Count == 0) continue;

   
            var first = list[rng.Next(list.Count)];
            result.Add(first);
            if (list.Count > 1 && rng.NextDouble() < 0.35 && result.Count < requiredCount)
            {
                var variations = list.Where(t => t != first).ToList();
                if (variations.Count > 0)
                {
                    result.Add(variations[rng.Next(variations.Count)]);
                }
            }
        }
        FillToMax(result);
    }

    private void BuildRoundHard(Dictionary<ToyVariationType, List<ToyItem>> groups, List<ToyItem> result)
    {
        var keys = groups.Keys.ToList();
        Helpers.ShuffleList(keys);

        foreach (var type in keys)
        {
            if (result.Count >= requiredCount) break;

            var list = groups[type];
            if (list.Count == 0) continue;

            Helpers.ShuffleList(list);

            int clusterSize = Mathf.Clamp(list.Count, 1, rng.Next(2, 5));

            for (int i = 0; i < clusterSize; i++)
            {
                if (i >= list.Count) break;
                if (result.Count >= requiredCount) break;

                result.Add(list[i]);
            }
        }

        FillToMax(result);
    }

    private void FillToMax(List<ToyItem> result)
    {
        if (result.Count >= requiredCount)
            return;

        var all = toyDatabase.allToys.ToList();
        Helpers.ShuffleList(all);

        foreach (var t in all)
        {
            if (result.Count >= requiredCount) break;
            if (!result.Contains(t))
                result.Add(t);
        }
    }
}
