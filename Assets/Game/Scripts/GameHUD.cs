using DG.Tweening;
using IACGGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TrainGame
{
    public class GameHUD : UIPanelBase
    {

        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameObject topBarGO;
        [SerializeField] private TextMeshProUGUI trainProgressText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI timeText;
        [Header("Countdown UI")]
        public GameObject countdownPanel;
        public TextMeshProUGUI countdownText;
        public Button testResetBtn;
    

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
        
          //  UpdateHUD(LevelManager.Instance.IsTutorailMode);

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
            //GameManager.Instance.LevelManager.OnCountdownTick -= HandleCountdownTick;
            //GameManager.Instance.LevelManager.OnTimerTick -= HandleTimerTick;
        
       
        }


        public void UpdateScore(int totalScore, int correctTrains, int totalTrains)
        {
            scoreText.text = $"Score: {totalScore}";
            trainProgressText.text = $"Correct {correctTrains}/{totalTrains}";

        }
        public void UpdateHUD(bool tutorial)
        {
            topBarGO.gameObject.SetActive(!tutorial);
           
        }

        
    }
}
