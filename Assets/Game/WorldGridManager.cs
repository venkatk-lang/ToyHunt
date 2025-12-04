using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGridManager : MonoBehaviour
{
    [Header("Grid")]
    public int rows = 5;
    public int cols = 7;

    [Header("Play Area")]
    public float boxWidth = 16f;  
    public float boxHeight = 9f;

    [Header("References")]
    public ToyCell toyCellPrefab;

    private List<ToyCell> cells = new List<ToyCell>();
    public IReadOnlyList<ToyCell> ActiveCells => cells.Where(t => t.Toy != null).ToList().AsReadOnly();

    private float cellSize;
    private float cellSpacing;

    public BoxController box;

    private void Awake()
    {
        CreateGridCells();
    }

    private void CreateGridCells()
    {
        if (toyCellPrefab == null) return;

        Helpers.DestroyChildren(transform);
        cells.Clear();

        float cellW = boxWidth / cols;

  
        float cellH = boxHeight / rows;

        cellSize = Mathf.Min(cellW, cellH);

        cellSpacing = cellSize * 0.10f;

       
        float totalWidth = cols * cellSize + (cols - 1) * cellSpacing;
        float totalHeight = rows * cellSize + (rows - 1) * cellSpacing;

        Vector2 startPos = new Vector2(
            -totalWidth * 0.5f + cellSize * 0.5f,
             totalHeight * 0.5f - cellSize * 0.5f
        );

    
        int index = 0;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector2 pos = new Vector2(
                    startPos.x + c * (cellSize + cellSpacing),
                    startPos.y - r * (cellSize + cellSpacing)
                );

                ToyCell newCell = Instantiate(toyCellPrefab, pos, Quaternion.identity, box.transform);

                newCell.transform.localScale = Vector3.one * cellSize;

                cells.Add(newCell);
                index++;
            }
        }
    }

    Coroutine boxAnimation;
    public void DisplayItems(List<ToyItem> items)
    {
        foreach (var cell in cells)
            cell.Clear();
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < cells.Count; i++)
            availableIndices.Add(i);

        Helpers.ShuffleList(availableIndices);

        Helpers.ShuffleList(items);

        int spawnCount = items.Count;

        for (int i = 0; i < spawnCount; i++)
        {
            int cellIndex = availableIndices[i];
            ToyCell cell = cells[cellIndex];

            cell.SetToy(items[i]);
            cell.Debug(GameManager.Instance.IsNew(cell.Toy));

        }
    }

    public void AnimateAndDisplay(List<ToyItem> items, Action OnComplete)
    {
        boxAnimation = StartCoroutine(box.PlayAnimation(() =>
        {
            DisplayItems(items);

        }, () => { OnComplete?.Invoke(); }));
    }

    private void OnDestroy()
    {
        if (boxAnimation != null)
        {
            StopCoroutine(boxAnimation);
        }
    }
}
