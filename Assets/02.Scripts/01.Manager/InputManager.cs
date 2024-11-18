using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private InputAction _placeAction;
    private InputAction _placeModeAction;
    private InputAction _ItemSelectAction;

    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool PlaceInput { get; private set; }
    public bool IsPlaceMode {  get; private set; }
    public bool ItemSelectInput { get; private set; }
    public bool ItemSelectPressed { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _playerInput = GetComponent<PlayerInput>();

        SetUpInputActions();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        HandleActions();
    }

    private void SetUpInputActions() // ActionMap�� InputAction �ʱ�ȭ
    {
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _lookAction = _playerInput.actions["Look"];
        _placeAction = _playerInput.actions["Place"];
        _placeModeAction = _playerInput.actions["PlaceMode"];
        _ItemSelectAction = _playerInput.actions["ItemSelect"];
    }

    private void HandleActions() // �׼� �� �޴� �Լ�
    {
        MoveInput = _moveAction.ReadValue<Vector2>();  

        JumpInput = _jumpAction.WasPressedThisFrame(); 

        LookInput = _lookAction.ReadValue<Vector2>();

        PlaceInput = _placeAction.WasPressedThisFrame();

        IsPlaceMode = _placeModeAction.IsPressed();

        ItemSelectInput = _ItemSelectAction.WasPressedThisFrame();

        ItemSelectPressed = _ItemSelectAction.IsPressed();
    }

    public void SwitchToActionMap(string actionMapName)
    {
        if (_playerInput != null)
        {
            // ���� ���� ����
            _playerInput.SwitchCurrentActionMap(actionMapName);
        }
    }
}
