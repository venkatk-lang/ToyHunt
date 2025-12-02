using System.Collections.Generic;
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
    public Camera mainCamera;

    private List<ToyCell> cells = new List<ToyCell>();

    private float cellSize;
    private float cellSpacing;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        CreateGridCells();
    }

    private void CreateGridCells()
    {
        if (toyCellPrefab == null) return;

        foreach (Transform t in transform)
            Destroy(t.gameObject);
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

                ToyCell newCell = Instantiate(toyCellPrefab, pos, Quaternion.identity, transform);

                newCell.transform.localScale = Vector3.one * cellSize;

                cells.Add(newCell);
                index++;
            }
        }
    }


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
            cell.Show();
            cell.Debug(GameManager.Instance.IsNew(cell.Toy));
            
        }
    }
    
}
