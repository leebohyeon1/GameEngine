using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerActionRecorder;

public class CloneReplayer : MonoBehaviour
{
    private ObjectBuilder Builder;
    private Animator animator;

    private List<PlayerAction> recordedActions;
    private int currentActionIndex = 0;
    private bool isReplaying = false;
    private float replayStartTime;

    [SerializeField] private Material[] cloneMaterials;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (isReplaying)
        {
            ReplayAction();
        }
    }

    // 리플레이를 시작하는 메서드
    public void StartReplay(List<PlayerAction> actions)
    {
        if (actions == null || actions.Count == 0) return;

        recordedActions = actions;
        currentActionIndex = 0;
        isReplaying = true;
        replayStartTime = Time.time;
    }

    // 행동을 재생하는 메서드
    private void ReplayAction()
    {
        if (currentActionIndex >= recordedActions.Count)
        {
            isReplaying = false;
            Destroy(gameObject);
            return;
        }

        // 현재 행동 가져오기
        var action = recordedActions[currentActionIndex];

        // 현재 시간과 재생 시작 시간의 차이 계산
        float elapsedTime = Time.time - replayStartTime;

        // 만약 현재 행동의 타임스탬프가 경과된 시간보다 작거나 같으면 행동 재생
        if (action.TimeStamp <= elapsedTime)
        {
            ApplyAnimatorParameters(action.AnimatorParameters);

            switch (action.Type)
            {
                case ActionType.Move:
                    transform.position = action.Position;
                    transform.rotation = action.Rotation;
                    break;
                case ActionType.Jump:
                    transform.position = action.Position;
                    transform.rotation = action.Rotation;
                    animator.SetTrigger("Jump");
                    break;
                case ActionType.Fall:
                    transform.position = action.Position;
                    transform.rotation = action.Rotation;
                    animator.SetTrigger("Fall");
                    break;
                case ActionType.Landing:
                    transform.position = action.Position;
                    transform.rotation = action.Rotation;
                    animator.SetTrigger("Landing");
                    break;
                case ActionType.PlaceObject:
                    GameObject placedObject = Instantiate(action.Prefab, action.Position, action.Rotation);
                    Collider[] colliders = placedObject.GetComponentsInChildren<Collider>(); // 클론이 소환한 오브젝트는 트리거 
                    foreach (Collider collider in colliders)
                    {
                        collider.isTrigger = true;
                    }
                    changebjectPreviewColor(placedObject);
                    Builder.ObjectClones.Add(placedObject);
                    animator.SetTrigger("Place");
                    break;
  

            }

            // 다음 행동으로 이동
            currentActionIndex++;
        }
    }

    private void ApplyAnimatorParameters(Dictionary<string, PlayerActionRecorder.AnimatorParameter> animatorParameters)
    {
        foreach (var parameter in animatorParameters)
        {
            switch (parameter.Value.type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(parameter.Key, (float)parameter.Value.value);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(parameter.Key, (int)parameter.Value.value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(parameter.Key, (bool)parameter.Value.value);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    // Trigger 상태는 직접 저장하지 않지만, 재생 시 필요한 경우에만 사용
                    break;
            }
        }
    }

    // 오브젝트 메테리얼 변경
    private void changebjectPreviewColor(GameObject placeObject)
    {
        Renderer[] objectRenderers = placeObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in objectRenderers)
        {
            renderer.materials = cloneMaterials; // 메테리얼 교체
        }
    }

    public void SetEngineer(ObjectBuilder builder)
    {
        Builder = builder;
    }


}
