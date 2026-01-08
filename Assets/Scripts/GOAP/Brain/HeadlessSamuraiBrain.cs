using System;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeadlessSamuraiBrain : Brain
{


    protected override void OnValidate()
    {
        base.OnValidate();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _provider.ClearGoal();

    }

    protected override void Awake()
    {
        if(_provider != null && _goap != null)
        {
            if(_provider.AgentTypeBehaviour == null)
            {
                _provider.AgentType = _goap.GetAgentType("HeadlessSamurai");
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        _provider.RequestGoal<ChasePlayerGoal>(false);
    }

    protected override void OnActionEnd(IAction action)
    {
        if (!gameObject.activeSelf) return;
        if(_isPlayerInCombatRange)
        {
            switch (Personality)
            {
                case PersonalityType.Aggressive:
                    _provider.RequestGoal<KillPlayerGoal, StrafeGoal>();
                    break;
                case PersonalityType.Cautious:
                    _provider.RequestGoal<KillPlayerCautiousGoal, StrafeGoal>();
                    break;
                case PersonalityType.Evasive:
                    _provider.RequestGoal<KillPlayerGoal, StrafeGoal>(); // TODO: Add evasive goal
                    break;
            }
        }
        else
        {
            _provider.RequestGoal<ChasePlayerGoal>();
        }
        

    }

    protected override void OnPlayerEnterCombatRange(Transform player)
    {
        _provider.ClearGoal();
        switch(Personality)
        {
            case PersonalityType.Aggressive:
                _provider.RequestGoal<KillPlayerGoal, StrafeGoal>();
                break;
            case PersonalityType.Cautious:
                _provider.RequestGoal<KillPlayerCautiousGoal, StrafeGoal>();
                break;
            case PersonalityType.Evasive:
                _provider.RequestGoal<KillPlayerGoal, StrafeGoal>(); // TODO: Add evasive goal
                break;
        }
        
        _isPlayerInCombatRange = true;
    }

    protected override void OnPlayerExitCombatRange(Vector3 lastKnownPosition)
    {
        _provider.ClearGoal();
        _provider.RequestGoal<ChasePlayerGoal>();
        _isPlayerInCombatRange = false;
    }

    

    
}
