using UnityEngine;

public class ToyCell : MonoBehaviour
{
    public SpriteRenderer iconRenderer;
    public ToyItem Toy { get; private set; }
    private int index;

    public void SetIndex(int idx) => index = idx;

    public void SetToy(ToyItem t)
    {
        Toy = t;

        iconRenderer.sprite = t != null ? t.sprite : null;
        iconRenderer.enabled = t != null;

    }

    public void Clear()
    {
        Toy = null;
        iconRenderer.enabled = false;

    }

    public void Show()
    {
        gameObject.SetActive(true);

    }

    public void PlayCorrect()
    {
       

    }

    public void PlayIncorrect()
    {
        
  
    }
}
