using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                return null;
            }

            if (_instance == null)
            {
                T[] instances = FindObjectsOfType<T>();

                if (instances.Length > 0)
                {
                    _instance = instances[0];

                    // 추가 인스턴스가 있다면 파괴
                    if (instances.Length > 1)
                    {
                        for (int i = 1; i < instances.Length; i++)
                        {
                            Destroy(instances[i].gameObject);
                        }
                        Debug.LogWarning("[Singleton] 여러 개의 인스턴스가 발견되어 추가 인스턴스를 파괴했습니다.");
                    }

                    DontDestroyOnLoad(_instance.gameObject);
                }
                else
                {
                    // 새로운 게임 오브젝트 생성 및 싱글톤 인스턴스 할당
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = "[Singleton] " + typeof(T).ToString();

                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        _applicationIsQuitting = true;
    }
}
