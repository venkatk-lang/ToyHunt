using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WorldGridManager gridManager;
    public RoundGenerator roundGenerator;

    public GameState currentState { get; private set; }

    private List<ToyItem> roundItems;
    private List<string> selectedOrder = new List<string>();
    private HashSet<string> selectedSet = new HashSet<string>();
    private ToyItem lastSelected;
    private int currentRound = 1;
    private int maxRounds = 3;

    private void Start()
    {
        ChangeState(GameState.Init);
    }

    // ---------------------------
    // CHANGE STATE
    // ---------------------------
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
                // wait for player input via InputManager
                break;

            case GameState.End:
                EndRound();
                break;
        }
    }

    // ---------------------------
    // ROUND START
    // ---------------------------
    private void StartRound()
    {
        selectedOrder.Clear();
        selectedSet.Clear();
        lastSelected = null;

        roundItems = roundGenerator.BuildRound(currentRound);

        ChangeState(GameState.SpawnNew);
    }

    // ---------------------------
    // SPAWN GRID CELLS FOR BOX
    // ---------------------------
    private void SpawnBox()
    {
        List<ToyItem> boxItems = BuildBoxItems();
        gridManager.DisplayItems(boxItems);

        ChangeState(GameState.WaitForPlayer);
    }

    // ---------------------------
    // PLAYER CLICK HANDLED FROM INPUT MANAGER
    // ---------------------------
    public void OnToyCellClicked(ToyCell cell)
    {
        if (currentState != GameState.WaitForPlayer) return;
        if (cell == null || cell.Toy == null) return;

        ToyItem toy = cell.Toy;

        // check wrong pick
        if (selectedSet.Contains(toy.id))
        {
            cell.PlayIncorrect();
            ChangeState(GameState.End);
            return;
        }

        // correct pick
        cell.PlayCorrect();
        selectedSet.Add(toy.id);
        selectedOrder.Add(toy.id);
        lastSelected = toy;

        // finished?
        if (selectedSet.Count >= roundItems.Count)
        {
            ChangeState(GameState.End);
            return;
        }

        // continue
        StartCoroutine(NextSpawnDelay());
    }

    private IEnumerator NextSpawnDelay()
    {
        yield return new WaitForSeconds(0.35f);
        ChangeState(GameState.SpawnNew);
    }

    // ---------------------------
    // END ROUND HANDLING
    // ---------------------------
    private void EndRound()
    {
        Debug.Log($"Round {currentRound} ended. Score items: {selectedOrder.Count}");

        currentRound++;
        if (currentRound > maxRounds)
        {
            Debug.Log("Game Over");
            return;
        }

        StartCoroutine(StartNextRound());
    }

    private IEnumerator StartNextRound()
    {
        yield return new WaitForSeconds(1.0f);
        ChangeState(GameState.Init);
    }

    // ---------------------------
    // BOX ITEM GENERATION
    // ---------------------------
    private List<ToyItem> BuildBoxItems()
    {
        List<ToyItem> result = new List<ToyItem>();

        // Always include last selected
        if (lastSelected != null)
            result.Add(lastSelected);

        // Add all previous selected items
        foreach (var id in selectedSet)
        {
            ToyItem t = roundItems.Find(x => x.id == id);
            if (t != null && !result.Contains(t))
                result.Add(t);
        }

        // Add 3 new items
        int newNeeded = 3;

        foreach (var t in roundItems)
        {
            if (newNeeded <= 0) break;
            if (selectedSet.Contains(t.id)) continue;
            if (result.Contains(t)) continue;

            result.Add(t);
            newNeeded--;
        }

        return result;
    }
}
