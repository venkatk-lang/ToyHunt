using UnityEngine;
using UnityEngine.UI;
using IACGGames;
namespace TrainGame
{
    public class MainMenu : UIPanelBase
    {
        
        [SerializeField] Button playButton;
        [SerializeField] Button howToPlayButton;

      
        protected override void OnEnable()
        {
            base.OnEnable();
            playButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySound("Click");
                OnPlayButtonPressed();
            });
            howToPlayButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySound("Click");
                OnHowToPlayButtonPressed();
            });
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            playButton.onClick.RemoveAllListeners();
            howToPlayButton.onClick.RemoveAllListeners();
        }
       
       
        public void OnPlayButtonPressed()
        {
           
        }

        public void OnHowToPlayButtonPressed()
        {
           // TutorialManager.Instance.StartTutorial();
        }
    }
}