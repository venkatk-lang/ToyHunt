using UnityEngine;

[CreateAssetMenu(fileName = "ToyItem", menuName = "ToyHunt/ToyItem")]
public class ToyItem : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite sprite;
    public ToyVariationType variationType = ToyVariationType.None;
    [TextArea] public string description;
}
