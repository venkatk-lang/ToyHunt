using DG.Tweening;
using UnityEngine;

public class FeedbackPopup : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Show(bool correct)
    {
        spriteRenderer.sprite = correct?sprites[0]: sprites[1];
        transform.localScale = Vector3.zero;
        transform.DOScale(1f,0.5f).SetEase(Ease.OutBack);
        
    }
}
