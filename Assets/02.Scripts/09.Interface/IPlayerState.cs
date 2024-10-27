using UnityEngine;

// 상태 인터페이스 정의
public interface IPlayerState
{
    void Enter(PlayerController player);
    void UpdateState(PlayerController player);
    void Exit(PlayerController player);
}
