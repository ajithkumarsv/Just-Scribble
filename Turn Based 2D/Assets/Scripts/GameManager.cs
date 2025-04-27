using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] EnemyController enemyController;
    [SerializeField] PlayerController playerController;

    public TurnState CurrentState = TurnState.HOLD_STATE;


    public IEnumerator Start()
    {
        // Application.targetFrameRate =60;
        yield return new WaitForSeconds(2);

        StartCoroutine(PlayGame());
        // YieplayerController.Take();
    }


    IEnumerator PlayGame()
    {
        yield return null;
        int turns = 100;
        while (true)
        {
            turns--;
            Debug.Log("Turns " + turns);
            if (CurrentState == TurnState.PLAYER_TURN)
            {
                Debug.Log("Palayer Turn");
                yield return StartCoroutine(playerController.TakeTurn());
                CurrentState = TurnState.ENEMY_TURN;
            }

            else if (CurrentState == TurnState.ENEMY_TURN)
            {
                Debug.Log("Enemy Turn");
                yield return StartCoroutine(enemyController.TakeTurn());
                CurrentState = TurnState.PLAYER_TURN;
            }


        }

    }

    public void OnEnemyDead(Troop troop)
    {
        if (troop != null)
        {
            TileManager.Instance.RemoveTroopFromList(troop.trooptile.position, troop.GetPlayerType());
            int index = TileManager.Instance.TilePositionToArrayIndex(troop.trooptile.position);
            TileData td = TileManager.Instance.GetCurrentTile(index);
            td.isOccpuied = false;
            td.playerType = (byte)PlayerType.None;
            TileManager.Instance.SetTileData(index, td);
            Destroy(troop.gameObject);
        }
        if(TileManager.Instance.IsEnemyEmpty(PlayerType.Player))
        {
             Debug.LogWarning("All Enemy Dead Player Won");
        }
        
         else if(TileManager.Instance.IsEnemyEmpty(PlayerType.Enemy))
        {
            Debug.LogWarning("All Players Dead Enemy Won");

        }

    }
    public void OnEnemyDead(int2 tilePos, PlayerType playerType)
    {
        int index = TileManager.Instance.TilePositionToArrayIndex(tilePos);
        TileData td = TileManager.Instance.GetCurrentTile(index);
        td.isOccpuied = false;
        td.playerType = (byte)PlayerType.None;
        TileManager.Instance.SetTileData(index, td);
        TileManager.Instance.RemoveTroopFromList(tilePos, playerType);
    }
}