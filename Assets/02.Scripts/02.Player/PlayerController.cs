using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputManager _inputManager;
    private CharacterController _characterController;
    private PlayerMovement _playerMovement;
    private FSMBase _fsm;

    private Transform _cameraTransform;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public Animator Animator { get; private set; }  

    #region Stat
    [Space(20f)]
    [SerializeField] private float _moveSpeed; //�̵� �ӵ�
    [SerializeField] private float _xRotationSpeed = 5f; // ȸ�� �ӵ�
    [SerializeField] private float _yRotationSpeed = 5f; // ȸ�� �ӵ�

    private float minOffsetY = 0.1f;  // ī�޶��� �ּ� Y ������
    private float maxOffsetY = 10f;  // ī�޶��� �ִ� Y ������
    #endregion

    private float _rotationY;
    private float _currentOffsetY;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _characterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();

        SetFSM();
    }

    private void Start()
    {
        _inputManager = InputManager.Instance;
        _cameraTransform = Camera.main.transform;

        // �ʱ� ī�޶� offset Y ����
        _currentOffsetY = virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        _fsm.Update(this);
    }

    #region InitialFunc

    private void SetFSM()
    {
        _fsm = new FSMBase();
        _fsm.AddState("Idle", new IdleState(_fsm));
        _fsm.AddState("Move", new MoveState(_fsm));

        _fsm.SetState("Idle", this);
    }

    #endregion

    #region MovementFunc

    public void Move()
    {
        // InputManager���� �Է� ���� �޾ƿ���
        Vector2 moveInput = _inputManager.MoveInput;

        // ī�޶��� ������ �������� �̵� ���� ����
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;

        // ���� ���ʹ� �����Ͽ� ��鿡�� �̵��ϵ��� ��
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // ī�޶� ������ �������� �̵� ���� ���
        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // �÷��̾� �̵�
        _characterController.Move(moveDirection * _moveSpeed * Time.deltaTime);

        // ���� Ʈ���� �Ķ���Ͱ� �Է�
        Animator.SetFloat("XDir", moveInput.x);
        Animator.SetFloat("YDir", moveInput.y);
    }

    public void Rotate()
    {
        Vector2 lookInput = _inputManager.LookInput;

        // �¿� ȸ�� ����
        _rotationY += lookInput.x * _xRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);

        // ī�޶��� Y ������ ���� (���Ʒ� ȸ�� ȿ��)
        _currentOffsetY -= lookInput.y * _yRotationSpeed * Time.deltaTime;
        _currentOffsetY = Mathf.Clamp(_currentOffsetY, minOffsetY, maxOffsetY);

        // CinemachineTransposer�� followOffset Y �� ������Ʈ
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        Vector3 offset = transposer.m_FollowOffset;
        offset.y = _currentOffsetY;
        transposer.m_FollowOffset = offset;
    }
    #endregion

    #region SetFunc

    public void SetState(string key)
    {
        _fsm.SetState(key, this);
    }

    #endregion

}
