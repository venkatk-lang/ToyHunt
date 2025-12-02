using UnityEngine;

public class ToyCell : MonoBehaviour
{
    public SpriteRenderer iconRenderer;
    public ToyItem Toy {  get; private set; }

    [SerializeField] Collider2D col;

    public void SetToy(ToyItem t)
    {
        Toy = t;

        iconRenderer.sprite = t != null ? t.sprite : null;
        iconRenderer.enabled = t != null;
        col.enabled = true;
    }

    public void Clear()
    {
        Toy = null;
        iconRenderer.enabled = false;
        col.enabled = false;
    }

    public void Show()
    {
        gameObject.SetActive(true);

    }
    public void Debug(bool isNew)
    {
        Color c = Color.white;
        c.a = 0.2f;
        iconRenderer.color = isNew ? Color.white:c;

    }
    public void PlayCorrect()
    {
       

    }

    public void PlayIncorrect()
    {
        
  
    }
}
