using System.Collections.Generic;
using UnityEngine;

public class WorldGridManager : MonoBehaviour
{
    [Header("Grid")]
    public int rows = 5;
    public int cols = 7;
    public float cellSize = 1f;         // FIXED SIZE
    public float cellSpacing = 0.1f;
    public Vector2 gridOffset;          // optional manual shift

    [Header("References")]
    public Camera mainCamera;
    public ToyCell toyCellPrefab; 

    private List<ToyCell> cells = new List<ToyCell>();

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        CreateGridCells();
    }

    // ----------------------------
    // CREATE WORLD-SPACE GRID
    // ----------------------------
    private void CreateGridCells()
    {
        if (toyCellPrefab == null) return;

        // Clear old
        foreach (Transform t in transform)
            Destroy(t.gameObject);
        cells.Clear();

        // -------------------------
        // CALCULATE TOTAL GRID SIZE
        // -------------------------
        float totalWidth = cols * cellSize + (cols - 1) * cellSpacing;
        float totalHeight = rows * cellSize + (rows - 1) * cellSpacing;

        // Center point
        Vector2 gridCenter = Vector2.zero;

        // Start (top-left)
        Vector2 startPos = new Vector2(
            gridCenter.x - totalWidth * 0.5f + cellSize * 0.5f,
            gridCenter.y + totalHeight * 0.5f - cellSize * 0.5f
        );

        int index = 0;

        // -------------------------
        // SPAWN GRID
        // -------------------------
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector2 pos = new Vector2(
                    startPos.x + c * (cellSize + cellSpacing),
                    startPos.y - r * (cellSize + cellSpacing)
                ) + gridOffset;

                ToyCell newCell = Instantiate(toyCellPrefab, pos, Quaternion.identity, transform);
                newCell.transform.localScale = Vector3.one * cellSize;

                newCell.SetIndex(index);
                cells.Add(newCell);
                index++;
            }
        }
    }

    public void DisplayItems(List<ToyItem> items)
    {

        foreach (var cell in cells)
            cell.Clear();

        for (int i = 0; i < items.Count && i < cells.Count; i++)
        {
            cells[i].SetToy(items[i]);
            cells[i].Show();
        }
    }
}
