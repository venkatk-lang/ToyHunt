using UnityEngine;
using UnityEngine.SceneManagement;

namespace IACGGames
{
    public class GameSDKSystem : Singleton<GameSDKSystem>
    {
     
        public bool IsPaused { get; private set; }

      

        public void PauseGame()
        {
            IsPaused = true;
            Time.timeScale = 0;
            SettingsManager.Instance.ShowPauseMenu(true);
        }

        public void ResumeGame()
        {
            IsPaused = false;
            Time.timeScale = 1;
            SettingsManager.Instance.ShowPauseMenu(false);
        }

        public void RestartGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}