using UnityEngine;
using UnityEngine.UI;
namespace IACGGames
{

    public class PauseMenu : MonoBehaviour
    {
        [Header("Buttons")]
        public Button resumeButton;
        public Button restartButton;
        public Button quitButton;

        [Header("Audio Toggles")]
        public Toggle musicToggle;
        public Toggle sfxToggle;

        private void Start()
        {
            // Hook up button events
            if (resumeButton) resumeButton.onClick.AddListener(OnResume);
            if (restartButton) restartButton.onClick.AddListener(OnRestart);
            if (quitButton) quitButton.onClick.AddListener(OnQuit);

            // Hook toggles
            if (musicToggle) musicToggle.onValueChanged.AddListener(OnMusicToggleChange);
            if (sfxToggle) sfxToggle.onValueChanged.AddListener(OnSfxToggleChange);

            // Sync UI state from AudioManager settings
            //if (musicToggle && AudioSystem.Instance)
            //    musicToggle.isOn = !AudioSystem.Instance.GetMusicMuted();

            //if (sfxToggle && AudioSystem.Instance)
            //    sfxToggle.isOn = !AudioSystem.Instance.GetSfxMuted();


        }

        public void OnResume()
        {
            GameSDKSystem.Instance.ResumeGame();
        }

        public void OnRestart()
        {
            GameSDKSystem.Instance.RestartGame();
        }

        public void OnQuit()
        {
            GameSDKSystem.Instance.QuitGame();
        }

        public void OnMusicToggleChange(bool isOn)
        {
            // AudioSystem.Instance.ToggleMusic(isOn);
        }

        public void OnSfxToggleChange(bool isOn)
        {
            // AudioSystem.Instance.ToggleSFX(isOn);
        }
    }
}