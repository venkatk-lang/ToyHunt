using TMPro;
using UnityEngine;

public class RoundStartPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI descriptionText;

    public void Show(int round,int maxRound,bool isTut)
    {
        if (isTut)
        {
            roundText.text = $"<sketchy>Tutorial</sketchy>";
            descriptionText.text = "";
        }
        else
        {
            roundText.text = $"<sketchy>FOREST {round} of {maxRound}</sketchy>";
            descriptionText.text = "";

        }
    }
}
