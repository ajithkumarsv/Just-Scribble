using UnityEngine;

[CreateAssetMenu(fileName = "GameLevel", menuName = "Scriptable Objects/GameLevel")]
public class GameLevel : ScriptableObject
{
    public int level;
    public int enemyCount;
    public int playerCount;
}
