using EasyTransition;
using IACGGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    public WorldGridManager gridManager;
    public RoundGenerator roundGenerator;
    public LevelType levelType;

    private GameState currentState { get; set; }
    public GameState CurrentState => currentState;
    public Action<GameState> OnStateChanged;

    private List<ToyItem> remainingItems;
    private HashSet<ToyItem> selectedSet = new HashSet<ToyItem>();
    public int TotalCorrectItemCount => selectedSet.Count;
    private ToyCell lastSelected;
    private int currentRound = 1;
    public int CurrentRound => currentRound;
 
    private int maxRounds = 3;
    private int score = 0;
    public int Score => score;
    private ToyCell wrongItem;
    [SerializeField] TransitionController transitionController;
    [SerializeField] TransitionSettings roundTransitionSettings;
    [SerializeField] TransitionSettings stepTransitionSettings;
    [SerializeField] FeedbackPopup feedbackPrefab; 
    FeedbackPopup currectFeedback; 
    private void Start()
    {
        
        SaveDataHandler.Instance.InitializeSavingSystem();
        UIManager.Instance.Init();
        gridManager.CreateGridCells(levelType.rows, levelType.cols);
    }
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(currentState);
        switch (newState)
        {
            case GameState.Init:
                //setup
                currentRound = 1;
                score = 0;
                selectedSet.Clear();
                UIManager.Instance.gameHUD.UpdateScore(score,selectedSet.Count);
                UIManager.Instance.gameHUD.UpdateRound(currentRound,maxRounds);
                ChangeState(GameState.StartRound);
                break;
            case GameState.StartRound:
                //show top UI
                StartRound();
                break;
            case GameState.SpawnNew:
                SpawnBox();
                break;
            case GameState.WaitForPlayer:
                break;
            case GameState.RoundEnd:
                EndRound();
                break;
            case GameState.GameEnd:
                //This is temporary, we need to implement close game here and submit score
                UIManager.Instance.Show(UIState.GameHUD, 0.2f);
                ChangeState(GameState.Init);
                break;
        }
    }

    private void StartRound()
    {
        selectedSet.Clear();
        UIManager.Instance.gameHUD.UpdateRound(currentRound, maxRounds);    
        UIManager.Instance.gameHUD.UpdateScore(0, selectedSet.Count);    
        UIManager.Instance.gameHUD.ShowRoundStart(currentRound, maxRounds);    
        wrongItem = null;
        lastSelected = null;
        remainingItems = roundGenerator.BuildRound(currentRound,levelType);
        Debug.Log("Remaining " + remainingItems.Count);
        transitionController.StartTransition(2f, roundTransitionSettings, null,
      () =>
      {
          UIManager.Instance.gameHUD.CloseRoundStart();
          ChangeState(GameState.SpawnNew);

      }, ()=> 
      {

          ChangeState(GameState.WaitForPlayer);
          gridManager.ShowAllActiveVisual();
      });
       
    }

    private void SpawnBox()
    {
        List<ToyItem> boxItems = BuildBoxItems();
        Debug.Log("boxItems " + boxItems.Count);
        if (lastSelected == null) //its first time spawn in this round , no animation needed
        {
            gridManager.DisplayItems(boxItems);

            return;
        }
        transitionController.StartTransition(0.4f, stepTransitionSettings,null,
            () =>
            {
                ClerFeedback();
                gridManager.DisplayItems(boxItems);
            }, () => 
            {
                gridManager.ShowAllActiveVisual(); 
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
            ShowFeedback(cell.transform.position,false);
            wrongItem = cell;
            cell.PlayIncorrect();
            HighlightAllCorrectCells();
            ChangeState(GameState.RoundEnd);
            return;
        }
        ShowFeedback(cell.transform.position, true);
        cell.PlayCorrect();
        selectedSet.Add(toy);
        lastSelected = cell;
        remainingItems.Remove(toy);


        score += SaveDataHandler.Instance.GameConfig.ScoreEachCorrect;
        UIManager.Instance.gameHUD.UpdateScore(score, selectedSet.Count);

        if (remainingItems.Count == 0) 
        {
            ChangeState(GameState.RoundEnd);
            return;
        }
        ChangeState(GameState.SpawnNew);
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
    public int GetRoundBonus()
    {
        return SaveDataHandler.Instance.GameConfig.RoundBonusScoreMultiplier * TotalCorrectItemCount * currentRound;
    }
    private void EndRound()
    {
   
        score += GetRoundBonus();

        currentRound++;
        //show summary panel
        List<ToyItem> selectedToys = selectedSet.ToList();
        if (wrongItem != null)
        {
            selectedToys.Add(wrongItem.Toy);
        }
        transitionController.StartTransition(0.4f, stepTransitionSettings, null,
              () =>
              {
                  ClerFeedback();
                  UIManager.Instance.gameHUD.ShowRoundSummary(selectedToys, wrongItem!=null? wrongItem.Toy.id:-1, currentRound>maxRounds);
                  gridManager.ClearAllItems();
              }, () =>
              {
                  //enable next button, start animation of summary screen
              });

       

    }
    //Call from summary button
    public void StartNextRound()
    {
        UIManager.Instance.gameHUD.CloseRoundSummary();
        ChangeState(GameState.StartRound);
       
    }
    public void EndGame()
    {
        UIManager.Instance.gameHUD.CloseRoundSummary();
        ChangeState(GameState.GameEnd);

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
 
    public void ShowFeedback(Vector3 pos,bool correct)
    {
        currectFeedback = Instantiate(feedbackPrefab);
        currectFeedback.transform.position = pos;
        currectFeedback.Show(correct);
    }
    private void ClerFeedback()
    {
        if (currectFeedback != null) { Destroy(currectFeedback.gameObject); }
    }
}
