using TMPro;
using UnityEngine;

public class RoundStartPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI descriptionText;

    public void Show(int round,int maxRound)
    {
        roundText.text = $"<sketchy>FOREST {round} of {maxRound}</sketchy>";
    }
}
