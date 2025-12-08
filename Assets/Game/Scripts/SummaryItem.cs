using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class SummaryItem : MonoBehaviour
{
    [SerializeField] GameObject highlighter;
    [SerializeField] Image itemIcon;

    public void Setup(Sprite iconSprite)
    {
        itemIcon.sprite = iconSprite;
        itemIcon.DOFade(0,0);
   
        Highlight(false);
    }
    public void FadeTween()
    {
         itemIcon.DOFade(1f, 0.5f);
    }
    public void Show()
    {
        itemIcon.color = Color.white;
    }
    public void Highlight(bool highlight)
    {
        highlighter.SetActive(highlight);
    }
}
