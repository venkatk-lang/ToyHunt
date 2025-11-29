using UnityEngine;
[CreateAssetMenu(menuName = "TrainGame/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Tooltip("Train Speed")]
    [SerializeField]private float trainSpeed = 2f;
    public float TrainSpeed => trainSpeed; 
    [Header("Score Settings")]
    [SerializeField] private int singleTrainScore = 100;
    public int SingleTrainScore => singleTrainScore; 

    [Header("Game Time")]
    [SerializeField] private int levelTimeInSec = 120;
    public int LevelTimeInSec => levelTimeInSec;
}
