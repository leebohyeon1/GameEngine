using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera; // Cinemachine Virtual Camera 참조
    private float _mouseSensitivity = 100f; // 마우스 감도 설정
    [SerializeField]
    private Transform _playerBody; // 플레이어 Transform

    private float _xRotation = 0f; // 상하 회전을 위한 변수
    private CinemachineTransposer _transposer; // 카메라의 Transposer 참조

    void Start()
    {
        if (_virtualCamera == null)
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();

        }

        if(_playerBody == null)
        _playerBody = GameObject.FindGameObjectWithTag("Player").transform;

        _virtualCamera.LookAt = _playerBody;
        _virtualCamera.Follow = _playerBody;

        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서를 잠금 상태로 설정

        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    void Update()
    {
        HandleCameraRotation();
    }

    private void HandleCameraRotation()
    {
        // 마우스 입력 받아오기
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        // 현재 카메라 각도 업데이트
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -20f, 40f); // 상하 회전 각도를 -30도와 70도로 제한

        // 플레이어의 좌우 회전
        _playerBody.Rotate(Vector3.up * mouseX);

        // 카메라의 상하 회전 적용
        Vector3 followOffset = _transposer.m_FollowOffset;
        followOffset.y = Mathf.Tan(Mathf.Deg2Rad * _xRotation) * Mathf.Abs(followOffset.z);
        _transposer.m_FollowOffset = followOffset;
    }
}
