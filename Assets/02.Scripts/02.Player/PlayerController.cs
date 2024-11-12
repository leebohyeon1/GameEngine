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
    [SerializeField] private float _moveSpeed; //이동 속도
    [SerializeField] private float _xRotationSpeed = 5f; // 회전 속도
    [SerializeField] private float _yRotationSpeed = 5f; // 회전 속도

    private float minOffsetY = 0.1f;  // 카메라의 최소 Y 오프셋
    private float maxOffsetY = 10f;  // 카메라의 최대 Y 오프셋
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

        // 초기 카메라 offset Y 설정
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
        // InputManager에서 입력 값을 받아오기
        Vector2 moveInput = _inputManager.MoveInput;

        // 카메라의 방향을 기준으로 이동 방향 설정
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;

        // 수직 벡터는 제거하여 평면에서 이동하도록 함
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // 카메라 방향을 기준으로 이동 방향 계산
        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // 플레이어 이동
        _characterController.Move(moveDirection * _moveSpeed * Time.deltaTime);

        // 블렌드 트리에 파라미터값 입력
        Animator.SetFloat("XDir", moveInput.x);
        Animator.SetFloat("YDir", moveInput.y);
    }

    public void Rotate()
    {
        Vector2 lookInput = _inputManager.LookInput;

        // 좌우 회전 적용
        _rotationY += lookInput.x * _xRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);

        // 카메라의 Y 오프셋 조정 (위아래 회전 효과)
        _currentOffsetY -= lookInput.y * _yRotationSpeed * Time.deltaTime;
        _currentOffsetY = Mathf.Clamp(_currentOffsetY, minOffsetY, maxOffsetY);

        // CinemachineTransposer의 followOffset Y 값 업데이트
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
