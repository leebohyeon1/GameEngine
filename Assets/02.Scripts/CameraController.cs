using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Cinemachine Virtual Camera 참조
    public float mouseSensitivity = 100f; // 마우스 감도 설정
    public Transform playerBody; // 플레이어 Transform

    private float xRotation = 0f; // 상하 회전을 위한 변수
    private CinemachineTransposer transposer; // 카메라의 Transposer 참조

    void Start()
    {
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

        }
        playerBody = GameObject.FindGameObjectWithTag("Player").transform;

        virtualCamera.LookAt = playerBody;
        virtualCamera.Follow = playerBody;

        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서를 잠금 상태로 설정

        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    void Update()
    {
        HandleCameraRotation();
    }

    private void HandleCameraRotation()
    {
        // 마우스 입력 받아오기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 현재 카메라 각도 업데이트
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -20f, 70f); // 상하 회전 각도를 -30도와 70도로 제한

        // 플레이어의 좌우 회전
        playerBody.Rotate(Vector3.up * mouseX);

        // 카메라의 상하 회전 적용
        Vector3 followOffset = transposer.m_FollowOffset;
        followOffset.y = Mathf.Tan(Mathf.Deg2Rad * xRotation) * Mathf.Abs(followOffset.z);
        transposer.m_FollowOffset = followOffset;
    }
}
