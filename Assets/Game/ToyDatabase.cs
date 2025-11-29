using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToyDatabase", menuName = "ToyHunt/ToyDatabase")]
public class ToyDatabase : ScriptableObject
{
    public List<ToyItem> allToys = new List<ToyItem>();

    // Runtime dictionary for fast access
    private Dictionary<ToyVariationType, List<ToyItem>> toyGroups;

    public void BuildDictionary()
    {
        toyGroups = new Dictionary<ToyVariationType, List<ToyItem>>();

        foreach (ToyVariationType type in System.Enum.GetValues(typeof(ToyVariationType)))
        {
            toyGroups[type] = new List<ToyItem>();
        }

        foreach (var toy in allToys)
        {
            toyGroups[toy.variationType].Add(toy);
        }
    }

    public List<ToyItem> GetGroup(ToyVariationType type)
    {
        if (toyGroups == null) BuildDictionary();
        return toyGroups[type];
    }

    public Dictionary<ToyVariationType, List<ToyItem>> GetDictionary()
    {
        if (toyGroups == null) BuildDictionary();
        return toyGroups;
    }
}
