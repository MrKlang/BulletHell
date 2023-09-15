using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayEntityData
{
    public int currentLives;
    public float bulletVelocity;
    public float bulletLifetime;
    public float respawnDelay;
    public float minimalRotationInterval;
    public float maximalRotationInterval;
    [Range(0, 360)] public float minimalRotation;
    [Range(0, 360)] public float maximalRotation;
}

public class GameplayEntity : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileSpawner;
    [SerializeField] private Transform _projectilePool;

    #endregion

    #region Private Fields

    private float _respawnInSeconds;
    private bool _isRespawning;

    private Vector3 _currentDesiredRotation = Vector3.zero;
    private float _currentRotationInterval = 0;
    private bool _isRotating;

    private GameplayEntityData _data;
    private WaitForSeconds _firingDelay = new WaitForSeconds(1.0f);
    private WaitForSeconds _respawnInterval;
    private Queue<GameplayProjectile> _projectiles;

    #endregion

    #region Public Fields

    [HideInInspector] public List<GameplayProjectile> firedProjectiles = new List<GameplayProjectile>();

    #endregion

    #region Public Properties

    public bool IsRespawning
    {
        get => _isRespawning;
        set
        {
            if (value.Equals(true))
            {
                StopAllCoroutines();
                _respawnInSeconds = _data.respawnDelay;
                _respawnInterval = new WaitForSeconds(_respawnInSeconds);
                _isRotating = false;
                StartCoroutine(RespawnRoutine());
            }

            _isRespawning = value;
        }
    }

    public GameplayEntityData Data 
    {
        get => _data;
        set
        {
            _data = value;
        }
    }

    #endregion

    #region Private Methods

    private void Start()
    {
        _projectiles = new Queue<GameplayProjectile>();
        for (int i =0; i < _projectilePool.childCount; i++)
        {
            _projectiles.Enqueue(_projectilePool.GetChild(i).GetComponent<GameplayProjectile>());
        }

        GameplayController.onUpdate += OnUpdate;
        Fire();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        GameplayController.onUpdate -= OnUpdate;
        GameplayController.onEntityPerished?.Invoke();
    }

    private void Fire()
    {
        if (_isRespawning)
        {
            return;
        }

        StartCoroutine(OpenFire());
    }

    private void OnUpdate()
    {
        if (_isRespawning)
        {
            return;
        }

        if (!_isRotating)
        {
            _isRotating = true;
            StartCoroutine(RotationRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        if (_data.currentLives.Equals(0))
        {
            Destroy(gameObject);
        }

        yield return _respawnInterval;
        GameplayController.instance.RespawnEntity(this);
        _isRespawning = false;
        Fire();
    }

    private IEnumerator RotationRoutine()
    {
        _currentDesiredRotation.z = Random.Range(0, 360);
        _currentRotationInterval = Random.Range(0, 1);

        yield return new WaitForSeconds(_currentRotationInterval);

        transform.Rotate(_currentDesiredRotation);
        _isRotating = false;
    }

    private IEnumerator OpenFire()
    {
        yield return _firingDelay;
        GameplayProjectile projectile = _projectiles.Dequeue();
        projectile.projectileDirection = transform.position - _projectileSpawner.position;
        projectile.projectileVelocity = _data.bulletVelocity;
        projectile.projectileLifetime = _data.bulletLifetime;

        if (projectile.NotifyCreatorOnCollision == null)
        {
            projectile.creator = transform;
            projectile.NotifyCreatorOnCollision += RemoveProjectileFromOwned;
        }

        firedProjectiles.Add(projectile);

        projectile.transform.parent = null;
        projectile.transform.localScale = Vector3.one;
        projectile.projectileImage.enabled = true;
        
        Fire();
    }

    private void RemoveProjectileFromOwned(GameplayProjectile projectile)
    {
        if(transform == null)
        {
            Destroy(projectile.gameObject);
            return;
        }

        projectile.projectileLifetime = 0;
        projectile.projectileImage.enabled = false;
        projectile.transform.parent = _projectilePool; //null shouldn't happen but OH WELL
        projectile.transform.localPosition = Vector3.zero;

        firedProjectiles.Remove(projectile);
        _projectiles.Enqueue(projectile);
    }

    #endregion
}
