using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayProjectile : MonoBehaviour
{
    [HideInInspector] public Vector3 projectileDirection;
    [HideInInspector] public float projectileVelocity;
    [HideInInspector] public float projectileLifetime;

    public Action<GameplayProjectile> NotifyOnDestroy; 

    private Vector3 projectileSpawnPosition;
    private float currentLifetime = 0.0f;

    void Start()
    {
        projectileSpawnPosition = transform.position;
        GameplayController.onUpdate += OnUpdate;
    }

    private void OnDestroy()
    {
        NotifyOnDestroy?.Invoke(this);
        GameplayController.onUpdate -= OnUpdate;
    }

    private void OnUpdate()
    {
        if(currentLifetime <= projectileLifetime)
        {
            currentLifetime += Time.deltaTime;
            transform.Translate(projectileDirection.normalized * Time.deltaTime * projectileVelocity);
            GameplayController.instance.CheckTilesForCollision(this, projectileSpawnPosition);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
