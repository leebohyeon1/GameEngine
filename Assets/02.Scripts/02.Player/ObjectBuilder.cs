using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ObjectBuilder : MonoBehaviour
{
    [SerializeField] private GameObject _objectPrefab; // ��ġ�� ������Ʈ ������
    [SerializeField] private LayerMask _buildableSurfaceLayer; // ��ġ ������ ǥ�� ���̾�
    [SerializeField] private float _maxBuildDistance = 25; // �÷��̾ ��ġ�� �� �ִ� �ִ� �Ÿ�

    private GameObject _objectPreview; // ������Ʈ ��ġ �̸�����
    private bool _isBuilding = false; // �Ǽ� ��� Ȱ��ȭ ����
    private Collider[] _objectCollider; // ������Ʈ �������� Collider
    private Renderer[] _objectRenderers; // ������Ʈ �������� ��� Renderer
    private List<GameObject> _placedObjects = new List<GameObject>(); // ��ġ�� ������Ʈ ���� ����Ʈ
    private bool _canPlace = false; // ��ġ���� ����

    private Transform _cameraTransform; // ī�޶� Transform


    private List<GameObject> _objectClones_ = new List<GameObject>();
    public List<GameObject> ObjectClones => _objectClones_;

    private void Start()
    {
        _cameraTransform = Camera.main.transform; // ���� ī�޶��� Transform ��������
    }

    // �Ǽ� ��带 Ȱ��ȭ/��Ȱ��ȭ�ϴ� �Է� ó��
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

    // �Ǽ� ��� ����
    private void StartBuildingMode()
    {
        if (_objectPrefab == null) return;

        // ��ġ �̸����� ����
        _objectPreview = Instantiate(_objectPrefab);
        _objectPreview.layer = 0;

        _objectCollider = _objectPreview.GetComponentsInChildren<Collider>(); // Collider ���� ��������
       
        foreach(Collider collider in _objectCollider)
        {
            collider.isTrigger = true; // �浹 ����
        }
        
        _objectRenderers = _objectPreview.GetComponentsInChildren<Renderer>(); // ��� Renderer ���� ��������

        if (_objectPreview.GetComponent<Turret>() != null)
        {
            _objectPreview.GetComponent<Turret>().enabled = false;
        }

        //objectPreview.GetComponent<NavMeshObstacle>().enabled = false;

        SetObjectPreviewMaterialAlpha(0.5f); // �������ϰ� ����
    }

    // �Ǽ� ��� ����
    private void StopBuildingMode()
    {
        if (_objectPreview != null)
        {
            Destroy(_objectPreview);
        }
    }

    // ��ġ �̸����� ������Ʈ
    private void UpdateObjectPreview()
    {
        if (_objectPreview == null) return;

        // ȭ�� �߾ӿ��� ������ �߻�
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // ���̸� �׷��� �ð������� Ȯ��
        Debug.DrawRay(ray.origin, ray.direction * _maxBuildDistance, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, _maxBuildDistance, _buildableSurfaceLayer))
        {
            // ��ǥ ��ġ�� ȸ���� ����
            _objectPreview.transform.position = hit.point;

            // ������ ������Ʈ�� ȸ���� ī�޶��� �չ���� ǥ�� ���� ������ �������� ����
            Vector3 forwardDirection = _cameraTransform.forward; // ī�޶� �ٶ󺸴� ����
            Vector3 upDirection = hit.normal; // ǥ���� ���� ����

            _objectPreview.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(forwardDirection, upDirection), upDirection);

            // ��ġ ���� ���ο� ���� ���� ����
            UpdateObjectPreviewColor(IsPlacementValid());

            _canPlace = true; // ��ġ����
        }
        else
        {
            // ���̰� ���� �ʾ��� ��, ������ �� �κп� ������ ��ġ ����
            _objectPreview.transform.position = ray.origin + ray.direction * _maxBuildDistance;
            _objectPreview.transform.rotation = Quaternion.LookRotation(_cameraTransform.forward, Vector3.up); // ī�޶� �������� ����
            UpdateObjectPreviewColor(false); // ��ġ �Ұ����� ���·� ���� ����

            _canPlace = false; // ��ġ�Ұ�
        }
    }

    // ������Ʈ ��ġ
    private void PlaceObject()
    {
        if (_objectPreview == null) return;

        // �浹 �˻�: ��ġ�Ϸ��� ��ġ�� �ٸ� ������Ʈ�� �ִ��� Ȯ��
        if (IsPlacementValid())
        {
            Vector3 buildPosition = _objectPreview.transform.position;
            Quaternion buildRotation = _objectPreview.transform.rotation;

            // ������Ʈ�� �����ϰ� ����Ʈ�� �߰�
            GameObject placedObject = Instantiate(_objectPrefab, buildPosition, buildRotation);
            ObjectClones.Add(placedObject);

        }
        else
        {
            Debug.Log("Cannot place object here, another object is in the way.");
        }
    }

    // ��ġ ���� ���� Ȯ��
    private bool IsPlacementValid()
    {
        // ��� �ݶ��̴��� �߽ɰ� ũ�⸦ �������� �ϳ��� �˻� ������ �����մϴ�.
        Bounds combinedBounds = new Bounds(_objectCollider[0].bounds.center, Vector3.zero);

        // ��� �ݶ��̴��� ��� ���ڸ� �����Ͽ� ���� Bounds�� ����ϴ�.
        foreach (Collider coll in _objectCollider)
        {
            combinedBounds.Encapsulate(coll.bounds);
        }

        // ���յ� Bounds�� ����Ͽ� �ߺ� �浹 üũ�� �����մϴ�.
        Collider[] colliders = Physics.OverlapBox(
            combinedBounds.center,
            combinedBounds.extents,
            Quaternion.identity,
            ~_buildableSurfaceLayer); // �浹�� �� �ִ� ���̾ �����Ͽ� �˻�

        foreach (var collider in colliders)
        {
            // �ڽ��� �ݶ��̴��� �ƴ� ��쿡�� �˻��մϴ�.
            if (System.Array.IndexOf(_objectCollider, collider) == -1)
            {
                return false; // ��ġ �Ұ���
            }
        }

        return true; // ��ġ ����
    }

    // ������Ʈ �̸������� ���� ����
    private void SetObjectPreviewMaterialAlpha(float alpha)
    {
        foreach (Renderer renderer in _objectRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.SetFloat("_Mode", 3); // ������ ��带 Transparent�� ����
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

                // ����� ������ ���� ���� ������ ����
                material.SetOverrideTag("RenderType", "Transparent");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
        }
    }

    // ��ġ ���� ���ο� ���� ������ ���� ������Ʈ
    private void UpdateObjectPreviewColor(bool canPlace)
    {
        Color color = canPlace ? Color.green : Color.red; // ��ġ ���� ���ο� ���� ���� ����
        color.a = 0.5f; // ������ ����

        foreach (Renderer renderer in _objectRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.color = color; // ���� ����
            }
        }
    }

    // ���� ����� �� ��ġ�� ������Ʈ ����
    public void ClearPlacedObjects()
    {
        foreach (GameObject obj in _placedObjects)
        {
            Destroy(obj); // ������Ʈ ����
        }
        _placedObjects.Clear(); // ����Ʈ �ʱ�ȭ
    }

    public List<GameObject> GetObjectClones()
    {
        return ObjectClones;
    }

    public void SetObject(GameObject obj)
    {
        _objectPrefab = obj;
    }
}
