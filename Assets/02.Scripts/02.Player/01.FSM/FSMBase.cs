using System.Collections.Generic;
using UnityEngine;

public class FSMBase
{
    protected Dictionary<string, FSMState> _states = new Dictionary<string, FSMState>();
    protected FSMState _currentState;

    public void AddState(string key, FSMState state)
    {
        if (!_states.ContainsKey(key))
            _states[key] = state;
    }

    public void SetState(string key, PlayerController player)
    {
        if (_states.ContainsKey(key))
        {
            _currentState?.OnExit(player);
            _currentState = _states[key];
            _currentState.OnEnter(player);
        }
    }

    public void Update(PlayerController player)
    {
        _currentState?.OnUpdate(player);
    }

    public void FixedUpdate(PlayerController player)
    {
        _currentState?.OnFixedUpdate(player);
    }

    public bool IsSameState(string key)
    {
        return _states[key] == _currentState;
    }
}