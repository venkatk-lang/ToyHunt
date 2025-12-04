using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Debug.Log("boxItems " + boxItems.Count);
        if (lastSelected == null) //its first time spawn in this round , no animation needed
        {
            gridManager.DisplayItems(boxItems);
            ChangeState(GameState.WaitForPlayer);
            return;
        }
        gridManager.AnimateAndDisplay(boxItems, () =>
        {
            ChangeState(GameState.WaitForPlayer);
        });

    }
    public void OnToyCellClicked(ToyCell cell)
    {
        if (currentState != GameState.WaitForPlayer) return;
        if (cell == null || cell.Toy == null) return;

        ToyItem toy = cell.Toy;

        if (selectedSet.Contains(toy))
        {
            cell.PlayIncorrect();
            HighlightAllCorrectCells();
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

       // StartCoroutine(NextSpawnDelay());
       StartCoroutine( Transition(0.4f, stepTransitionSettings));
    }
    private void HighlightAllCorrectCells()
    {
        foreach (var cell in gridManager.ActiveCells)
        {
            if (cell.Toy == null) continue;

            bool isNew = IsNew(cell.Toy);
            cell.Highlight(isNew);
        }
    }
    public bool IsNew(ToyItem boxToys)
    {
        return !selectedSet.Contains(boxToys);
    }

    //private IEnumerator NextSpawnDelay()
   // {
       // yield return new WaitForSeconds(0.35f);
      //  ChangeState(GameState.SpawnNew);
  //  }

 
    private void EndRound()
    {
        Debug.Log($"Round {currentRound} ended. Score items: {selectedSet.Count}");
        currentRound++;
        if (currentRound > maxRounds)
        {
            Debug.Log("Game Over - Show Summary screen");
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
    Action onTransitionBegin;
    Action onTransitionCutPointReached;
    Action onTransitionEnd;
    [SerializeField]GameObject transitionTemplate;
    [SerializeField]TransitionSettings roundTransitionSettings;
    [SerializeField]TransitionSettings stepTransitionSettings;
    IEnumerator Transition(float startDelay, TransitionSettings transitionSettings)
    {
        yield return new WaitForSeconds(startDelay);

        onTransitionBegin?.Invoke();

        GameObject template = Instantiate(transitionTemplate) as GameObject;
        template.GetComponent<Transition>().transitionSettings = transitionSettings;

        float transitionTime = transitionSettings.transitionTime;
        if (transitionSettings.autoAdjustTransitionTime)
            transitionTime = transitionTime / transitionSettings.transitionSpeed;

        yield return new WaitForSeconds(transitionTime);

        onTransitionCutPointReached?.Invoke();


        ChangeState(GameState.SpawnNew);

        yield return new WaitForSeconds(transitionSettings.destroyTime);

        onTransitionEnd?.Invoke();
    }
}
