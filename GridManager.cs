using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages global translation workflows across multiple dynamic grid sub-sections.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private List<GridGenerator> gridSections = new List<GridGenerator>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void RegisterGrid(GridGenerator grid)
    {
        if (!gridSections.Contains(grid))
        {
            gridSections.Add(grid);
        }
    }

    /// <summary>
    /// Translates global 3D vector coordinates into discrete 2D map matrix array indices.
    /// </summary>
    public Vector2Int GetCellCoordinatesAtWorldPos(Vector3 worldPos)
    {
        foreach (var grid in gridSections)
        {
            Vector3 localPos = worldPos - grid.transform.position;

            int cellX = Mathf.FloorToInt(localPos.x / grid.cellSize);
            int cellY = Mathf.FloorToInt(localPos.z / grid.cellSize);

            if (cellX >= 0 && cellX < grid.gridWidth &&
                cellY >= 0 && cellY < grid.gridHeight)
            {
                return new Vector2Int(cellX, cellY);
            }
        }

        return Vector2Int.zero;
    }

    /// <summary>
    /// Maps 2D cell grid coordinates back to localized global center space vectors.
    /// </summary>
    public Vector3 GetWorldPosAtCell(Vector2Int cellCoord)
    {
        foreach (var grid in gridSections)
        {
            if (grid.ContainsCellCoordinates(cellCoord))
            {
                float worldX = cellCoord.x * grid.cellSize + grid.cellSize * 0.5f;
                float worldZ = cellCoord.y * grid.cellSize + grid.cellSize * 0.5f;

                return new Vector3(
                    grid.transform.position.x + worldX,
                    grid.transform.position.y, 
                    grid.transform.position.z + worldZ
                );
            }
        }

        return Vector3.zero;
    }

    public bool ContainsWorldPosition(Vector3 worldPos)
    {
        foreach (var grid in gridSections)
        {
            Vector3 localPos = worldPos - grid.transform.position;

            // Using explicit property configuration mappings matching target schemas
            int x = Mathf.FloorToInt(localPos.x / grid.cellSize);
            int y = Mathf.FloorToInt(localPos.z / grid.cellSize);

            if (x >= 0 && x < grid.gridWidth && y >= 0 && y < grid.gridHeight)
            {
                return true;
            }
        }

        return false;
    }
}