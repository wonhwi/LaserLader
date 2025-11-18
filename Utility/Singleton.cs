using UnityEngine;
using UnityEngine.Serialization;

public abstract class Singleton<T> : MonoBehaviour where T : class
{
  private static T _instance;

  [FormerlySerializedAs("_dontDestroy")]
  [SerializeField]
  private bool dontDestroy = true;

  public Singleton()
  {
    _instance = this as T;
  }

  public Singleton(bool bDontDestroy)
  {
    _instance = this as T;
    dontDestroy = bDontDestroy;
  }

  public static T Instance
  {
    get
    {
      if (_instance == null)
      {
        _instance = new GameObject("@" + typeof(T).Name.ToString(), typeof(T)).GetComponent<T>();
      }
      return _instance;
    }
  }
  private void Awake()
  {
    if (dontDestroy)
    {
      DontDestroyOnLoad(FindTransformRoot(gameObject.transform));
    }
    AwakeSetting();
  }

  protected virtual void AwakeSetting() { }

  // 최상위 루트 찾기
  protected Transform FindTransformRoot(Transform target)
  {
    Transform result = target;
    if (result.parent != null)
    {
      result = FindTransformRoot(result.parent);
    }
    return result;
  }
}