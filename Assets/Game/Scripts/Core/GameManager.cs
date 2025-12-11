using EasyTransition;
using IACGGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GameManager : GameManagerBase<GameManager>
{
    public WorldGridManager gridManager;
    public RoundGenerator roundGenerator;
    public LevelType levelType;

    private GameState currentState { get; set; }
    public GameState CurrentState => currentState;
    public Action<GameState> OnStateChanged;

    private List<ToyItem> remainingItems = new List<ToyItem>();
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

    public bool isTutorialMode;

    public Action OnCorrectClicked;
    public Action OnWrongClicked;
    public Action OnRoundStart;
    public Action OnRoundEnd;
    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Concrete Awake");
    }
    private void Start()
    {
      
        UIManager.Instance.Init();
       // AudioManager.Instance.PlayBGM(BGMAudioID.MainMenu, true);
    }

    public void StartGame(bool isTutorial)
    {
        isTutorialMode = isTutorial;

        UIManager.Instance.Show(UIState.GameHUD, 0.2f);
        ChangeState(GameState.Init);
        AudioManager.Instance.PlayBGM(BGMAudioID.Gameplay, true);
    }

    public void ChangeState(GameState newState)
    {
        Debug.Log("Current State " + newState);
        currentState = newState;
        OnStateChanged?.Invoke(currentState);
        switch (newState)
        {
            case GameState.Init:
                //setup
                ClearFeedback();
                gridManager.CreateGridCells(levelType.rows, levelType.cols);
                currentRound = 1;
                score = 0;
                selectedSet.Clear();
                UIManager.Instance.gameHUD.UpdateScore(score, selectedSet.Count);
                UIManager.Instance.gameHUD.UpdateRound(currentRound, maxRounds);
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
                OnRoundEnd?.Invoke();
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
        remainingItems.Clear();
        UIManager.Instance.gameHUD.UpdateRound(currentRound, maxRounds);
        UIManager.Instance.gameHUD.UpdateScore(0, selectedSet.Count);
        UIManager.Instance.gameHUD.ShowRoundStart(currentRound, maxRounds, isTutorialMode);

        wrongItem = null;
        lastSelected = null;
        if (isTutorialMode)
        {
            remainingItems = roundGenerator.BuildTutorialRound(levelType, 5);
            Debug.Log("TUT items");
        }
        else
        {
            remainingItems = roundGenerator.BuildRound(currentRound, levelType);
            Debug.Log("items " );
        }

        Debug.Log("Remaining " + remainingItems.Count);
        transitionController.StartTransition(2f, roundTransitionSettings, () => 
        {
         //   AudioManager.Instance.PlaySound(AudioID.PageIn);
            AudioManager.Instance.PlaySFX(SFXAudioID.PageIn);
        },
      () =>
      {
          OnRoundStart?.Invoke();
         // AudioManager.Instance.PlaySound(AudioID.PageOut);
          AudioManager.Instance.PlaySFX(SFXAudioID.PageOut);
          UIManager.Instance.gameHUD.CloseRoundStart();
          ChangeState(GameState.SpawnNew);
          if(!isTutorialMode) UIManager.Instance.gameHUD.ShowTopBar(true);

      }, () =>
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
        transitionController.StartTransition(0.4f, stepTransitionSettings, () => 
        {
            AudioManager.Instance.PlaySFX(SFXAudioID.Erase);
        },
            () =>
            {
                ClearFeedback();
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
            ShowFeedback(cell.transform.position, false);
            wrongItem = cell;
            cell.PlayIncorrect();
            HighlightAllCorrectCells();
            OnWrongClicked?.Invoke();
            ChangeState(GameState.RoundEnd);
            AudioManager.Instance.PlaySFX(SFXAudioID.Wrong);
            return;
        }
        ShowFeedback(cell.transform.position, true);
        cell.PlayCorrect();
        selectedSet.Add(toy);
        lastSelected = cell;
        remainingItems.Remove(toy);
        OnCorrectClicked?.Invoke();

        score += SaveDataHandler.Instance.GameConfig.ScoreEachCorrect;
        UIManager.Instance.gameHUD.UpdateScore(score, selectedSet.Count);
        AudioManager.Instance.PlaySFX(SFXAudioID.Correct);

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
        bool failEnd = wrongItem != null;
  
        if (isTutorialMode) // end tutorial when first round is completed.
        {
            if (!failEnd)
            {
                transitionController.StartTransition(0.4f, stepTransitionSettings, () => 
                {
                    AudioManager.Instance.PlaySFX(SFXAudioID.Erase);
                },
                  () =>
                  {
                      UIManager.Instance.gameHUD.ShowTopBar(false);
                      TutorialManager.Instance.SkipTutorial();
                      ClearFeedback();
                      gridManager.ClearAllItems();
                  }, () =>
                  {
                     
                      StartGame(false);
                  });
            }

                return;
        }

        score += GetRoundBonus();

        currentRound++;
        //show summary panel
        List<ToyItem> selectedToys = selectedSet.ToList();
        if (failEnd)
        {
            selectedToys.Add(wrongItem.Toy);
        }
        transitionController.StartTransition(failEnd?1f:0.4f, stepTransitionSettings, () =>
        {
            AudioManager.Instance.PlaySFX(SFXAudioID.Erase);
        },
              () =>
              {
                  ClearFeedback();
                  UIManager.Instance.gameHUD.ShowRoundSummary(selectedToys, wrongItem != null ? wrongItem.Toy.id : -1, currentRound > maxRounds);
                  gridManager.ClearAllItems();
              },null);



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

    public void ShowFeedback(Vector3 pos, bool correct)
    {
        currectFeedback = Instantiate(feedbackPrefab);
        currectFeedback.transform.position = pos;
        currectFeedback.Show(correct);
    }
    private void ClearFeedback()
    {
        if (currectFeedback != null) { Destroy(currectFeedback.gameObject); }
    }
 
    public override void OnPause()
    {
        // disable input
    }

    public override void OnResume()
    {
       // enable input
    }

    public override void OnRestart()
    {
        // restart level
    }

    public override void OnQuit()
    {
        // reset level
    }

    public override void OnStartTutorial()
    {
        // reset level
        TutorialManager.Instance.StartTutorial();
    }
}
