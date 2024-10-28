using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController _characterController;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {

    }

    //IEnumerator Slide()
    //{
    //    isSliding = true;

    //    Vector3 slideDirection = controller.velocity.normalized;  // 현재 이동 방향 유지
    //    float startTime = Time.time;

    //    while (Time.time < startTime + slideDuration)
    //    {
    //        controller.Move(slideDirection * slideSpeed * Time.deltaTime);  // 빠르게 이동
    //        yield return null;  // 다음 프레임까지 대기
    //    }

    //    controller.height = originalHeight;  // 원래 높이로 복원
    //    isSliding = false;
    //}
}
