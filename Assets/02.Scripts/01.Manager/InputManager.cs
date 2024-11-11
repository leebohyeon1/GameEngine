using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _jumpAction;


    public Vector2 MoveInput { get; private set; }
    public Vector2 JumpInput { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        _playerInput = GetComponent<PlayerInput>();

    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void SetUpInputActions() // ActionMap의 InputAction 초기화
    {
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
    }
}
