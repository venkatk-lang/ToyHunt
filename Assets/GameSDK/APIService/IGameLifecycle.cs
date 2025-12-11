
public interface IGameLifecycle
{
    void OnPause();
    void OnResume();
    void OnRestart();
    void OnQuit();
    void OnStartTutorial();
}