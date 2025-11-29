using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using IACGGames;
public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    public RoundGenerator roundGenerator;
    public GridManager gridManager;
    public BoxController boxController;
    public UIManager uiManager;
  //  public AudioManager audioManager;

    [Header("Game Settings")]
    public int totalRounds = 3;
    public int maxItemsPerRound = 35;

    // runtime state
    private int currentRound = 1;
    private List<ToyItem> roundItems = new List<ToyItem>(); // 35 unique items for round
    private HashSet<string> selectedIds = new HashSet<string>(); // selected this round
    private ToyItem lastSelected = null;
    private int currentDisplayedCount = 3; // grows by +1 each correct pick until 35

    private int correctSelectionsThisRound = 0;
    private int totalScore = 0;

    // events for UI/other systems
    public UnityEvent onRoundStart;
    public UnityEvent onRoundEnd;

    private System.Random rng = new System.Random();

    private void Start()
    {
        StartRound(1);
    }

    public void StartRound(int roundNumber)
    {
        currentRound = roundNumber;
        selectedIds.Clear();
        lastSelected = null;
        correctSelectionsThisRound = 0;
        currentDisplayedCount = 3;

        // Build round item list
        roundItems = roundGenerator.BuildRound(roundNumber);

        uiManager.SetRoundNumber(roundNumber);
        uiManager.ClearRoundSummary();

        onRoundStart?.Invoke();

        // Show initial box (3 items)
        ShowNextBox();
    }

    private void ShowNextBox()
    {
        // If we've selected all unique items, show the completion phase (two final boxes).
        if (selectedIds.Count >= roundItems.Count)
        {
            // Show final boxes: they still display all collected toys but no new ones left.
            currentDisplayedCount = Mathf.Min(maxItemsPerRound, roundItems.Count);
            var items = BuildBoxItems(finalPhase: true);
            gridManager.DisplayItems(items);
            boxController.PlayInAnimation(() => { /* awaiting player selection (which will be wrong) */ });
            return;
        }

        // compute next box size: +1 each time until 35 (but at least 3)
        currentDisplayedCount = Mathf.Clamp(3 + selectedIds.Count, 3, maxItemsPerRound);

        var boxItems = BuildBoxItems(finalPhase: false);
        gridManager.DisplayItems(boxItems);
        boxController.PlayInAnimation(() => { /* playable */ });
    }

    /// <summary>
    /// Build items that will be displayed in the next box according to rules:
    /// - Include lastSelected (if not null)
    /// - Include previously selected items as needed such that the total item count is currentDisplayedCount
    /// - Include 3 new unique items if available (or as many as available)
    /// </summary>
    private List<ToyItem> BuildBoxItems(bool finalPhase)
    {
        var result = new List<ToyItem>();
        var usedIds = new HashSet<string>();

        // 1) lastSelected mandatory (if exists and not already included)
        if (lastSelected != null && !usedIds.Contains(lastSelected.id))
        {
            result.Add(lastSelected);
            usedIds.Add(lastSelected.id);
        }

        // 2) Add previously selected items (old items) to reach target (but keep room for 3 new items if present)
        // We'll add previously selected items first (except lastSelected which is already added)
        foreach (var id in selectedIds)
        {
            if (result.Count >= currentDisplayedCount) break;
            if (usedIds.Contains(id)) continue;
            var toy = roundItems.Find(t => t.id == id);
            if (toy != null)
            {
                result.Add(toy);
                usedIds.Add(id);
            }
        }

        // 3) Add new items (not in selectedIds and not included yet) — ensure we add up to 3 new toys each time if possible.
        if (!finalPhase)
        {
            // Try to add 3 new ones (or as many as available)
            int newNeeded = 3;
            // but also ensure total doesn't exceed currentDisplayedCount
            newNeeded = Mathf.Min(newNeeded, currentDisplayedCount - result.Count);

            var candidates = new List<ToyItem>();
            foreach (var t in roundItems)
            {
                if (usedIds.Contains(t.id)) continue;
                if (selectedIds.Contains(t.id)) continue;
                candidates.Add(t);
            }
            Shuffle(candidates);

            for (int i = 0; i < candidates.Count && newNeeded > 0; i++)
            {
                result.Add(candidates[i]);
                usedIds.Add(candidates[i].id);
                newNeeded--;
            }
        }
        else
        {
            // final phase: fill with collected toys (which will be repeats)
            // just ensure we display up to currentDisplayedCount
            var allCollectedList = new List<ToyItem>();
            foreach (var t in roundItems)
            {
                if (selectedIds.Contains(t.id)) allCollectedList.Add(t);
            }
            Shuffle(allCollectedList);
            foreach (var t in allCollectedList)
            {
                if (result.Count >= currentDisplayedCount) break;
                if (!usedIds.Contains(t.id))
                {
                    result.Add(t);
                    usedIds.Add(t.id);
                }
            }
        }

        // 4) If still slots left (due to small DB or edge cases), fill with any remaining round items
        foreach (var t in roundItems)
        {
            if (result.Count >= currentDisplayedCount) break;
            if (!usedIds.Contains(t.id))
            {
                result.Add(t);
                usedIds.Add(t.id);
            }
        }

        // 5) Shuffle positions for display
        Shuffle(result);
        return result;
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    /// <summary>
    /// Called by GridManager/ToyCell when user taps a toy.
    /// </summary>
    public void OnToySelected(ToyItem toy, ToyCell cell)
    {
        // Prevent double act
        if (toy == null) return;

        bool isRepeat = selectedIds.Contains(toy.id);

        if (isRepeat)
        {
            // Wrong — end round
            cell.PlayIncorrect();
            //audioManager.PlayIncorrect();
            uiManager.ShowRoundSummary(roundItems, selectedIds, toy);
            EndRound(false, toy);
            return;
        }

        // Correct
        selectedIds.Add(toy.id);
        lastSelected = toy;
        correctSelectionsThisRound++;
        totalScore += 500;
        uiManager.UpdateScore(totalScore);
        cell.PlayCorrect();

        // Check if collected all 35 unique items
        if (selectedIds.Count >= roundItems.Count)
        {
            // award bonus for the round and present final summary
            int bonus = 100 * selectedIds.Count * currentRound;
            totalScore += bonus;
            uiManager.UpdateScore(totalScore);
            uiManager.ShowRoundSummary(roundItems, selectedIds, null); // no duplicate
            EndRound(true, null);
            return;
        }

        // otherwise continue: spawn next box after a short delay to allow toy animation
        StartCoroutine(DelayedNextBox(0.55f));
    }

    private IEnumerator DelayedNextBox(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowNextBox();
    }

    /// <summary>
    /// Clean up and proceed to next round or end game.
    /// </summary>
    private void EndRound(bool success, ToyItem duplicatedToy)
    {
        // give round bonus even on success handled earlier
        onRoundEnd?.Invoke();

        if (!success && duplicatedToy != null)
        {
            // If wrong selection, compute bonus for the picks accumulated before error
            int bonus = 100 * selectedIds.Count * currentRound;
            totalScore += bonus;
            uiManager.UpdateScore(totalScore);
        }

        // Advance to next round after delay (or show continue button)
        StartCoroutine(ProceedToNextPhase(1.0f));
    }

    private IEnumerator ProceedToNextPhase(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentRound < totalRounds)
        {
            StartRound(currentRound + 1);
        }
        else
        {
            // Game over
            uiManager.ShowFinalSummary(totalScore);
           // audioManager.PlayGameOver();
        }
    }
}
