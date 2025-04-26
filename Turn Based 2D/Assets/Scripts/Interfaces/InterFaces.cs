using System.Collections;

public interface IUnit
{
    public abstract PlayerType GetPlayerType();
}
public interface IPlayable
{
    public abstract IEnumerator TakeTurn();
    public abstract IEnumerator EndTurn();
}