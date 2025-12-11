using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToyType", menuName = "ToyHunt/ToyType")]
public class ToyType : ScriptableObject
{
    public string displayName;
    [Header("All ToyItems belonging to this group")]
    public List<ToyItem> items = new List<ToyItem>();
}