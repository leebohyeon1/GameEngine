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

    [SerializeField] private GameObject _npcReplayerPrefab; // NPCReplayer ������ ����

    #region Stat
    [Space(20f)]
    [SerializeField] private float _moveSpeed; //�̵� �ӵ�
    [SerializeField] private float _jumpHeight = 2f; // ���� ����
    [SerializeField] private float _gravity = -9.81f; // �߷�

    [SerializeField] private float _xRotationSpeed = 5f; // ȸ�� �ӵ�
    [SerializeField] private float _yRotationSpeed = 5f; // ȸ�� �ӵ�

    private float minOffsetY = 0.1f;  // ī�޶��� �ּ� Y ������
    private float maxOffsetY = 10f;  // ī�޶��� �ִ� Y ������
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

        // �ʱ� ī�޶� offset Y ����
        _currentOffsetY = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;

        GameManager.Instance.SetPlayer(gameObject);
    }

    private void Update()
    {
        if(GameManager.Instance.GetCurGameState() == GameState.GameOver)
        {
            return;
        }

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
        _velocity.x = moveDirection.x;
        _velocity.z = moveDirection.z;

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
        var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        Vector3 offset = transposer.m_FollowOffset;
        offset.y = _currentOffsetY;
        transposer.m_FollowOffset = offset;
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            // ���� �̵� ������ ���� ���Ϳ� �߰�
            Vector2 moveInput = _inputManager.MoveInput;
            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            // ���� �̵� ���� ����
            Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

            // �̵� ����� ���� ���̸� �Բ� �ݿ�
            _velocity = moveDirection * _moveSpeed + Vector3.up * Mathf.Sqrt(_jumpHeight * -2f * _gravity);

            SetGravity();
            SetState("JumpUp");

        }
    }

    private void CheckGround()
    {
        // ���鿡 ��� �ִ��� Ȯ��
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
        // �߷� ����
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

    public void Die(Vector3 position)
    {
        _allRecordedActions.Add(ActionRecorder.GetRecordedActions());
        ActionRecorder.StopRecording();

        _characterController.Move(position - transform.position);

        ResetPlayer();
    }

    public void ResetPlayer()
    {
        ActionRecorder.ResetRecording();
        ActionRecorder.StartRecording();

        foreach (GameObject clone in _actionClones) // �н� ����
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }
        _actionClones.Clear();

        foreach (GameObject clone in ObjectBuilder.ObjectClones) //��ġ�� ������Ʈ ����
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }

        ObjectBuilder.ObjectClones.Clear();


        for (int i = 0; i < _allRecordedActions.Count; i++)
        {
            CreateNPC(_allRecordedActions[i]);// �н� ����
        }
    }

    private void CreateNPC(List<PlayerActionRecorder.PlayerAction> actions)
    {
        GameObject npcInstance = Instantiate(_npcReplayerPrefab, transform.position, Quaternion.identity);
        _actionClones.Add(npcInstance.gameObject); // Ŭ�� ����Ʈ�� ������Ʈ �߰�
        CloneReplayer clone = npcInstance.GetComponent<CloneReplayer>();
        clone.SetEngineer(ObjectBuilder);         // ������ ����
        clone.StartReplay(actions);         // �÷��̾� �ൿ ���� �ѱ��
    }
}
