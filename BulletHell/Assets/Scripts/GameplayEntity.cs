using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayEntityData
{
    public int currentLives;
    public float bulletVelocity;
    public float respawnDelay;
    public float minimalRotationInterval;
    public float maximalRotationInterval;
    [Range(0, 360)] public float minimalRotation;
    [Range(0, 360)] public float maximalRotation;
}


public class GameplayEntity : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileSpawnPoint;

    private Vector3 _currentDesiredRotation = Vector3.zero;
    private float _currentRotationInterval = 0;
    private bool _isRotating;

    private GameplayEntityData _data;
    private WaitForSeconds _firingDelay = new WaitForSeconds(1.0f);

    public GameplayEntityData Data 
    {
        get => _data;
        set
        {
            _data = value;
        }
    }

    private void Start()
    {
        GameplayController.onUpdate += OnUpdate;
        StartCoroutine(OpenFire());
    }

    private void OnUpdate()
    {
        if (!_isRotating)
        {
            _isRotating = true;
            StartCoroutine(RotationRoutine());
        }
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
        GameObject spawnedProjectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, Quaternion.identity);
        spawnedProjectile.GetComponent<GameplayProjectile>().projectileDirection = transform.position - _projectileSpawnPoint.position;
    }
}
