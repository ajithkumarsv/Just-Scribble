using Unity.Mathematics;

[System.Serializable]
public struct TileData
{
    public int2 position;
    public bool isHavingTile;
    public bool CanMove;

    public bool isAdjacentTile;
    public byte playerType;
    public bool isOccpuied; // Check spelling if needed
    public override string ToString()
    {
        return $"x,y : {position.x}, {position.y}, isHavingTile : {isHavingTile}, CanMove : {CanMove}, isOccpuied : {isOccpuied}, playerType : {playerType}";
    }
}