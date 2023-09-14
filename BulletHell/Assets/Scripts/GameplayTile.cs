using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayTileData
{
    public Vector3 bottomCorner;
    public Vector3 topCorner;
    public Vector3 position;
}

public class GameplayTile
{
    public GameplayEntity occupyingEntity;

    private GameplayTileData _data;
    public GameplayTileData Data 
    {
        get => _data;
        set
        {
            _data = value;
        }
    }

    public bool IsPositionInTile(Vector3 incomingObjectPosition)
    {
        if(incomingObjectPosition.x <= _data.topCorner.x && incomingObjectPosition.x >= _data.bottomCorner.x)
        {
            if(incomingObjectPosition.y <= _data.topCorner.y && incomingObjectPosition.y >= _data.bottomCorner.y)
            {
                return true;
            }
        }

        return false;
    }
}
