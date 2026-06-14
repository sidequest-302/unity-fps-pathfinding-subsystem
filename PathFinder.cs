using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles load-balanced, asynchronous A* pathfinding requests across custom spatial grids.
/// </summary>
public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Instance { get; private set; } 
    
    private readonly Queue<PathRequest> _requestQueue = new Queue<PathRequest>();
    private bool _isProcessingRequest;
    private GridGenerator _gridGenerator;

    public struct PathRequest
    {
        public Vector2Int Start;
        public Vector2Int End;
        public Action<List<Vector2Int>> Callback;

        public PathRequest(Vector2Int start, Vector2Int end, Action<List<Vector2Int>> callback)
        {
            Start = start;
            End = end;
            Callback = callback;
        }
    }

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);

        _gridGenerator = FindFirstObjectByType<GridGenerator>();
    }

    /// <summary>
    /// Enqueues a path request to be processed asynchronously over successive frames.
    /// </summary>
    public void RequestPath(Vector2Int start, Vector2Int end, Action<List<Vector2Int>> callback)
    {
        PathRequest newRequest = new PathRequest(start, end, callback);
        _requestQueue.Enqueue(newRequest);

        if (!_isProcessingRequest)
        {
            StartCoroutine(ProcessRequests());
        }
    }

    private IEnumerator ProcessRequests()
    {
        _isProcessingRequest = true;

        while (_requestQueue.Count > 0)
        {
            PathRequest current = _requestQueue.Dequeue();
            List<Vector2Int> path = FindPath(current.Start, current.End);

            current.Callback?.Invoke(path);

            // Yield control back to Unity engine loop to distribute frame load evenly
            yield return null;
        }

        _isProcessingRequest = false;
    }

    /// <summary>
    /// Executes a standard grid-based A* pathfinding operation.
    /// </summary>
    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int endPos)
    {
        // Open set optimization: using standard list for traversal flexibility
        List<Vector2Int> openSet = new List<Vector2Int> { startPos };
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int> { [startPos] = 0 };

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet[0];
            int bestF = int.MaxValue;
            
            // Linear search over open set to determine minimal algorithmic cost
            foreach (var node in openSet)
            {
                int g = gScore.TryGetValue(node, out int gVal) ? gVal : int.MaxValue;
                int f = g + Heuristic(node, endPos);
                if (f < bestF)
                {
                    bestF = f;
                    current = node;
                }
            }

            if (current == endPos)
                return ReconstructPath(cameFrom, current, startPos);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!_gridGenerator.ContainsCellCoordinates(neighbor))
                    continue;

                Cell neighborCell = _gridGenerator.GetCell(neighbor);
                if (neighborCell == null || !neighborCell.isWalkable || closedSet.Contains(neighbor))
                    continue;

                int tentativeG = gScore[current] + 1;

                if (!gScore.TryGetValue(neighbor, out int existingG) || tentativeG < existingG)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Vector2Int>(); // Graceful fallback for unreachable targets
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current, Vector2Int start)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (!current.Equals(start))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        // Cardinal direction checks for orthogonal 2D map node analysis
        return new List<Vector2Int>
        {
            new Vector2Int(cell.x + 1, cell.y),
            new Vector2Int(cell.x - 1, cell.y),
            new Vector2Int(cell.x, cell.y + 1),
            new Vector2Int(cell.x, cell.y - 1)
        };
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        // Manhattan distance calculation suited for non-diagonal orthogonal grids
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}