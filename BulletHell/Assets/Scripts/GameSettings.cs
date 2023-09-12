using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntitySettings
{
    public int Lives;
    public float BulletVelocity;
    public float RespawnDelay;
    public float MinimalRotationInterval;
    public float MaximalRotationInterval;
    [Range(0, 360)] public float MinimalRotation;
    [Range(0, 360)] public float MaximalRotation;
}

[Serializable]
public class LevelSettings
{
    public int EntityCount;
    public Color Color;
    public EntitySettings EntitySettings;
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    public List<LevelSettings> LevelsSettings;
}
