using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectBuilder : MonoBehaviour
{
    public GameObject objectPrefab; // 설치할 오브젝트 프리팹
    public LayerMask buildableSurfaceLayer; // 설치 가능한 표면 레이어
    public float maxBuildDistance = 25; // 플레이어가 설치할 수 있는 최대 거리

    private GameObject objectPreview; // 오브젝트 설치 미리보기
    private bool isBuilding = false; // 건설 모드 활성화 여부
    private Collider objectCollider; // 오브젝트 프리뷰의 Collider
    private Renderer[] objectRenderers; // 오브젝트 프리뷰의 모든 Renderer
    private List<GameObject> placedObjects = new List<GameObject>(); // 설치된 오브젝트 추적 리스트
    private bool canPlace = false; // 설치가능 여부

    private Transform cameraTransform; // 카메라 Transform


    private List<GameObject> objectClones_ = new List<GameObject>();
    public List<GameObject> objectClones => objectClones_;

    private void Start()
    {
        cameraTransform = Camera.main.transform; // 메인 카메라의 Transform 가져오기
    }

    // 건설 모드를 활성화/비활성화하는 입력 처리
    public void HandleBuildingInput(bool isBuildingModeActive, bool placeObject)
    {
        if (isBuildingModeActive != isBuilding)
        {
            isBuilding = !isBuilding;

            if (isBuilding)
            {
                StartBuildingMode();
            }
            else
            {
                StopBuildingMode();
            }
        }

        if (isBuilding)
        {
            UpdateObjectPreview();
            if (placeObject && canPlace)
            {
                PlaceObject();
            }
        }
    }

    // 건설 모드 시작
    private void StartBuildingMode()
    {
        if (objectPrefab == null) return;

        // 설치 미리보기 생성
        objectPreview = Instantiate(objectPrefab);
        objectPreview.layer = 0;

        objectCollider = objectPreview.GetComponentInChildren<Collider>(); // Collider 참조 가져오기
        objectCollider.isTrigger = true; // 충돌 방지

        objectRenderers = objectPreview.GetComponentsInChildren<Renderer>(); // 모든 Renderer 참조 가져오기

        if (objectPreview.GetComponent<Turret>() != null)
        {
            objectPreview.GetComponent<Turret>().enabled = false;
        }

        //objectPreview.GetComponent<NavMeshObstacle>().enabled = false;

        SetObjectPreviewMaterialAlpha(0.5f); // 반투명하게 설정
    }

    // 건설 모드 종료
    private void StopBuildingMode()
    {
        if (objectPreview != null)
        {
            Destroy(objectPreview);
        }
    }

    // 설치 미리보기 업데이트
    private void UpdateObjectPreview()
    {
        if (objectPreview == null) return;

        // 화면 중앙에서 광선을 발사
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // 레이를 그려서 시각적으로 확인
        Debug.DrawRay(ray.origin, ray.direction * maxBuildDistance, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, maxBuildDistance, buildableSurfaceLayer))
        {
            // 목표 위치와 회전을 설정
            objectPreview.transform.position = hit.point;

            // 프리뷰 오브젝트의 회전을 카메라의 앞방향과 표면 법선 방향을 기준으로 설정
            Vector3 forwardDirection = cameraTransform.forward; // 카메라가 바라보는 방향
            Vector3 upDirection = hit.normal; // 표면의 법선 방향

            objectPreview.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(forwardDirection, upDirection), upDirection);

            // 설치 가능 여부에 따라 색상 변경
            UpdateObjectPreviewColor(IsPlacementValid());

            canPlace = true; // 설치가능
        }
        else
        {
            // 레이가 맞지 않았을 때, 레이의 끝 부분에 프리뷰 위치 설정
            objectPreview.transform.position = ray.origin + ray.direction * maxBuildDistance;
            objectPreview.transform.rotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up); // 카메라 방향으로 설정
            UpdateObjectPreviewColor(false); // 설치 불가능한 상태로 색상 변경

            canPlace = false; // 설치불가
        }
    }

    // 오브젝트 설치
    private void PlaceObject()
    {
        if (objectPreview == null) return;

        // 충돌 검사: 설치하려는 위치에 다른 오브젝트가 있는지 확인
        if (IsPlacementValid())
        {
            Vector3 buildPosition = objectPreview.transform.position;
            Quaternion buildRotation = objectPreview.transform.rotation;

            // 오브젝트를 생성하고 리스트에 추가
            GameObject placedObject = Instantiate(objectPrefab, buildPosition, buildRotation);
            objectClones.Add(placedObject);

        }
        else
        {
            Debug.Log("Cannot place object here, another object is in the way.");
        }
    }

    // 설치 가능 여부 확인
    private bool IsPlacementValid()
    {
        Collider[] colliders = Physics.OverlapBox(
            objectCollider.bounds.center,
            objectCollider.bounds.extents,
            objectPreview.transform.rotation,
            ~buildableSurfaceLayer); // 충돌할 수 있는 레이어를 제외하여 검사

        foreach (var collider in colliders)
        {
            if (collider != objectCollider) // 자기 자신이 아닌 다른 오브젝트와 충돌 시
            {
                return false; // 설치 불가능
            }
        }

        return true; // 설치 가능
    }

    // 오브젝트 미리보기의 투명도 설정
    private void SetObjectPreviewMaterialAlpha(float alpha)
    {
        foreach (Renderer renderer in objectRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.SetFloat("_Mode", 3); // 렌더링 모드를 Transparent로 설정
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;

                Color color = material.color;
                color.a = alpha;
                material.color = color;

                // 변경된 렌더링 모드와 알파 설정을 적용
                material.SetOverrideTag("RenderType", "Transparent");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
        }
    }

    // 설치 가능 여부에 따라 프리뷰 색상 업데이트
    private void UpdateObjectPreviewColor(bool canPlace)
    {
        Color color = canPlace ? Color.green : Color.red; // 설치 가능 여부에 따른 색상 설정
        color.a = 0.5f; // 반투명도 유지

        foreach (Renderer renderer in objectRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.color = color; // 색상 적용
            }
        }
    }

    // 게임 재시작 시 설치된 오브젝트 제거
    public void ClearPlacedObjects()
    {
        foreach (GameObject obj in placedObjects)
        {
            Destroy(obj); // 오브젝트 제거
        }
        placedObjects.Clear(); // 리스트 초기화
    }

    public List<GameObject> GetObjectClones()
    {
        return objectClones;
    }
}
