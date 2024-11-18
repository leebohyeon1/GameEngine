using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ObjectBuilder : MonoBehaviour
{
    [SerializeField] private GameObject _objectPrefab; // 설치할 오브젝트 프리팹
    [SerializeField] private LayerMask _buildableSurfaceLayer; // 설치 가능한 표면 레이어
    [SerializeField] private float _maxBuildDistance = 25; // 플레이어가 설치할 수 있는 최대 거리

    private GameObject _objectPreview; // 오브젝트 설치 미리보기
    private bool _isBuilding = false; // 건설 모드 활성화 여부
    private Collider[] _objectCollider; // 오브젝트 프리뷰의 Collider
    private Renderer[] _objectRenderers; // 오브젝트 프리뷰의 모든 Renderer
    private List<GameObject> _placedObjects = new List<GameObject>(); // 설치된 오브젝트 추적 리스트
    private bool _canPlace = false; // 설치가능 여부
    private int _price = 0;

    private Transform _cameraTransform; // 카메라 Transform
    private PlayerController _playerController;

    private List<GameObject> _objectClones_ = new List<GameObject>();
    public List<GameObject> ObjectClones => _objectClones_;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();

        _cameraTransform = Camera.main.transform; // 메인 카메라의 Transform 가져오기
    }

    // 건설 모드를 활성화/비활성화하는 입력 처리
    public void HandleBuildingInput(bool isBuildingModeActive, bool placeObject)
    {
        if (isBuildingModeActive != _isBuilding)
        {
            _isBuilding = !_isBuilding;

            if (_isBuilding)
            {
                StartBuildingMode();
            }
            else
            {
                StopBuildingMode();
            }
        }

        if (_isBuilding)
        {
            UpdateObjectPreview();
            if (placeObject && _canPlace)
            {
                PlaceObject();
            }
        }
    }

    // 건설 모드 시작
    private void StartBuildingMode()
    {
        if (_objectPrefab == null) return;

        // 설치 미리보기 생성
        _objectPreview = Instantiate(_objectPrefab);
        _objectPreview.layer = 0;

        _objectCollider = _objectPreview.GetComponentsInChildren<Collider>(); // Collider 참조 가져오기
       
        foreach(Collider collider in _objectCollider)
        {
            collider.isTrigger = true; // 충돌 방지
        }
        
        _objectRenderers = _objectPreview.GetComponentsInChildren<Renderer>(); // 모든 Renderer 참조 가져오기

        if (_objectPreview.GetComponent<BuildObject>() != null)
        {
            _objectPreview.GetComponent<BuildObject>().enabled = false;
        }

        _objectPreview.GetComponentInChildren<NavMeshObstacle>().enabled = false;

        SetObjectPreviewMaterialAlpha(0.5f); // 반투명하게 설정
    }

    // 건설 모드 종료
    private void StopBuildingMode()
    {
        if (_objectPreview != null)
        {
            Destroy(_objectPreview);
        }
    }

    // 설치 미리보기 업데이트
    private void UpdateObjectPreview()
    {
        if (_objectPreview == null) return;

        // 화면 중앙에서 광선을 발사
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // 레이를 그려서 시각적으로 확인
        Debug.DrawRay(ray.origin, ray.direction * _maxBuildDistance, Color.green);

        if (GameManager.Instance.GetMoney() >= _price &&
            Physics.Raycast(ray, out RaycastHit hit, _maxBuildDistance, _buildableSurfaceLayer))
        {
            // 목표 위치와 회전을 설정
            _objectPreview.transform.position = hit.point;

            // 프리뷰 오브젝트의 회전을 카메라의 앞방향과 표면 법선 방향을 기준으로 설정
            Vector3 forwardDirection = _cameraTransform.forward; // 카메라가 바라보는 방향
            Vector3 upDirection = hit.normal; // 표면의 법선 방향

            _objectPreview.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(forwardDirection, upDirection), upDirection);

            // 설치 가능 여부에 따라 색상 변경
            UpdateObjectPreviewColor(IsPlacementValid());

            _canPlace = true; // 설치가능
        }
        else
        {
            // 레이가 맞지 않았을 때, 레이의 끝 부분에 프리뷰 위치 설정
            _objectPreview.transform.position = ray.origin + ray.direction * _maxBuildDistance;
            _objectPreview.transform.rotation = Quaternion.LookRotation(_cameraTransform.forward, Vector3.up); // 카메라 방향으로 설정
            UpdateObjectPreviewColor(false); // 설치 불가능한 상태로 색상 변경

            _canPlace = false; // 설치불가
        }
    }

    // 오브젝트 설치
    private void PlaceObject()
    {
        if (_objectPreview == null) return;

        // 충돌 검사: 설치하려는 위치에 다른 오브젝트가 있는지 확인
        if (IsPlacementValid())
        {
            Vector3 buildPosition = _objectPreview.transform.position;
            Quaternion buildRotation = _objectPreview.transform.rotation;

            // 오브젝트를 생성하고 리스트에 추가
            GameObject placedObject = Instantiate(_objectPrefab, buildPosition, buildRotation);
            ObjectClones.Add(placedObject);

            _playerController.ActionRecorder.RecordPlaceObject(_objectPrefab, buildPosition, buildRotation);

            GameManager.Instance.DecreaseMoney(_price);
        }
        else
        {
            Debug.Log("Cannot place object here, another object is in the way.");
        }
    }

    // 설치 가능 여부 확인
    private bool IsPlacementValid()
    {
        // 모든 콜라이더의 중심과 크기를 기준으로 하나의 검사 영역을 생성합니다.
        Bounds combinedBounds = new Bounds(_objectCollider[0].bounds.center, Vector3.zero);

        // 모든 콜라이더의 경계 상자를 병합하여 단일 Bounds로 만듭니다.
        foreach (Collider coll in _objectCollider)
        {
            combinedBounds.Encapsulate(coll.bounds);
        }

        // 병합된 Bounds를 사용하여 중복 충돌 체크를 수행합니다.
        Collider[] colliders = Physics.OverlapBox(
            combinedBounds.center,
            combinedBounds.extents,
            Quaternion.identity,
            ~_buildableSurfaceLayer); // 충돌할 수 있는 레이어를 제외하여 검사

        foreach (var collider in colliders)
        {
            // 자신의 콜라이더가 아닌 경우에만 검사합니다.
            if (System.Array.IndexOf(_objectCollider, collider) == -1)
            {
                return false; // 설치 불가능
            }
        }

        return true; // 설치 가능
    }

    // 오브젝트 미리보기의 투명도 설정
    private void SetObjectPreviewMaterialAlpha(float alpha)
    {
        foreach (Renderer renderer in _objectRenderers)
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

        foreach (Renderer renderer in _objectRenderers)
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
        foreach (GameObject obj in _placedObjects)
        {
            Destroy(obj); // 오브젝트 제거
        }
        _placedObjects.Clear(); // 리스트 초기화
    }


    public void SetObject(GameObject obj, int price)
    {
        _objectPrefab = obj;
        _price = price;
    }
}
