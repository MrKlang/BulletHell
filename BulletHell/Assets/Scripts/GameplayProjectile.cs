using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayProjectile : MonoBehaviour
{
    [HideInInspector] public Vector3 projectileDirection;
    [HideInInspector] public Transform projectileSpawner;

    private float _distanceToImpact;
    private float _movementTime;
    private Transform _target;

    private float currentLifetime = 5.0f;

    void Start()
    {
        RaycastHit2D[] circleCastResults = Physics2D.CircleCastAll(transform.position, GetComponent<CircleCollider2D>().radius, projectileDirection.normalized);

        Debug.LogError($"Circle cast results count: {circleCastResults.Length}");

        for(int i =0;i< circleCastResults.Length; i++)
        {
            if (circleCastResults[i].transform != projectileSpawner)
            {
                _target = circleCastResults[i].transform;
                _distanceToImpact = circleCastResults[i].distance;
                Debug.LogError($"Got target which is {_target.name}");
            }
        }

        _movementTime = _target != null ? _distanceToImpact / GameplayController.instance.CurrentLevelSettings.EntitySettings.BulletVelocity : 5.0f;

        StartCoroutine(MoveProjectile());
    }

    private IEnumerator MoveProjectile()
    {
        float currentTime = 0;
        while (currentTime < _movementTime)
        {
            currentTime += Time.deltaTime;
            transform.Translate(projectileDirection.normalized * Time.deltaTime);
            yield return null;
        }

        Debug.LogError("Destroy here!");
    }
}
