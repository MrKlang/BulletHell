using System;
using UnityEngine;

public class GameplayProjectile : MonoBehaviour
{
    #region Public Fields

    public SpriteRenderer projectileImage;
    [HideInInspector] public Vector3 projectileDirection;
    [HideInInspector] public float projectileVelocity;
    [HideInInspector] public float projectileLifetime;
    [HideInInspector] public Transform creator;

    public Action<GameplayProjectile> NotifyCreatorOnCollision;

    #endregion

    #region Private Fields

    private Vector3 projectileSpawnPosition;
    private float currentLifetime = 0.0f;

    #endregion

    #region Public Methods

    public void NotifyEntity()
    {
        currentLifetime = 0;

        if (creator != null)
        {
            NotifyCreatorOnCollision?.Invoke(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Private Methods

    private void Start()
    {
        projectileSpawnPosition = transform.position;
        GameplayController.onUpdate += OnUpdate;
    }

    private void OnDestroy()
    {
        GameplayController.onUpdate -= OnUpdate;
    }

    private void OnUpdate()
    {
        if(currentLifetime < projectileLifetime)
        {
            currentLifetime += Time.deltaTime;
            transform.Translate(projectileDirection.normalized * Time.deltaTime * projectileVelocity);
            GameplayController.instance.CheckTilesForCollision(this, projectileSpawnPosition);
        }
        else
        {
            if (!currentLifetime.Equals(0))
            {
                NotifyEntity();
            }
        }
    }

    #endregion
}
