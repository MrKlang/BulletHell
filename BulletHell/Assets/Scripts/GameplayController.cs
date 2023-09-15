using System.Collections.Generic;
using UnityEngine;

#region Delegates

public delegate void OnUpdate();
public delegate void OnBeginPlay();
public delegate void OnEntityPerished();

#endregion

public class GameplayController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private GameObject _entityPrefab;
    [SerializeField] private Grid _grid;
    [SerializeField] private Transform _graveyard;
    [SerializeField] private GameSettings _settings;

    #endregion

    #region Private Fields

    private Camera _mainCamera;
    private Vector2 _minSpawnPosition;
    private Vector2 _maxSpawnPosition;
    private Vector2 _entitySize;
    private LevelSettings _currentlevelSettings;
    private List<GameplayTile> _availableTilesForSpawn = new List<GameplayTile>();
    private List<GameplayTile> _occupiedTilesForSpawn = new List<GameplayTile>();
    private List<GameplayEntity> _liveEntities = new List<GameplayEntity>();

    #endregion

    #region Public Properties

    public GameSettings Settings => _settings;
    public LevelSettings CurrentLevelSettings => _currentlevelSettings;

    #endregion

    #region Statics
    public static GameplayController instance { get; private set; }
    public static OnUpdate onUpdate;
    public static OnBeginPlay onBeginPlay;
    public static OnEntityPerished onEntityPerished;

    #endregion

    #region Public Methods

    public void BeginPlay()
    {
        _liveEntities.Clear();
        EntitySettings entitySettings = _currentlevelSettings.EntitySettings;

        for (int i = 0; i < _currentlevelSettings.EntityCount; i++)
        {
            GameplayTile tile = _availableTilesForSpawn[Random.Range(0, _availableTilesForSpawn.Count)];
            Transform newTransform = Instantiate(_entityPrefab, tile.Data.position, Quaternion.identity, _grid.transform).transform;
            GameplayEntity currentGameplayEntity = newTransform.GetComponent<GameplayEntity>();
            currentGameplayEntity.Data = new GameplayEntityData 
            {   currentLives = entitySettings.Lives,
                bulletVelocity = entitySettings.BulletVelocity,
                bulletLifetime = entitySettings.BulletLifetime,
                maximalRotation = entitySettings.MaximalRotation,
                minimalRotation = entitySettings.MinimalRotation,
                minimalRotationInterval = entitySettings.MinimalRotationInterval,
                maximalRotationInterval = entitySettings.MaximalRotationInterval,
                respawnDelay = entitySettings.RespawnDelay
            };

            tile.occupyingEntity = currentGameplayEntity;
            _liveEntities.Add(currentGameplayEntity);
            _occupiedTilesForSpawn.Add(tile);
            _availableTilesForSpawn.Remove(tile);
        }
    }

    public void CheckTilesForCollision(GameplayProjectile projectile, Vector3 originPosition)
    {
        for(int i =0; i< _occupiedTilesForSpawn.Count; i++)
        {
            if(_occupiedTilesForSpawn[i].IsPositionInTile(projectile.transform.position) && !_occupiedTilesForSpawn[i].occupyingEntity.firedProjectiles.Contains(projectile))
            {
                MoveEntityToGraveyard(_occupiedTilesForSpawn[i].occupyingEntity);
                EmptyTile(_occupiedTilesForSpawn[i]);
                projectile.NotifyEntity();
                break;
            }
        }
    }

    public void RespawnEntity(GameplayEntity entity)
    {
        GameplayTile tile = _availableTilesForSpawn[Random.Range(0, _availableTilesForSpawn.Count)];
        tile.occupyingEntity = entity;
        entity.transform.position = tile.Data.position;
        entity.transform.parent = _grid.transform;

        _occupiedTilesForSpawn.Add(tile);
        _availableTilesForSpawn.Remove(tile);
    }

    public void SetCurrentLevelSettings(int index)
    {
        _currentlevelSettings = _settings.LevelsSettings[index];
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        Vector3[] frustrumCorners = new Vector3[4];

        onBeginPlay += BeginPlay;
        onEntityPerished += PerformGameOverCheck;

        _mainCamera = Camera.main;
        _mainCamera.CalculateFrustumCorners(_mainCamera.rect, _mainCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustrumCorners);
        _entitySize = _entityPrefab.GetComponent<BoxCollider2D>().size * _entityPrefab.transform.lossyScale;
        _grid.cellSize = _entitySize;

        _minSpawnPosition = _mainCamera.transform.TransformVector(frustrumCorners[0]);
        _minSpawnPosition += _entitySize / 2;                                           //Maximal position will always be inside the frustrum, however minimal position might be outside/below so this is a precaution
        _maxSpawnPosition = _mainCamera.transform.TransformVector(frustrumCorners[2]);

        PrepareTiles();
    }

    private void PerformGameOverCheck()
    {
        if(_liveEntities.Count == 1)
        {
            _liveEntities[0].StopAllCoroutines();
            Destroy(_liveEntities[0].gameObject);
            FindObjectOfType<UIController>().ToggleGameOver(true);
        }
    }

    private void Update()
    {
        onUpdate?.Invoke();
    }

    private void PrepareTiles()
    {
        Vector3Int gridMin = _grid.WorldToCell(_minSpawnPosition);
        Vector3Int gridMax = _grid.WorldToCell(_maxSpawnPosition);

        for (int x = gridMin.x; x < gridMax.x; ++x)
        {
            for (int y = gridMin.y; y < gridMax.y; ++y)
            {
                Vector3 proposedSpawnPosition = _grid.GetCellCenterWorld(new Vector3Int(x, y));
                Vector3 tileOffsetBasedOnSize = _entitySize / 2;
                GameplayTile newTile = new GameplayTile();
                newTile.Data = new GameplayTileData { position = proposedSpawnPosition, topCorner = proposedSpawnPosition + tileOffsetBasedOnSize, bottomCorner = proposedSpawnPosition - tileOffsetBasedOnSize };
                _availableTilesForSpawn.Add(newTile);
            }
        }
    }

    private void EmptyTile(GameplayTile tile)
    {
        tile.occupyingEntity = null;
        _availableTilesForSpawn.Add(tile);
        _occupiedTilesForSpawn.Remove(tile);
    }

    private void MoveEntityToGraveyard(GameplayEntity entity)
    {
        if (entity != null)
        {
            entity.transform.parent = _graveyard;
            entity.transform.localPosition = Vector3.zero;
            entity.Data.currentLives--;

            if (entity.Data.currentLives == 0)
            {
                _liveEntities.Remove(entity);
            }

            entity.IsRespawning = true;
        }
    }

    #endregion
}
