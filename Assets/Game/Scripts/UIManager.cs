using System.Collections.Generic;
using UnityEngine;
using TrainGame;
namespace IACGGames
{
    public enum UIState
    {
        MainMenu,
        GameHUD,
        LevelComplete
    }
    public class UIManager : Singleton<UIManager>
    {
        [Header("Panels (assign inspector)")]
        public GameHUD gameHUD;
        public MainMenu mainMenu;
        public LevelCompletePanel levelCompletePanel;
        private Dictionary<UIState, UIPanelBase> panels;
        private UIState currentState;
      //  [SerializeField] TutorialSkipButton tutorialSkipButton;
        protected override void Awake()
        {
            base.Awake();

            panels = new Dictionary<UIState, UIPanelBase>()
        {
            { UIState.MainMenu, mainMenu },
            { UIState.GameHUD, gameHUD },
            { UIState.LevelComplete, levelCompletePanel }
        };

            foreach (var item in panels)
            {
                item.Value.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            //tutorialSkipButton.Init();

        }
        public void Show(UIState state,float animTime)
        {
          
            // Hide currently active
            if (panels.ContainsKey(currentState))
                panels[currentState].Hide();

            // Show new
           // Debug.Log("Current UI " + state);
            panels[state].Show(animTime);
            currentState = state;
        }
        public void Init()
        {
      
            Show(UIState.MainMenu, 0); //show level selection first
        }
        public TMPro.TMP_Text scoreText;
        public TMPro.TMP_Text roundText;
        public GameObject roundSummaryPanel;
        public Transform summaryGridParent; // where to display collected toys
        public GameObject summaryCellPrefab; // simple prefab showing toy + highlight

        public void UpdateScore(int score)
        {
            if (scoreText != null) scoreText.text = score.ToString();
        }

        public void SetRoundNumber(int round)
        {
            if (roundText != null) roundText.text = $"Round {round}";
        }

        public void ShowRoundSummary(List<ToyItem> roundItems, HashSet<string> selectedIds, ToyItem duplicate)
        {
            // show the summary panel and populate summary grid in selection order if you store that order
            roundSummaryPanel.SetActive(true);
            // TODO: fill in summary cells. Implementation depends on how you store selection order.
        }

        public void ClearRoundSummary()
        {
            roundSummaryPanel.SetActive(false);
        }

        public void ShowFinalSummary(int totalScore)
        {
            // Show final screen
        }
    }
}

