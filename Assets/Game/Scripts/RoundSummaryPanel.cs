using DG.Tweening;
using IACGGames;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundSummaryPanel : MonoBehaviour
{
    [SerializeField] Transform gridParent;
    [SerializeField] Button nextButton;
    [SerializeField] SummaryItem summaryItemPrefab;
    bool lastSummary;
    float delayEachCell = 0.5f;
    List<SummaryItem> wrongItems = new List<SummaryItem>();
    [SerializeField] TextMeshProUGUI itemCountText;
    [SerializeField] TextMeshProUGUI bonusScoreText;

    public void Init(List<ToyItem> items, int wrongItemID, bool _lastSummary)
    {
        Debug.Log("Wrong id " + wrongItemID);
        wrongItems.Clear();
        Helpers.DestroyChildren(gridParent);
        int count = items.Count;
        for (int i = 0; i < count; i++) 
        {
            SummaryItem item = Instantiate(summaryItemPrefab, gridParent);
            item.Setup(items[i].sprite, i * delayEachCell);
            if (items[i].id == wrongItemID)
            {
                wrongItems.Add(item);
                Debug.Log(wrongItems.Count);
            }
 
            Sequence cellSeq = DOTween.Sequence();
            cellSeq.Join(item.FadeTween());
        }
        DOVirtual.DelayedCall(count * delayEachCell, () =>
        {
            foreach (SummaryItem item in wrongItems)
            {
                item.Highlight(true);
            }
            UIManager.Instance.gameHUD.ShowBonusScore(GameManager.Instance.GetRoundBonus());
        });
        
        lastSummary = _lastSummary;

        itemCountText.text = GameManager.Instance.TotalCorrectItemCount.ToString();
        bonusScoreText.text = $" {GameManager.Instance.TotalCorrectItemCount} X {GameManager.Instance.CurrentRound*100}";
        

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    private void OnNextButtonClicked()
    {
        if (!lastSummary)
        {
            GameManager.Instance.StartNextRound();
        }
        else
        {
            GameManager.Instance.EndGame();
        }

    }
}
