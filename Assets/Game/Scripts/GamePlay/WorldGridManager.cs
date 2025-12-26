using IACGGames;
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

    public Transform gridParent;

    private void Awake()
    {
        gridParent.transform.position = Vector3.zero;

    }

    public void CreateGridCells(int levelRows,int levelColumns)
    {
        rows = levelRows;
        cols = levelColumns;
        if (toyCellPrefab == null) return;

        Helpers.DestroyChildren(gridParent);
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

                ToyCell newCell = Instantiate(toyCellPrefab, pos, Quaternion.identity, gridParent);
                newCell.Clear();
                newCell.transform.localScale = Vector3.one * cellSize;

                cells.Add(newCell);
                index++;
            }
        }
    }

    public void DisplayItems(List<ToyItem> items)
    {
        ClearAllItems();
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
            if(SaveDataHandler.Instance.GameConfig.ShowUsedItem)
            cell.Debug(GameManager.Instance.IsNew(cell.Toy));

        }
    }

    public void ClearAllItems()
    {
        foreach (var cell in cells)
            cell.Clear();
    }
    public void ShowAllActiveVisual()
    {
        Debug.Log("Active Visual " + ActiveCells.Count);
        foreach (var item in ActiveCells)
        {
            item.ActiveVisual();
        }
    }
}
