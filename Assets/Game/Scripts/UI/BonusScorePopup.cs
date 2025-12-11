using DG.Tweening;
using TMPro;
using UnityEngine;

public class BonusScorePopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bonusScoreText;
    RectTransform rt;
    float yStratPos = -150;
    float yEndPos = -80;
    private void Awake()
    {
        rt = GetComponent<RectTransform>();

    }
    public void Show(int bonus)
    {
        ResetDefault();
        bonusScoreText.text = $"+{bonus}";
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        rt.DOAnchorPosY(yEndPos, 1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
   
    private void ResetDefault()
    {
        transform.localScale = Vector3.one * 0.3f;
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, yStratPos);
        rt.DOKill();
    }
}
