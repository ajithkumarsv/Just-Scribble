using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;

public class EnemyFinder : MonoBehaviour
{
    [SerializeField] private bool allowDiagonal = false;
    private int y_range;
    [SerializeField] private TileData[] tileData;
    private int2 minBounds;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        while (TileManager.Instance == null)
            yield return null;

        minBounds = new int2(TileManager.Instance.minBounds.x, TileManager.Instance.minBounds.y);
        y_range = TileManager.Instance.y_range;
        tileData = TileManager.Instance.GetTileDataArray();
    }

    public int2[] FindEnemyInRange(int2 start, PlayerType playerType, int attackRange)
    {
        HashSet<int2> attackablePositions = new HashSet<int2>();
        if (!IsPositionValid(start))
        {
            Debug.LogWarning("Invalid start position");
            return attackablePositions.ToArray();
        }

        Queue<(int2 position, int distance)> queue = new Queue<(int2, int)>();
        HashSet<int2> visited = new HashSet<int2>();

        queue.Enqueue((start, 0));
        visited.Add(start);


        Queue<int2> tilesInrange = new Queue<int2>();

        for (int i = 0; i < attackRange; i++)
        {
            GetNeighbors(start);
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int currentDistance = current.distance;

            foreach (int2 neighbor in GetNeighborsInRange(current.position, attackRange))
            {
                

                if (!IsPositionValid(neighbor))
                    continue;

                if (IsEnemy(neighbor, playerType))
                {
                    attackablePositions.Add(neighbor);
                }
            }
        }

        return attackablePositions.ToArray();
    }
    public int2[] FindEnemyPath(int2 start, PlayerType playerType)
    {
        Debug.Log("calculating Path");
        if (!IsPositionValid(start))
        {
            Debug.LogWarning("Invalid start position");
            return Array.Empty<int2>();
        }

        Queue<int2> queue = new Queue<int2>();
        HashSet<int2> visited = new HashSet<int2>();
        Dictionary<int2, int2> cameFrom = new Dictionary<int2, int2>();

        queue.Enqueue(start);
        visited.Add(start);

        Debug.Log("Quees" + queue.Count);
        while (queue.Count > 0)
        {
            int2 current = queue.Dequeue();

            foreach (int2 neighbor in GetNeighbors(current))
            {
                if (!IsPositionValid(neighbor)) continue;

                if (IsEnemy(neighbor, playerType))
                {
                    cameFrom[neighbor] = current;
                    return ReconstructPath(cameFrom, neighbor);
                }

                if (IsWalkable(neighbor) && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }
        return Array.Empty<int2>();
    }

    private IEnumerable<int2> GetNeighborsInRange(int2 pos, int range)
    {
        for (int i = 0; i < range; i++)
        {
            for (int j = 0; j < range; j++)
            {
                if(i==0 && j==0) continue;
                int2[] cardinals = { new(i, j), new(i, j), new(i, -j), new(-i, j), new(-i, -j) };
                foreach (var d in cardinals)
                {
                    int2 neighbor = pos + d;
                    if (IsPositionValid(neighbor))
                        yield return neighbor;
                }

                // if (allowDiagonal)
                // {
                //     int2[] diagonals = { new(i, i), new(i, -i), new(-i, i), new(-i, -i) };
                //     foreach (var d in diagonals)
                //     {
                //         int2 neighbor = pos + d;
                //         int2 adj1 = new int2(pos.x + d.x, pos.y);
                //         int2 adj2 = new int2(pos.x, pos.y + d.y);

                //         if (IsPositionValid(neighbor) &&
                //             IsPositionValid(adj1) &&
                //             IsPositionValid(adj2) &&
                //             IsWalkable(adj1) &&
                //             IsWalkable(adj2))
                //         {
                //             yield return neighbor;
                //         }
                //     }
                // }
            }
        }

    }
    private IEnumerable<int2> GetNeighbors(int2 pos)
    {
        int2[] cardinals = { new(0, 1), new(1, 0), new(0, -1), new(-1, 0) };
        foreach (var d in cardinals)
        {
            int2 neighbor = pos + d;
            if (IsPositionValid(neighbor))
                yield return neighbor;
        }

        if (allowDiagonal)
        {
            int2[] diagonals = { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
            foreach (var d in diagonals)
            {
                int2 neighbor = pos + d;
                int2 adj1 = new int2(pos.x + d.x, pos.y);
                int2 adj2 = new int2(pos.x, pos.y + d.y);

                if (IsPositionValid(neighbor) &&
                    IsPositionValid(adj1) &&
                    IsPositionValid(adj2) &&
                    IsWalkable(adj1) &&
                    IsWalkable(adj2))
                {
                    yield return neighbor;
                }
            }
        }
    }

    private bool IsEnemy(int2 pos, PlayerType playerType)
    {
        int index = TilePositionToArrayIndex(pos);
        // return tileData[index].isHavingTile && 
        //tileData[index].CanMove && 
        //    tileData[index].isOccpuied;

        if (tileData[index].isHavingTile && tileData[index].isOccpuied)
        {
            byte type = (byte)tileData[index].playerType;
            if (type > 0 && type != (byte)playerType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
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