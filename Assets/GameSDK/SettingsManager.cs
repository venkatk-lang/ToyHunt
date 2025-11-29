using UnityEngine;
using UnityEngine.UI;
namespace IACGGames
{
    public class SettingsManager : Singleton<SettingsManager>
    {
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private Button pauseButton;
        private void Start()
        {
            ShowPauseMenu(false);
        }
        private void OnEnable()
        {
            pauseButton.onClick.AddListener(() =>
            {
                GameSDKSystem.Instance.PauseGame();
            });

        }
        private void OnDisable()
        {
            pauseButton.onClick.RemoveAllListeners();
        }
        public void ShowPauseMenu(bool show)
        {
            if (pauseMenu != null)
                pauseMenu.gameObject.SetActive(show);
        }
    }
}