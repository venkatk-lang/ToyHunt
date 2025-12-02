using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public WorldGridManager gridManager;
    public RoundGenerator roundGenerator;

    public GameState currentState { get; private set; }

    private List<ToyItem> remainingItems;
    private HashSet<ToyItem> selectedSet = new HashSet<ToyItem>();
    private ToyItem lastSelected;
    private int currentRound = 1;
    private int maxRounds = 3;

    private void Start()
    {
        ChangeState(GameState.Init);
    }
    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.Init:
                StartRound();
                break;

            case GameState.SpawnNew:
                SpawnBox();
                break;

            case GameState.WaitForPlayer:

                break;

            case GameState.End:
                EndRound();
                break;
        }
    }

    private void StartRound()
    {
        selectedSet.Clear();
        lastSelected = null;

        remainingItems = roundGenerator.BuildRound(currentRound, gridManager.rows*gridManager.cols);
        Debug.Log("Remaining " + remainingItems.Count);
        ChangeState(GameState.SpawnNew);
    }

    private void SpawnBox()
    {
        List<ToyItem> boxItems = BuildBoxItems();
        gridManager.DisplayItems(boxItems);

        ChangeState(GameState.WaitForPlayer);
    }
    public void OnToyCellClicked(ToyCell cell)
    {
        if (currentState != GameState.WaitForPlayer) return;
        if (cell == null || cell.Toy == null) return;

        ToyItem toy = cell.Toy;

        if (selectedSet.Contains(toy))
        {
            cell.PlayIncorrect();
            ChangeState(GameState.End);
            return;
        }


        cell.PlayCorrect();
        selectedSet.Add(toy);
        lastSelected = toy;
        remainingItems.Remove(toy);


        if (remainingItems.Count == 0) 
        {
            ChangeState(GameState.End);
            return;
        }

        StartCoroutine(NextSpawnDelay());
    }

    public bool IsNew(ToyItem boxToys)
    {

        return !selectedSet.Contains(boxToys);
           
        
    }

    private IEnumerator NextSpawnDelay()
    {
        yield return new WaitForSeconds(0.35f);
        ChangeState(GameState.SpawnNew);
    }

 
    private void EndRound()
    {
        Debug.Log($"Round {currentRound} ended. Score items: {selectedSet.Count}");


        currentRound++;
        if (currentRound > maxRounds)
        {
            Debug.Log("Game Over - Show Summary screen");
            //Show Summary screen
            return;
        }

        StartCoroutine(StartNextRound());
    }

    private IEnumerator StartNextRound()
    {
        yield return new WaitForSeconds(1.0f);
        ChangeState(GameState.Init);
    }

   
    private List<ToyItem> BuildBoxItems()
    {
        List<ToyItem> result = new List<ToyItem>();
        foreach (var sToy in selectedSet)
        {
                result.Add(sToy);
        }

        int newNeeded = 3;

        foreach (var t in remainingItems)
        {
            if (newNeeded <= 0) break;
            result.Add(t);
            newNeeded--;
        }
       Helpers.ShuffleList(result);
        return result;
    }
}
