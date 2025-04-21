using System;
using System.Collections.Generic;
using System.IO;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;



public class TileManager : Singleton<TileManager>
{
    // Singleton pattern to ensure only one instance of SelectionManager exists

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GameObject selectionPrefab;
    [SerializeField] private TileData[] tileData;


    [SerializeField] public Dictionary<int2, Troop> playerTroops = new Dictionary<int2, Troop>();
    [SerializeField] public Dictionary<int2, Troop> enemyTroops = new Dictionary<int2, Troop>();

    public Vector3Int minBounds;
    public Vector3Int maxBounds;
    public int x_range;
    public int y_range;



    int2 startPos;

    public List<GameObject> pathGameObject = new List<GameObject>();

    [SerializeField] public PathFinder pathFinder;
    [SerializeField] public EnemyFinder enemyFinder;
    public TileData currentTile { get; private set; }

    public Building buildingPrefab;


    protected override void Awake()
    {
        base.Awake();
    }


    public TileData[] GetTileDataArray()
    {
        return tileData;
    }
    void Start()
    {
        minBounds = tilemap.cellBounds.min;
        maxBounds = tilemap.cellBounds.max;
        x_range = maxBounds.x - minBounds.x + 1;
        y_range = maxBounds.y - minBounds.y + 1;

        tileData = new TileData[x_range * y_range];

        for (int x = minBounds.x; x <= maxBounds.x; x++)
        {
            for (int y = minBounds.y; y <= maxBounds.y; y++)
            {
                int arrayX = x - minBounds.x;
                int arrayY = y - minBounds.y;
                int index = arrayX * y_range + arrayY;

                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                tileData[index].position = new int2(x, y);
                tileData[index].isHavingTile = tile != null;
                tileData[index].CanMove = tile != null;
                tileData[index].isOccpuied = false;
            }
        }

        var playertroops = FindObjectsByType<Troop>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Troop item in playertroops)
        {
            if (item.GetType() == PlayerType.Player)
            {
                Vector3 v = item.transform.position;
                v.z = 0;
                Vector3Int v2 = tilemap.WorldToCell(v);
                int2 pos = new(v2.x, v2.y);
                playerTroops.Add(pos, item);
                int index = TilePositionToArrayIndex(pos);
                item.SetTileData(tileData[index]);
                tileData[index].isOccpuied = true;
                tileData[index].playerType = (byte)PlayerType.Player;
            }
            else if (item.GetType() == PlayerType.Enemy)
            {
                Vector3 v = item.transform.position;
                v.z = 0;
                Vector3Int v2 = tilemap.WorldToCell(v);
                int2 pos = new(v2.x, v2.y);
                enemyTroops.Add(pos, item);
                int index = TilePositionToArrayIndex(pos);
                item.SetTileData(tileData[index]);
                tileData[index].isOccpuied = true;
                tileData[index].playerType = (byte)PlayerType.Enemy;
            }
        }
        Debug.Log("Player troops Count " + playerTroops.Count);
        Debug.Log("enemy troops ount" + enemyTroops.Count);
    }


    public int TilePositionToArrayIndex(int2 tilePos)
    {
        int arrayX = tilePos.x - minBounds.x;
        int arrayY = tilePos.y - minBounds.y;
        int index = arrayX * y_range + arrayY;
        return index;

    }
    public Vector3 TilePositionToWorldPosition(int2 tilePos)
    {
        Vector3Int tpos = new Vector3Int(tilePos.x, tilePos.y, 0);
        return tilemap.GetCellCenterWorld(tpos);
    }
    public TileData GetCurrentTile(int index)
    {
        return tileData[index];
    }
    public bool IsInTileBounds(Vector2Int tilePos)
    {
        if (tilePos.x < minBounds.x || tilePos.x > maxBounds.x || tilePos.y < minBounds.y || tilePos.y > maxBounds.y)
            return false;
        return true;
    }
    public void SetTileData(int index, TileData data)
    {
        if (index >= 0 && index < tileData.Length)
        {
            tileData[index] = data;
        }
    }
    public void SetTileData(int2 tilePos, TileData data)
    {
        int index = TilePositionToArrayIndex(tilePos);
        if (index >= 0 && index < tileData.Length)
        {
            tileData[index] = data;
        }
    }
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tpos = tilemap.WorldToCell(mousePos);
        tpos.z = 0;

        if (!IsInTileBounds((Vector2Int)tpos))

        {
            selectionPrefab.SetActive(false);
            return;
        }

        // int arrayX = tpos.x - minBounds.x;
        // int arrayY = tpos.y - minBounds.y;
        int index = TilePositionToArrayIndex(new int2(tpos.x, tpos.y));
        if (index >= 0 && index < tileData.Length)
        {
            currentTile = tileData[index];
            selectionPrefab.SetActive(currentTile.isHavingTile);
            selectionPrefab.transform.position = tilemap.GetCellCenterWorld(tpos);
            Debug.Log(currentTile.CanMove);
        }
        else
        {
            currentTile = new TileData();
            selectionPrefab.SetActive(false);
        }
        Debug.Log(" current tile  " + currentTile);

        DebugPath(mousePos);
        DebugBuilding(mousePos);

    }
    public void DebugBuilding(Vector3 mousePos)
    {
        if (Input.GetMouseButtonDown(0) && currentTile.isHavingTile && !currentTile.isOccpuied)
        {
            Vector3Int t = tilemap.WorldToCell(mousePos);

            Building build = Instantiate(buildingPrefab, TilePositionToWorldPosition(new int2(t.x, t.y)), Quaternion.identity);

            foreach (Transform tr in build.GetAllPoints())
            {

                Vector3Int tpos = tilemap.WorldToCell(tr.position);
                int index = TilePositionToArrayIndex(new int2(tpos.x, tpos.y));
                TileData data = tileData[index];

                if (!data.isHavingTile || data.isOccpuied)
                {
                    Debug.Log("Tile is not having tile");
                    Destroy(build.gameObject);
                    return;
                }

            }
            foreach (Transform tr in build.GetAllPoints())
            {

                Vector3Int tpos = tilemap.WorldToCell(tr.position);
                int index = TilePositionToArrayIndex(new int2(tpos.x, tpos.y));
                TileData data = tileData[index];

                data.isHavingTile = true;
                data.CanMove = false;
                data.isOccpuied = true;
                tileData[index] = data;
            }
        }
    }

    public void DebugPath(Vector3 mousePos)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int t = tilemap.WorldToCell(mousePos);
            startPos = new int2(t.x, t.y);
        }

        if (Input.GetMouseButton(1))
        {
            Debug.Log("CAlculating path");
            foreach (GameObject obj in pathGameObject)
            {
                Destroy(obj);
            }
            pathGameObject.Clear();
            Vector3Int t = tilemap.WorldToCell(mousePos);

            int2 epos = new int2(t.x, t.y);
            // int2[] path = pathFinder.CalculatePath(startPos,epos);
            int2[] path = enemyFinder.FindEnemyPath(startPos, PlayerType.Enemy);

            Debug.Log(" path length" + path.Length);
            for (int i = 0; i < path.Length; i++)
            {
                pathGameObject.Add(Instantiate(selectionPrefab, TilePositionToWorldPosition(path[i]), Quaternion.identity));
            }
        }
    }

    public void DrawPath(int2[] path)
    {
        foreach (GameObject obj in pathGameObject)
        {
            Destroy(obj);
        }
        pathGameObject.Clear();
        for (int i = 0; i < path.Length; i++)
        {
            pathGameObject.Add(Instantiate(selectionPrefab, TilePositionToWorldPosition(path[i]), Quaternion.identity));
        }
    }

    public int2[] FindPath(int2 startPos, PlayerType playerType)
    {
        return enemyFinder.FindEnemyPath(startPos, playerType);
    }
}