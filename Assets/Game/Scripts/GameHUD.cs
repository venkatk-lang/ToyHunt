using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace ForestGame
{
    public class GameHUD : UIPanelBase
    {

        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private GameObject topBarGO;
        [SerializeField] private TextMeshProUGUI correctText;
  
        public Button testResetBtn;

        public RoundSummaryPanel roundSummaryPanel;
        public RoundStartPanel roundStartPanel;
        public BonusScorePopup bonusScorePopup;
        protected override void OnEnable()
        {
            base.OnEnable();

        }

        protected override void OnDisable()
        {
            base.OnDisable();

        }

        public override void Show(float animTime = 0)
        {
            base.Show(animTime);
        
           // UpdateHUD(LevelManager.Instance.IsTutorailMode);
            UpdateHUD(false);
            CloseRoundSummary();
            CloseRoundStart();
            bonusScorePopup.gameObject.SetActive(false);
            ////Testing
            //if (LevelManager.Instance.IsTutorailMode)
            //{
            //    testResetBtn.gameObject.SetActive(false);
            //}
            //else
            //{
            //    testResetBtn.gameObject.SetActive(true);
            //    testResetBtn.onClick.RemoveAllListeners();
            //    testResetBtn.onClick.AddListener(() =>
            //    {
            //        LevelManager.Instance.ResetLevelState();
            //        AudioManager.Instance.PlaySound("Click");
            //        UIManager.Instance.Show(UIState.LevelSelection, 0.25f);

            //    });
            //}


        }
        public override void Hide(float animTime = 0)
        {
            base.Hide(animTime);
        }


        public void UpdateScore(int totalScore, int correctCount)
        {
            scoreText.text = totalScore.ToString();
            correctText.text = correctCount.ToString();
        }
        public void UpdateRound(int currentRound,int totalRound)
        {
            roundText.text = $"FOREST {currentRound} of {totalRound}";
            
        }
        public void UpdateHUD(bool tutorial)
        {
            topBarGO.gameObject.SetActive(!tutorial);
           
        }
        public void ShowRoundSummary(List<ToyItem> selectedItems,int wrongItemId,bool lastSummary)
        {
            roundSummaryPanel.gameObject.SetActive(true);
            roundSummaryPanel.Init(selectedItems, wrongItemId,lastSummary);

        }
        public void ShowBonusScore(int bonusScore)
        {
            bonusScorePopup.gameObject.SetActive(true);
            bonusScorePopup.Show(bonusScore);
        }
        public void CloseRoundSummary()
        {
            roundSummaryPanel.gameObject.SetActive(false);
        }
        public void ShowRoundStart(int round,int maxRound)
        {
            roundStartPanel.gameObject.SetActive(true);
            roundStartPanel.Show(round, maxRound);

        }

        public void CloseRoundStart()
        {
            roundStartPanel.gameObject.SetActive(false);
        }

        public void ShowFinalSummary(int totalScore)
        {
            // Show final screen
        }
    }
}
