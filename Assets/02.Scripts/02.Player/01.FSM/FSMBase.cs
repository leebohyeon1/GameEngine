using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FSMBase
{
    protected Dictionary<string, FSMState> _states = new Dictionary<string, FSMState>();
    protected FSMState _currentState;
    protected FSMState _previouState;

    public void AddState(string key, FSMState state)
    {
        if (!_states.ContainsKey(key))
            _states[key] = state;
    }

    public void SetState(string key, PlayerController player)
    {
        if (_states.ContainsKey(key))
        {
            if(_previouState == null)
            {
                _previouState = _states[key];
            }
            else
            {
                _previouState = _currentState;
            }
            _currentState?.OnExit(player);
            _currentState = _states[key];
            _currentState.OnEnter(player);
        }
    }

    public void SetState(FSMState fSMState, PlayerController player)
    {
        if (_states.ContainsValue(fSMState))
        {
            if (_previouState == null)
            {
                _previouState = fSMState;
            }
            else
            {
                _previouState = _currentState;
            }
            _currentState?.OnExit(player);
            _currentState = fSMState;
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

    public FSMState GetCurState()
    {
        return _currentState;
    }

    public FSMState GetPreState()
    {
        return _previouState;
    }
}