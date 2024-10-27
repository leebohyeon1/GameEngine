using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController: MonoBehaviour
{
    public float speed = 5f;  // 기본 이동 속도
    public float sprintSpeed = 8f;  // 달리기 속도
    public float slideSpeed = 12f;  // 슬라이딩 속도
    public float slideDuration = 1f;  // 슬라이딩 지속 시간
    public float jumpHeight = 2f;  // 점프 높이
    public float gravity = -9.81f;  // 중력 가속도
    public Transform cameraTransform;  // 메인 카메라의 Transform

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isSliding = false;
    private float originalHeight;  // 캐릭터의 원래 높이
    public float slideHeight = 0.5f;  // 슬라이딩 시 캐릭터 높이

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;  // 원래 콜라이더 높이 저장
    }

    void Update()
    {
 
    }

    IEnumerator Slide()
    {
        isSliding = true;

        Vector3 slideDirection = controller.velocity.normalized;  // 현재 이동 방향 유지
        float startTime = Time.time;

        while (Time.time < startTime + slideDuration)
        {
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);  // 빠르게 이동
            yield return null;  // 다음 프레임까지 대기
        }

        controller.height = originalHeight;  // 원래 높이로 복원
        isSliding = false;
    }
}
