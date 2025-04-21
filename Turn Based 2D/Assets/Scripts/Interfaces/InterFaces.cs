using System.Collections;

public interface IUnit
{
    public abstract PlayerType GetType();
}
public interface IPlayable
{
    public abstract IEnumerator TakeTurn();
    public abstract IEnumerator EndTurn();
}