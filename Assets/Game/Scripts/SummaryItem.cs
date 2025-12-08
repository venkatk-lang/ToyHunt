using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class SummaryItem : MonoBehaviour
{
    [SerializeField] GameObject highlighter;
    [SerializeField] Image itemIcon;
    float vDelay;
    public void Setup(Sprite iconSprite,float visualDelay)
    {
        itemIcon.sprite = iconSprite;
        itemIcon.DOFade(0,0);
        vDelay = visualDelay;
        Highlight(false);
    }
    public Tween FadeTween()
    {
        return itemIcon.DOFade(1f, 0.5f).SetDelay(vDelay);
    }
    public void Highlight(bool highlight)
    {
        highlighter.SetActive(highlight);
    }
}
