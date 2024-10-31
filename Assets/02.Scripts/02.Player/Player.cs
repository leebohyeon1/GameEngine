using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController _characterController;
    private Camera _cam;
    public Animator PlayerAnimator { get; private set; }

    private IPlayerState _curState;

    #region Stat
    [SerializeField]
    private float _walkSpeed = 2f;
    [SerializeField]
    private float _runSpeed = 5f;

    [Space(20f), SerializeField]
    private float _slideDuration;
    [SerializeField]
    private float _slideSpeed;
    private float _originalHeight;
    #endregion

    #region State
    private bool _canControl = true;
    #endregion

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서를 잠금 상태로 설정
        _characterController = GetComponent<CharacterController>();
        PlayerAnimator = GetComponent<Animator>();
        _cam = Camera.main;

        ChangeState(new IPlayerIdleState());
        _canControl = true;
        _originalHeight = _characterController.height;
    }

    void Update()
    {
        if (_curState == null || !_canControl)
        {
            return;
        }
      
        _curState.UpdateState(this);
        
    }

    private void FixedUpdate()
    {
        if (_curState == null || !_canControl)
        {
            return;
        }

        _curState.FixedUpdateState(this);
    }

    public void ChangeState(IPlayerState playerState)
    {
        if(_curState !=null)
        {
            _curState.Exit(this);
        }
        _curState = playerState;
        _curState.Enter(this);
    }

    public void SetControl()
    {
        _canControl = !_canControl;
    }

    public void Move()
    {
        float moveSpeed = InputManager.Instance.RunInput ? _runSpeed : _walkSpeed;

        Vector3 lookForward = new Vector3(_cam.transform.forward.x, 0, _cam.transform.forward.z).normalized;
        Vector3 lookRIght = new Vector3(_cam.transform.right.x, 0, _cam.transform.right.z).normalized;

        Vector3 moveDir = InputManager.Instance.MoveInput.x * lookRIght
            + InputManager.Instance.MoveInput.y * lookForward;

        if (moveDir.magnitude != 0)
        {
            Quaternion viewRot = Quaternion.LookRotation(moveDir.normalized);

            transform.rotation = Quaternion.Lerp(transform.rotation, viewRot, Time.deltaTime * 20);

            _characterController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
    }

    public IEnumerator Slide()
    {
        Vector3 slideDirection = _characterController.velocity.normalized;  // 현재 이동 방향 유지
        _characterController.height = _originalHeight / 2;
        _characterController.center = _characterController.center / 2;

        float startTime = Time.time;

        while (Time.time < startTime + _slideDuration)
        {
            _characterController.Move(slideDirection * _slideSpeed * Time.deltaTime);  // 빠르게 이동
            yield return null;  // 다음 프레임까지 대기
        }

        _characterController.height = _originalHeight;  // 원래 높이로 복원
    }
}
