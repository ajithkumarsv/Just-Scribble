using System.Collections;
using UnityEngine;

public class GameManager : Singleton<MonoBehaviour>
{

    [SerializeField] EnemyController enemyController;
    [SerializeField] PlayerController playerController;

    public TurnState CurrentState = TurnState.HOLD_STATE;


    public IEnumerator Start()
    {
        yield return new WaitForSeconds(1);

        StartCoroutine(PlayGame());
        // YieplayerController.Take();
    }

    IEnumerator PlayGame()
    {
        yield return null;
        while (true)
        {
            
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


}