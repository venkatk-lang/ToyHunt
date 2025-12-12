using UnityEngine;

public class NormalScoreWrapper : MonoBehaviour
{
    public NormalScoreUI scoreUI;

    private NormalScore normalScore;

    public void Initialize(int startingScore = 0)
    {
        // Create the core score system
        normalScore = new NormalScore(startingScore);

        // Hook UI to score system
        if (scoreUI != null)
            scoreUI.Initialize(normalScore);
    }

    // Optional: expose the score to gameplay
    public NormalScore Score => normalScore;


}
