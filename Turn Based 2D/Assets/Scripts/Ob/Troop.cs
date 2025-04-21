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

    TileData trooptile;

    public void SetTileData(TileData tiledata)
    {
        trooptile = tiledata;
    }

    public new PlayerType GetType()
    {
        return playerType;
    }


    public IEnumerator TakeTurn()
    {

        int2[] path = TileManager.Instance.FindPath(trooptile.position, playerType);
        TileManager.Instance.DrawPath(path);
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Path" + path.Length);
        if (path.Length==0)
        {
            yield break;
        }

        for (int i = 0; i < path.Length; i++)
        {
            if (path.Length <= AttackRange)
            {
                 break;
            }

            // Debug.Log("Counting" + i);
            int ind = TileManager.Instance.TilePositionToArrayIndex(trooptile.position);

            TileData td = TileManager.Instance.GetCurrentTile(ind);
            td.isOccpuied = false;
            td.playerType = (byte)PlayerType.None;
            TileManager.Instance.SetTileData(ind, td);

            transform.position = TileManager.Instance.TilePositionToWorldPosition(path[i]);
            int index = TileManager.Instance.TilePositionToArrayIndex(path[i]);
            td = TileManager.Instance.GetCurrentTile(ind);
            td.isOccpuied = true;
            td.playerType = (byte)PlayerType.Player;
            trooptile = td;
            
            TileManager.Instance.SetTileData(index, td);
            yield return new WaitForSeconds(0.35f);


        }
    }
    public IEnumerator EndTurn()
    {
        throw new System.NotImplementedException();
    }

}
