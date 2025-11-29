using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToyCell : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public GameObject correctMark;
    public GameObject incorrectMark;
    public Animator animator; // optional animator on the cell

    private ToyItem toy;
    private int index;
    private GameManager gameManager;

    private void Awake()
    {
        // Find GameManager in scene (cache)
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetIndex(int idx) => index = idx;

    public void SetToy(ToyItem t)
    {
        toy = t;
        if (icon != null)
        {
            icon.sprite = t != null ? t.sprite : null;
            icon.enabled = t != null;
        }

        correctMark?.SetActive(false);
        incorrectMark?.SetActive(false);
    }

    public void Clear()
    {
        toy = null;
        if (icon != null) { icon.sprite = null; icon.enabled = false; }
        correctMark?.SetActive(false);
        incorrectMark?.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        animator?.Play("Spawn", -1, 0f); // optional
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (toy == null) return;
        gameManager.OnToySelected(toy, this);
    }

    public void PlayCorrect()
    {
        correctMark?.SetActive(true);
        animator?.SetTrigger("Correct");
        // play a small animation then hide the cell or play move to shelf animation
    }

    public void PlayIncorrect()
    {
        incorrectMark?.SetActive(true);
        animator?.SetTrigger("Incorrect");
        // optional shake animation
    }
}
