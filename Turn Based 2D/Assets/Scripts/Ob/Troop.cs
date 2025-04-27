using System.Collections;
using Unity.Mathematics;
using UnityEngine;


public class Troop : MonoBehaviour, IUnit, IPlayable
{
    [SerializeField]
    PlayerType playerType;
    [SerializeField] int MovementSpeed = 2;
    [SerializeField] int AttackRange = 1;
    [SerializeField] int AttackDamage = 40;

    public TileData trooptile;
    Troop targetTroop;
    TileData targetTile;
    public void SetTileData(TileData tiledata)
    {
        trooptile = tiledata;
    }

    public PlayerType GetPlayerType()
    {
        return playerType;
    }


    // public IEnumerator TakeTurn()
    // {
    //     Debug.Log("Point 1");

    //     int2[] path = TileManager.Instance.FindPath(trooptile.position, playerType);
    //     TileManager.Instance.DrawPath(path);
    //     yield return new WaitForSeconds(0.3f);
    //     Debug.Log("Path" + path.Length);
    //     Debug.Log("Point 2");
    //     if (path.Length == 0 )
    //     {
    //         Debug.Log("No Path Found or Path is too short");
    //         // TileManager.Instance.ClearPath();
    //         yield break;
    //     }
    //     Debug.Log("Point 3");
    //     if (path.Length > MovementSpeed)
    //     {
    //         for (int i = 0; i < path.Length; i++)
    //         {


    //             // Debug.Log("Counting" + i);
    //             // int ind = TileManager.Instance.TilePositionToArrayIndex(trooptile.position);

    //             // TileData td = TileManager.Instance.GetCurrentTile(ind);
    //             // td.isOccpuied = false;
    //             // td.playerType = (byte)PlayerType.None;
    //             // TileManager.Instance.SetTileData(ind, td);
    //             int index = TileManager.Instance.TilePositionToArrayIndex(trooptile.position);
    //             trooptile.isOccpuied = false;
    //             trooptile.playerType = (byte)PlayerType.None;
    //             TileManager.Instance.SetTileData(index, trooptile);

    //             yield return null;
    //             transform.position = TileManager.Instance.TilePositionToWorldPosition(path[i]);
    //             index = TileManager.Instance.TilePositionToArrayIndex(path[i]);
    //             TileData td = TileManager.Instance.GetCurrentTile(index);
    //             td.isOccpuied = true;
    //             td.playerType = (byte)playerType;
    //             trooptile = td;
    //             TileManager.Instance.SetTileData(index, td);
    //             // int index = TileManager.Instance.TilePositionToArrayIndex(path[i]);
    //             // td = TileManager.Instance.GetCurrentTile(ind);
    //             // td.isOccpuied = true;
    //             // td.playerType = (byte)PlayerType.Player;
    //             // trooptile = td;

    //             // TileManager.Instance.SetTileData(index, td);
    //             yield return new WaitForSeconds(0.35f);
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log("Path i stoo short");
    //         // TileManager.Instance.ClearPath();
    //     }

    //     Debug.Log("Point 4");
    // }
    public IEnumerator TakeTurn()
    {
        Debug.Log("Point 1");
        
        int2[] path = TileManager.Instance.FindPath(trooptile.position, playerType);
        TileManager.Instance.DrawPath(path);
       yield return new WaitForSeconds(TileManager.Instance.troopDelay);

        Debug.Log("Path length: " + path.Length);
        Debug.Log("Point 2");

        if (path.Length == 0)
        {
            Debug.Log("No Path Found");
            yield break;
        }
        
        if(targetTroop !=null)
        {
            Debug.Log("Target Troop Found");
        }
        yield return null;
        var enemyTiles =TileManager.Instance.findEnemiesInRange(trooptile.position,AttackRange,playerType);
        if (enemyTiles.Length>0)
        {
            
             yield return StartCoroutine(AttackEnemy(enemyTiles));
             yield break;
        }
        Debug.Log("Point 3");

        int stepsToMove = Mathf.Min(path.Length, MovementSpeed);
        for (int i = 0; i < stepsToMove; i++)
        {
            // Clear current tile data
            int newIndex = TileManager.Instance.TilePositionToArrayIndex(path[i]);
            TileData newTile = TileManager.Instance.GetCurrentTile(newIndex);
            if (!newTile.isOccpuied)
            {
                int currentIndex = TileManager.Instance.TilePositionToArrayIndex(trooptile.position);
                trooptile.isOccpuied = false;
                trooptile.playerType = (byte)PlayerType.None;
                TileManager.Instance.SetTileData(currentIndex, trooptile);
               
                // Move to next position
                transform.position = TileManager.Instance.TilePositionToWorldPosition(path[i]);

                // Update new tile data
                TileManager.Instance.ReplaceTroopTile(trooptile.position,path[i],this);
                newTile.isOccpuied = true;
                newTile.playerType = (byte)playerType;
                TileManager.Instance.SetTileData(newIndex, newTile);
                trooptile = newTile;

                yield return new WaitForSeconds(TileManager.Instance.troopDelay);
                // if (path.Length <= AttackRange + 1)
                // {
                //     yield return StartCoroutine(AttackEnemy(enemyTiles));
                //     yield break;
                // }
            }
        }
        Debug.Log("Point 4");
        // TileManager.Instance.ClearPath();
    }

    public IEnumerator AttackEnemy(int2[] enemyTiles)
    {
        int index = TileManager.Instance.TilePositionToArrayIndex(enemyTiles[0]);
        targetTile=TileManager.Instance.GetCurrentTile(index);
        targetTroop = TileManager.Instance.GetTroopAtTile(enemyTiles[0]);
        TileManager.Instance.DrawTarget(enemyTiles);
        if(targetTile.isOccpuied && targetTroop != null)
        {
            targetTroop.GetComponent<Health>().TakeDamage(AttackDamage);
        }   
        Debug.Log("Attack Enemy");
       yield return new WaitForSeconds(TileManager.Instance.troopDelay);
        yield break;
    }

    public IEnumerator EndTurn()
    {
        throw new System.NotImplementedException();
    }

}
