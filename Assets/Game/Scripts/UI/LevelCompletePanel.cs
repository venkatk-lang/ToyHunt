using IACGGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace ForestGame
{

    public class LevelCompletePanel : UIPanelBase
    {

        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI trainProgressText;
        [SerializeField] private Button nextButton;


        public override void Show(float animTime = 0)
        {
            base.Show(animTime);
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() =>
            {
                
                AudioManager.Instance.PlaySFX(SFXAudioID.Click);
               // UIManager.Instance.Show(UIState.LevelSelection, 0.25f);

            });
        }
        public override void Hide(float animTime = 0)
        {
            base.Hide(animTime);
            nextButton.onClick.RemoveAllListeners();
        }

        public void Init(int totalScore, int correctTrains, int totalTrains)
        {
            scoreText.text = $"Score: {totalScore}";
            trainProgressText.text = $"Correct {correctTrains}/{totalTrains}";

        }

    }
}