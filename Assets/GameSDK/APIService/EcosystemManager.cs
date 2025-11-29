using UnityEngine;
using System.Threading.Tasks;

public class EcosystemManager : Singleton<EcosystemManager>
{
    public PlayerData playerData;

    public IEcosystemCallbacks gameCallbacks;


    // Called when a game starts
    public void OnGameStart()
    {
        Debug.Log($"Game started for {playerData.playerName}");
        gameCallbacks?.OnGameStart();
    }

    // Called when game over happens
    public async void OnGameOver(int score)
    {
        Debug.Log($"Game over! Score: {score}");
        gameCallbacks?.OnGameOver(score);
        await APIService.UpdatePlayerScore(playerData.playerId, score);
    }

    // Load player data
    public async Task InitializePlayer(string playerId)
    {
        playerData = await APIService.GetPlayerData(playerId);
        if (playerData == null)
        {
            Debug.LogWarning("Failed to fetch player data. Using defaults.");
            playerData = new PlayerData(playerId, "Guest");
        }
    }
}