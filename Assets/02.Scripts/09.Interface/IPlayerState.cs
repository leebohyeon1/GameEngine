using UnityEngine;

// 상태 인터페이스 정의
public interface IPlayerState
{
    void Enter(Player player);
    void UpdateState(Player player);
    void FixedUpdateState(Player player);
    void Exit(Player player);
}
