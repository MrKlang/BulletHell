using System.Collections.Generic;
using UnityEngine;

public delegate void OnUpdate();
public delegate void OnBeginPlay();
public delegate void OnEntityPerished();

public class GameplayController : MonoBehaviour
{
    [SerializeField] private GameObject _entityPrefab;
    [SerializeField] private Grid _grid;
    [SerializeField] private Transform _graveyard;
    [SerializeField] private GameSettings _settings;

    private Camera _mainCamera;
    private Vector2 _minSpawnPosition;
    private Vector2 _maxSpawnPosition;
    private Vector2 _entitySize;
    private LevelSettings _currentlevelSettings;

    private List<Vector3> _availablePositionsForSpawn = new List<Vector3>();
    private Dictionary<Vector2, GameplayEntity> _spawnedTransforms = new Dictionary<Vector2, GameplayEntity>();

    public GameSettings Settings => _settings;
    public LevelSettings CurrentLevelSettings => _currentlevelSettings;

    public static GameplayController instance { get; private set; }
    
    public static OnUpdate onUpdate;
    public static OnBeginPlay onBeginPlay;
    public static OnEntityPerished onEntityPerished;

    public void BeginPlay()
    {
        EntitySettings entitySettings = _currentlevelSettings.EntitySettings;

        for (int i = 0; i < _currentlevelSettings.EntityCount; i++)
        {
            int index = Random.Range(0, _availablePositionsForSpawn.Count);
            Vector2 spawnPosition = _availablePositionsForSpawn[index];
            Transform newTransform = Instantiate(_entityPrefab, spawnPosition, Quaternion.identity, _grid.transform).transform;
            GameplayEntity currentGameplayEntity = newTransform.GetComponent<GameplayEntity>();
            currentGameplayEntity.Data = new GameplayEntityData 
            {   currentLives = entitySettings.Lives,
                bulletVelocity = entitySettings.BulletVelocity,
                maximalRotation = entitySettings.MaximalRotation,
                minimalRotation = entitySettings.MinimalRotation,
                minimalRotationInterval = entitySettings.MinimalRotationInterval,
                maximalRotationInterval = entitySettings.MaximalRotationInterval,
                respawnDelay = entitySettings.RespawnDelay
            };

            _spawnedTransforms.Add(spawnPosition, currentGameplayEntity);
            _availablePositionsForSpawn.RemoveAt(index);
        }
    }

    public void SetCurrentLevelSettings(int index)
    {
        _currentlevelSettings = _settings.LevelsSettings[index];
    }

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

        _mainCamera = Camera.main;
        _mainCamera.CalculateFrustumCorners(_mainCamera.rect, _mainCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustrumCorners);
        _entitySize = _entityPrefab.GetComponent<BoxCollider2D>().size * _entityPrefab.transform.lossyScale;
        _grid.cellSize = _entitySize;

        _minSpawnPosition = _mainCamera.transform.TransformVector(frustrumCorners[0]);
        _minSpawnPosition += _entitySize / 2;
        _maxSpawnPosition = _mainCamera.transform.TransformVector(frustrumCorners[2]);
        _maxSpawnPosition -= _entitySize / 2;

        CacheAvailablePositions();
    }

    private void Update()
    {
        onUpdate?.Invoke();
    }

    private void CacheAvailablePositions()
    {
        Vector3Int gridMin = _grid.WorldToCell(_minSpawnPosition);
        Vector3Int gridMax = _grid.WorldToCell(_maxSpawnPosition);

        for (int x = gridMin.x; x < gridMax.x; ++x)
        {
            for (int y = gridMin.y; y < gridMax.y; ++y)
            {
                Vector3 proposedSpawnPosition = _grid.GetCellCenterWorld(new Vector3Int(x, y));
                RaycastHit2D hit = Physics2D.BoxCast(proposedSpawnPosition, _entitySize, 0, Vector2.zero);
                
                if (hit.transform != null)
                {
                    continue;
                }

                _availablePositionsForSpawn.Add(proposedSpawnPosition);
            }
        }
    }
}
