using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    public int rows = 5;
    public int cols = 7;
    public GameObject toyCellPrefab; // prefab with ToyCell component
    public RectTransform gridParent; // UI parent (GridLayoutGroup recommended)

    private List<ToyCell> cells = new List<ToyCell>();

    private void Awake()
    {
        CreateGridCells();
    }

    private void CreateGridCells()
    {
        if (toyCellPrefab == null || gridParent == null) return;

        // Clear existing
        foreach (Transform t in gridParent) Destroy(t.gameObject);
        cells.Clear();

        int total = rows * cols;
        for (int i = 0; i < total; i++)
        {
            var go = Instantiate(toyCellPrefab, gridParent);
            var cell = go.GetComponent<ToyCell>();
            cell.SetIndex(i);
            cells.Add(cell);
        }
    }

    /// <summary>
    /// Display the list of items in the first N cells where N = items.Count, rest remain empty/hidden.
    /// Items are already shuffled by GameManager.
    /// </summary>
    public void DisplayItems(List<ToyItem> items)
    {
        // Clear all cells first
        foreach (var c in cells) c.Clear();

        for (int i = 0; i < items.Count && i < cells.Count; i++)
        {
            cells[i].SetToy(items[i]);
            cells[i].Show();
        }
    }
}
