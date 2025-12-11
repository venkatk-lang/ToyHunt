using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : Singleton<TutorialManager>
{
    [Header("UI")]
    [SerializeField] GameObject tutorialUIRoot;
    [SerializeField] TMPro.TextMeshProUGUI tutorialText;
    [SerializeField] Button restartButton;
    [SerializeField] Button skipTutorialButton;

    protected override void Awake()
    {
        base.Awake();
        tutorialUIRoot.SetActive(false);
    }
  
    public void StartTutorial()
    {
        restartButton.gameObject.SetActive(false);
        tutorialUIRoot.SetActive(true);
        SetTutorialText(string.Empty);
        GameManager.Instance.OnRoundStart += OnRoundStart;
        GameManager.Instance.OnCorrectClicked += OnCorrectClicked;
        GameManager.Instance.OnWrongClicked += OnWrongClicked;
        GameManager.Instance.OnRoundEnd += RoundEnd;
        skipTutorialButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SFXAudioID.Click);
            SkipTutorial();
            GameManager.Instance.StartGame(false);
        });
        restartButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SFXAudioID.Click);
            SkipTutorial();
            StartTutorial();
        });

        GameManager.Instance.StartGame(true);
    }
    public void OnRoundStart()
    {
        SetTutorialText("Click any item and remember your choice.");
    }
    public void OnWrongClicked()
    {
        SetTutorialText("You chose an item you already clicked earlier.");
        restartButton.gameObject.SetActive(true);
    }
    public void OnCorrectClicked()
    {
        SetTutorialText("Click any item you HAVEN'T clicked yet.");
    }
    private void SetTutorialText(string text)
    {
        tutorialText.text = text;
    }
    private void RoundEnd()
    {
        SetTutorialText(string.Empty);
        GameManager.Instance.OnRoundStart -= OnRoundStart;
        GameManager.Instance.OnCorrectClicked -= OnCorrectClicked;
        GameManager.Instance.OnWrongClicked -= OnWrongClicked;
        GameManager.Instance.OnRoundEnd -= RoundEnd;
    }
    private void TutorialComplete()
    {
        skipTutorialButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        tutorialUIRoot.SetActive(false);
    }

    public void SkipTutorial()
    {
        RoundEnd();
        TutorialComplete();
     
    }
}
