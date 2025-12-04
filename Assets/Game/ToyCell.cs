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
    public void SetToy(ToyItem t)
    {
        // change postiion of this object randomlit above its original position
        //while moving the coontainer, move from that postion to original position.

        Toy = t;

        iconRenderer.sprite = t != null ? t.sprite : null;
        iconRenderer.gameObject.SetActive(t != null);
        outlineRenderer.gameObject.SetActive(false);
        col.enabled = t != null;
       
    }

    public void Clear()
    {
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
        Highlight(state);
    }
    public void Highlight(bool state)
    {
        outlineRenderer.gameObject.SetActive(true);
        outlineRenderer.color = state ? activeHighlightColor : normalHighlightColor;
    }
}
