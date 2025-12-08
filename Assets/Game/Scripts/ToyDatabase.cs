using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToyDatabase", menuName = "ToyHunt/ToyDatabase")]
public class ToyDatabase : ScriptableObject
{
    public List<ToyType> variationTypes = new List<ToyType>();
   
}
