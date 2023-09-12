using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntitySettings
{
    public int Lives;
    public float BulletVelocity;
    public float RespawnDelay;
}

[Serializable]
public class LevelSettings
{
    public int EntityCount;
    public Color color;
    public EntitySettings entitySettings;
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    public List<LevelSettings> levelsSettings;
}
