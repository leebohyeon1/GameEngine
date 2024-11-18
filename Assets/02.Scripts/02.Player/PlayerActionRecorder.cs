using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionRecorder : MonoBehaviour
{
    public enum ActionType
    {
        Move,
        Rotate,
        Jump,
        Fall,
        Landing,
        PlaceObject // 오브젝트 설치 액션 추가
    }

    [System.Serializable]
    public class PlayerAction
    {
        public ActionType Type;        // 행동의 유형
        public Vector3 Position;       // 플레이어의 위치 또는 오브젝트 설치 위치
        public Quaternion Rotation;    // 플레이어의 회전 또는 오브젝트 설치 회전
        public string FunctionName;    // 호출할 함수 이름
        public GameObject Prefab;      // 설치할 오브젝트 프리팹
        public Dictionary<string, AnimatorParameter> AnimatorParameters; // 애니메이터 파라미터 값
        public float TimeStamp;        // 행동이 기록된 시간
    }

    // 애니메이터 파라미터 구조체 정의
    [System.Serializable]
    public struct AnimatorParameter
    {
        public AnimatorControllerParameterType type;
        public object value;
    }

    private Animator animator; // 플레이어 애니메이션

    private List<PlayerAction> actions = new List<PlayerAction>(); // 녹화된 행동 리스트
    private bool isRecording = false;  // 현재 녹화 중인지 여부
    private float startTime; // 녹화 시작 시간

    void Start()
    {
        animator = GetComponent<Animator>();

        StartRecording();
    }

    void FixedUpdate()
    {
        if (isRecording)
        {
            // 이동을 자동으로 기록
            RecordAction(ActionType.Move, transform.position, transform.rotation);
        }
    }

    // 녹화를 시작하는 메서드
    public void StartRecording()
    {
        isRecording = true;
        startTime = Time.time;
    }

    // 녹화를 중지하는 메서드
    public void StopRecording()
    {
        isRecording = false;
    }

    // 녹화 데이터를 초기화하는 메서드
    public void ResetRecording()
    {
        actions.Clear();
        isRecording = false;
    }

    // 자동 녹화 메서드
    private void RecordAction(ActionType type, Vector3 position, Quaternion rotation, string functionName = null, GameObject prefab = null)
    {
        var animatorParameters = new Dictionary<string, AnimatorParameter>();

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            AnimatorParameter paramData = new AnimatorParameter { type = parameter.type };

            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    paramData.value = animator.GetFloat(parameter.name);
                    break;
                case AnimatorControllerParameterType.Int:
                    paramData.value = animator.GetInteger(parameter.name);
                    break;
                case AnimatorControllerParameterType.Bool:
                    paramData.value = animator.GetBool(parameter.name);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    paramData.value = false; // Trigger는 이벤트 기반이므로 상태를 저장하지 않음
                    break;
            }

            animatorParameters[parameter.name] = paramData;
        }

        var currentAction = new PlayerAction
        {
            Type = type,
            Position = position,
            Rotation = rotation,
            FunctionName = functionName,
            AnimatorParameters = animatorParameters,
            Prefab = prefab,
            TimeStamp = Time.time - startTime

        };

        actions.Add(currentAction);
    }

    // 오브젝트 설치 녹화 메서드
    public void RecordPlaceObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        RecordAction(ActionType.PlaceObject, position, rotation, null, prefab);
    }

    public void RecordJump(Vector3 position, Quaternion rotation)
    {
        RecordAction(ActionType.Jump, position, rotation);
    }

    public void RecordFall(Vector3 position, Quaternion rotation)
    {
        RecordAction(ActionType.Fall, position, rotation);
    }

    public void RecordLanding(Vector3 position, Quaternion rotation)
    {
        RecordAction(ActionType.Landing, position, rotation);
    }
    // 녹화된 행동 리스트를 반환하는 메서드
    public List<PlayerAction> GetRecordedActions()
    {
        return new List<PlayerAction>(actions);
    }
}
