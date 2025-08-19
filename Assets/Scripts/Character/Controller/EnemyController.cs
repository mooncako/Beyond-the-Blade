using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;

public class EnemyController : Controller, IPoolable
{
    [field: SerializeField, FoldoutGroup("Base Reference")] private PlayerSensor _playerSensor;

    [BoxGroup("Debug"), ReadOnly] public bool CanMove = true;
    [BoxGroup("Debug"), ReadOnly] public bool CanAttack = true;
    
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] public Transform CurrentTargetTransform;

    

    protected override void OnValidate()
    {
        base.OnValidate();
        if (_playerSensor == null) _playerSensor = GetComponentInChildren<PlayerSensor>();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        _playerSensor.OnPlayerEnter += playerTransform =>
        {
            CurrentTargetTransform = playerTransform;
            Movement.LookInMoveDirection = false;
        };
    }

    protected override void OnDisable()
    {
        _playerSensor.OnPlayerEnter -= playerTransform =>
        {
            CurrentTargetTransform = playerTransform;
            Movement.LookInMoveDirection = false;
        };
    }

    private void Update()
    {
        if (CurrentTargetTransform != null)
        {
            Movement.SetLookPosition(CurrentTargetTransform.position);
        }
    }


    public void MoveTo(Vector3 destination)
    {
        if (CanMove)
        {
            Movement.MoveTo(destination);
        }
        else
        {
            Movement.Stop();
        }
    }

    public void Stop()
    {
        Movement.Stop();
    }

    public void OnPoolGet()
    {
        throw new System.NotImplementedException();
    }

    public void OnPoolReturn()
    {
        throw new System.NotImplementedException();
    }
}
