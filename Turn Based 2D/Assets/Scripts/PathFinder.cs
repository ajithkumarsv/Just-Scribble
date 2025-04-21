using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class PathFinder : MonoBehaviour
{
    [SerializeField] private bool allowDiagonal = false;
    int y_range;
    [SerializeField] private TileData[] tileData;
    int2 minBounds;

    public IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        while (TileManager.Instance == null)
            yield return null;
        
        minBounds = new int2(TileManager.Instance.minBounds.x, TileManager.Instance.minBounds.y);
        y_range = TileManager.Instance.y_range;
        tileData = TileManager.Instance.GetTileDataArray();
    }

    public int2[] CalculatePath(int2 start, int2 end)
    {
        
        //only make this code to job system
        if (!IsPositionValid(start) || !IsPositionValid(end))
        {
            Debug.LogWarning("Invalid start/end position");
            return Array.Empty<int2>();
        }

        var openSet = new PriorityQueue();
        var closedSet = new HashSet<int2>();
        var cameFrom = new Dictionary<int2, int2>();
        var gScore = new Dictionary<int2, float>();
        var fScore = new Dictionary<int2, float>();
        
        gScore[start] = 0;
        fScore[start] = HeuristicCostEstimate(start, end);
        openSet.Enqueue(new Node(start, fScore[start]));

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();
            
            if (current.Position.Equals(end))
                return ReconstructPath(cameFrom, current.Position);

            closedSet.Add(current.Position);

            foreach (var neighbor in GetNeighbors(current.Position))
            {
                if (closedSet.Contains(neighbor)) continue;

                float moveCost = DistanceBetween(current.Position, neighbor);
                float tentativeGScore = gScore[current.Position] + moveCost;
                bool neighborInOpenSet = openSet.Contains(neighbor);

                if (!neighborInOpenSet || tentativeGScore < gScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    cameFrom[neighbor] = current.Position;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + HeuristicCostEstimate(neighbor, end);

                    if (!neighborInOpenSet)
                        openSet.Enqueue(new Node(neighbor, fScore[neighbor]));
                    else
                        openSet.UpdatePriority(neighbor, fScore[neighbor]);
                }
            }
        }
        return Array.Empty<int2>();
    }

    private int2[] ReconstructPath(Dictionary<int2, int2> cameFrom, int2 current)
    {
        List<int2> path = new List<int2> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path.ToArray();
    }

    private float HeuristicCostEstimate(int2 a, int2 b)
    {
        if (allowDiagonal)
        {
            int dx = math.abs(a.x - b.x);
            int dy = math.abs(a.y - b.y);
            return math.max(dx, dy);
        }
        return math.abs(a.x - b.x) + math.abs(a.y - b.y);
    }

    private float DistanceBetween(int2 a, int2 b)
    {
        int dx = math.abs(a.x - b.x);
        int dy = math.abs(a.y - b.y);
        return (dx + dy == 2 && allowDiagonal) ? 1.4142f : 1f;
    }

    private IEnumerable<int2> GetNeighbors(int2 pos)
    {
        int2[] cardinals = { new(0,1), new(1,0), new(0,-1), new(-1,0) };
        foreach (var d in cardinals)
        {
            int2 neighbor = pos + d;
            if (IsPositionValid(neighbor) && IsWalkable(neighbor))
                yield return neighbor;
        }

        if (allowDiagonal)
        {
            int2[] diagonals = { new(1,1), new(1,-1), new(-1,1), new(-1,-1) };
            foreach (var d in diagonals)
            {
                int2 neighbor = pos + d;
                int2 adj1 = new int2(pos.x + d.x, pos.y);
                int2 adj2 = new int2(pos.x, pos.y + d.y);
                
                if (IsPositionValid(neighbor) && 
                    IsWalkable(neighbor) &&
                    IsWalkable(adj1) && 
                    IsWalkable(adj2))
                {
                    yield return neighbor;
                }
            }
        }
    }

    private bool IsPositionValid(int2 pos)
    {
        int arrayX = pos.x - minBounds.x;
        int arrayY = pos.y - minBounds.y;
        return arrayX >= 0 && arrayX < (tileData.Length / y_range) &&
               arrayY >= 0 && arrayY < y_range;
    }

    private bool IsWalkable(int2 pos)
    {
        int index = TilePositionToArrayIndex(pos);
        return tileData[index].isHavingTile && 
               tileData[index].CanMove && 
               !tileData[index].isOccpuied;
    }

    private int TilePositionToArrayIndex(int2 pos)
    {
        int arrayX = pos.x - minBounds.x;
        int arrayY = pos.y - minBounds.y;
        return arrayX * y_range + arrayY;
    }
}

public class Node : IComparable<Node>
{
    public int2 Position { get; }
    public float Priority { get; }

    public Node(int2 pos, float priority)
    {
        Position = pos;
        Priority = priority;
    }

    public int CompareTo(Node other) => Priority.CompareTo(other.Priority);
}

public class PriorityQueue
{
    private List<Node> heap = new List<Node>();
    private Dictionary<int2, int> positionMap = new Dictionary<int2, int>();

    public int Count => heap.Count;
    public bool Contains(int2 pos) => positionMap.ContainsKey(pos);

    public void Enqueue(Node node)
    {
        heap.Add(node);
        positionMap[node.Position] = heap.Count - 1;
        HeapifyUp(heap.Count - 1);
    }

    public Node Dequeue()
    {
        Node first = heap[0];
        positionMap.Remove(first.Position);
        heap[0] = heap[^1];
        heap.RemoveAt(heap.Count - 1);
        if (heap.Count > 0)
        {
            positionMap[heap[0].Position] = 0;
            HeapifyDown(0);
        }
        return first;
    }

    public void UpdatePriority(int2 pos, float newPriority)
    {
        if (positionMap.TryGetValue(pos, out int index))
        {
            float old = heap[index].Priority;
            heap[index] = new Node(pos, newPriority);
            if (newPriority < old) HeapifyUp(index);
            else HeapifyDown(index);
        }
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (heap[index].CompareTo(heap[parent]) >= 0) break;
            Swap(index, parent);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        while (true)
        {
            int left = 2 * index + 1;
            if (left >= heap.Count) break;

            int smallest = left;
            int right = left + 1;
            if (right < heap.Count && heap[right].CompareTo(heap[left]) < 0)
                smallest = right;

            if (heap[index].CompareTo(heap[smallest]) <= 0) break;
            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        (heap[a], heap[b]) = (heap[b], heap[a]);
        positionMap[heap[a].Position] = a;
        positionMap[heap[b].Position] = b;
    }
}