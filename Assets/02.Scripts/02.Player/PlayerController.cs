using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputManager _inputManager;
    private CharacterController _characterController;
    public ObjectBuilder ObjectBuilder { get; private set; }
    public PlayerActionRecorder ActionRecorder { get; private set; }

    private FSMBase _fsm;

    private Transform _cameraTransform;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    public Animator Animator { get; private set; }


    private List<List<PlayerActionRecorder.PlayerAction>> _allRecordedActions = new List<List<PlayerActionRecorder.PlayerAction>>();
    private List<GameObject> _actionClones = new List<GameObject>();

    [SerializeField] private GameObject _npcReplayerPrefab; // NPCReplayer 프리팹 참조

    #region Stat
    [Space(20f)]
    [SerializeField] private float _moveSpeed; //이동 속도
    [SerializeField] private float _jumpHeight = 2f; // 점프 높이
    [SerializeField] private float _gravity = -9.81f; // 중력

    [SerializeField] private float _xRotationSpeed = 5f; // 회전 속도
    [SerializeField] private float _yRotationSpeed = 5f; // 회전 속도

    private float minOffsetY = 0.1f;  // 카메라의 최소 Y 오프셋
    private float maxOffsetY = 10f;  // 카메라의 최대 Y 오프셋
    #endregion

    private float _rotationY;
    private float _currentOffsetY;
    private Vector3 _velocity;
    private bool _isGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        ObjectBuilder = GetComponent<ObjectBuilder>();
        ActionRecorder = GetComponent<PlayerActionRecorder>();
        Animator = GetComponent<Animator>();

        SetFSM();
    }

    private void Start()
    {
        _inputManager = InputManager.Instance;
        _cameraTransform = Camera.main.transform;

        // 초기 카메라 offset Y 설정
        _currentOffsetY = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;


    }

    private void Update()
    {
        CheckGround();
        _characterController.Move(_velocity * _moveSpeed * Time.deltaTime);

        _fsm.Update(this);

    }

    private void FixedUpdate()
    {
        _fsm.FixedUpdate(this);
    }

    #region InitialFunc

    private void SetFSM()
    {
        _fsm = new FSMBase();
        _fsm.AddState("Idle", new IdleState(_fsm));
        _fsm.AddState("Move", new MoveState(_fsm));
        _fsm.AddState("JumpUp", new JumpUpState(_fsm));
        _fsm.AddState("Fall", new FallState(_fsm));
        _fsm.AddState("JumpDown", new JumpDownState(_fsm));
        _fsm.AddState("Place", new PlaceState(_fsm));

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
        _velocity.x = moveDirection.x;
        _velocity.z = moveDirection.z;

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
        var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        Vector3 offset = transposer.m_FollowOffset;
        offset.y = _currentOffsetY;
        transposer.m_FollowOffset = offset;
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            // 현재 이동 방향을 점프 벡터에 추가
            Vector2 moveInput = _inputManager.MoveInput;
            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            // 현재 이동 방향 설정
            Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

            // 이동 방향과 점프 높이를 함께 반영
            _velocity = moveDirection * _moveSpeed + Vector3.up * Mathf.Sqrt(_jumpHeight * -2f * _gravity);

            SetGravity();
            SetState("JumpUp");

        }
    }

    private void CheckGround()
    {
        // 지면에 닿아 있는지 확인
        _isGrounded = _characterController.isGrounded;
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -1f;
        }
        else if(!_isGrounded)
        {
            SetGravity();
            if (_velocity.y < -1.5f && !_fsm.IsSameState("Fall"))
            {
                SetState("Fall");
            }
        }
        
    }

    #endregion

    #region SetFunc

    public void SetState(string key)
    {
        _fsm.SetState(key, this);
    }

    public void SetState(FSMState fsmState)
    {
        _fsm.SetState(fsmState, this);
    }

    public void SetGravity()
    {
        // 중력 적용
        _velocity.y += _gravity * Time.deltaTime;
    }

    public void SetZeroVelocity()
    {
        _velocity = Vector3.zero;
    }

    #endregion

    #region GetFunc
    public bool GetIsGround() => _isGrounded;

    public FSMState GetPreState() => _fsm.GetPreState();
    #endregion

    public void Die()
    {
        _allRecordedActions.Add(ActionRecorder.GetRecordedActions());
        ActionRecorder.StopRecording();

        ResetPlayer();
    }

    public void ResetPlayer()
    {
        ActionRecorder.ResetRecording();
        ActionRecorder.StartRecording();

        foreach (GameObject clone in _actionClones) // 분신 제거
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }
        _actionClones.Clear();

        foreach (GameObject clone in ObjectBuilder.ObjectClones) //설치한 오브젝트 제거
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }

        ObjectBuilder.ObjectClones.Clear();


        for (int i = 0; i < _allRecordedActions.Count; i++)
        {
            CreateNPC(_allRecordedActions[i]);// 분신 생성
        }
    }

    private void CreateNPC(List<PlayerActionRecorder.PlayerAction> actions)
    {
        GameObject npcInstance = Instantiate(_npcReplayerPrefab, transform.position, Quaternion.identity);
        _actionClones.Add(npcInstance.gameObject); // 클론 리스트에 오브젝트 추가
        CloneReplayer clone = npcInstance.GetComponent<CloneReplayer>();
        clone.SetEngineer(ObjectBuilder);         // 생성자 설정
        clone.StartReplay(actions);         // 플레이어 행동 정보 넘기기
    }
}
