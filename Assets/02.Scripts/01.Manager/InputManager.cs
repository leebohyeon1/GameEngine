using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _slidieAction;

    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool SlideInput { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _playerInput = GetComponent<PlayerInput>();

        InitialInputActions();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        HandleActions();
    }

    private void InitialInputActions()
    {
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _slidieAction = _playerInput.actions["Slide"];
    }

    private void HandleActions()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();

        JumpInput = _jumpAction.WasPressedThisFrame();

        SlideInput = _slidieAction.IsPressed();
    }

}
