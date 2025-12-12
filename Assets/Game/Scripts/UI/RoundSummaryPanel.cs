using DG.Tweening;
using IACGGames;
using System.Collections;
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
    List<SummaryItem> wrongItems = new List<SummaryItem>();
    [SerializeField] TextMeshProUGUI itemCountText;
    [SerializeField] TextMeshProUGUI bonusScoreText;
    Coroutine panelAnimationC;
    List<SummaryItem> summaryItems = new List<SummaryItem>();
    public void Init(List<ToyItem> items, int wrongItemID, bool _lastSummary)
    {

        Debug.Log("Wrong id " + wrongItemID);
        wrongItems.Clear();
        summaryItems.Clear();
        Helpers.DestroyChildren(gridParent);
        int count = items.Count;
        for (int i = 0; i < count; i++) 
        {
            SummaryItem item = Instantiate(summaryItemPrefab, gridParent);
            item.Setup(items[i].sprite);
            if (items[i].id == wrongItemID)
            {
                wrongItems.Add(item);
            }
            summaryItems.Add(item); 
        }
        lastSummary = _lastSummary;

        itemCountText.text = GameManager.Instance.TotalCorrectItemCount.ToString();
        bonusScoreText.text = $" {GameManager.Instance.TotalCorrectItemCount} X {GameManager.Instance.CurrentRound*100}";

        nextButton.interactable = true;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(OnNextButtonClicked);

        if(panelAnimationC !=null)StopCoroutine(panelAnimationC);
        panelAnimationC = StartCoroutine(AnimatePanel());
    }

    IEnumerator AnimatePanel()
    {
        Debug.Log("Animate start");
        int count = summaryItems.Count;
        for (int i = 0; i < count; i++)
        {
            Debug.Log("wait");
            yield return new WaitForSeconds(0.2f);
            Debug.Log("fade");
            AudioManager.Instance.PlaySFX(SFXAudioID.SummaryItem,0.8f,1.2f);
            summaryItems[i].FadeTween();
        }
        yield return new WaitForSeconds(0.5f);
        foreach (SummaryItem item in wrongItems)
        {
            AudioManager.Instance.PlaySFX(SFXAudioID.WrongSummaryItem);
            item.Highlight(true);
        }
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySFX(SFXAudioID.Bonus);
        UIManager.Instance.gameHUD.ShowBonusScore(GameManager.Instance.GetRoundBonus());
        GameManager.Instance.UpdateScore();
        GameManager.Instance.CompleteRound();
        panelAnimationC = null;
    }
    void ShowDirect()
    {
        int count = summaryItems.Count;
        for (int i = 0; i < count; i++)
        {
            summaryItems[i].Show();
        }
        foreach (SummaryItem item in wrongItems)
        {
            item.Highlight(true);
        }
        UIManager.Instance.gameHUD.ShowBonusScore(GameManager.Instance.GetRoundBonus());
        GameManager.Instance.CompleteRound();
    } 
    private void OnNextButtonClicked()
    {
        nextButton.interactable = false;
        if (panelAnimationC != null)
        {
            StopCoroutine(panelAnimationC);
            Debug.Log("Co " + panelAnimationC);
            ShowDirect();
        }

        DOVirtual.DelayedCall(1.1f, () =>
        {
            if (!lastSummary)
            {
                GameManager.Instance.StartNextRound();
            }
            else
            {
                GameManager.Instance.EndGame();
            }
        });
    }
    private void OnDestroy()
    {
        if (panelAnimationC != null) StopCoroutine(panelAnimationC);
    }
}
