using DG.Tweening;
using IACGGames;
using UnityEditor;
using UnityEngine;

public class ToyCell : MonoBehaviour
{
    public SpriteRenderer iconRenderer;
    public SpriteRenderer outlineRenderer;
    public ToyItem Toy {  get; private set; }

    [SerializeField] Collider2D col;
    private static readonly Color normalHighlightColor = new Color(0f, 0f, 0f, 0.5f); 
    private static readonly Color activeHighlightColor = new Color(1f, 1f, 1f, 1f);
    Sequence seq;
  //  float originalScale;
    public void SetToy(ToyItem t)
    {
      //  originalScale = transform.localScale.x;
        Toy = t;
        iconRenderer.sprite =  t.sprite;
        outlineRenderer.sprite = t.outlineSprite;
        iconRenderer.gameObject.SetActive(true);
        Highlight(false);
        col.enabled = t != null;
       
    }

    public void Clear()
    {
        seq.Kill();
        Toy = null;
        col.enabled = false;
        iconRenderer.sprite = null;
        iconRenderer.gameObject.SetActive(false);
        outlineRenderer.gameObject.SetActive(false);
    }

    public void Debug(bool isNew)
    {
        Color c = Color.white;
        c.a = 0.2f;
        iconRenderer.color = isNew ? Color.white:c;

    }
    public void PlayCorrect()
    {
       // tick
    }

    public void PlayIncorrect()
    {
        // shake
    }
    public void SetHover(bool state)
    {
        if (GameSDKSystem.Instance.IsPaused) return;
        if (GameManager.Instance.CurrentState != GameState.WaitForPlayer) return;

        if (state)
        {
          //  AudioManager.Instance.PlaySound(AudioID.Hover);
            AudioManager.Instance.PlaySFX(SFXAudioID.Hover);
        }
        Highlight(state);
    }
    public void Highlight(bool state)
    {
        outlineRenderer.gameObject.SetActive(true);
        outlineRenderer.color = state ? activeHighlightColor : normalHighlightColor;
    }
    public void ActiveVisual()
    {
        //shine aniamtion
        //seq = DOTween.Sequence();
        //seq.Append(transform.DOScale(originalScale+0.2f, 0.3f).SetEase(Ease.InBack));
        //seq.Append(transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack));
    }
}
