using UnityEngine;
using UnityEngine.UI;
using IACGGames;
namespace ForestGame
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
                AudioManager.Instance.PlaySFX(SFXAudioID.Click);
                OnPlayButtonPressed();
            });
            howToPlayButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySFX(SFXAudioID.Click);
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
            GameManager.Instance.StartGame(false);
        }

        public void OnHowToPlayButtonPressed()
        {
            GameSDKSystem.Instance.StartTutorail();
       
        }
    }
}