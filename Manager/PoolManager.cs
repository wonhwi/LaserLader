using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : Singleton<PoolManager>
{
  private Dictionary<Type, ObjectPool<PrefabUI>> prefabUISlotDict = new Dictionary<Type, ObjectPool<PrefabUI>>();

  private void Awake()
  {
    InitPooling<PrefabPositionSlot>(ResourcePath.PREFAB_UI_POSITION_SLOT, 10);
    InitPooling<PrefabPositionToggle>(ResourcePath.PREFAB_UI_POSITION_TOGGLE, 20);

    InitPooling<PrefabMapPosition>(ResourcePath.PREFAB_UI_MAP_POSITION_POINT, 20);
    InitPooling<PrefabMapRP>(ResourcePath.PREFAB_UI_MAP_RP_POINT, 20);
    InitPooling<PrefabMeasureSlot>(ResourcePath.PREFAB_UI_MEASURE_SLOT, 20);

  }

  #region UI Pooling
  /// <summary>
  /// UI Pooling
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="path">프리팹 경로</param>
  /// <param name="defaultCapacity">최소 생성 갯수</param>
  public void InitPooling<T>(string path, int defaultCapacity) where T : PrefabUI
  {
    Type type = typeof(T);

    if (!prefabUISlotDict.ContainsKey(type))
    {
      ObjectPool<PrefabUI> pool = new ObjectPool<PrefabUI>(
        createFunc: () => CreatePrefabSlot<T>(path),
        actionOnGet: OnGetPrefabUI,
        actionOnRelease: OnReleasePrefabUI,
        actionOnDestroy: OnDestroyPrefabUI,
        collectionCheck: false,
        defaultCapacity: defaultCapacity
        );

      prefabUISlotDict.Add(type, pool);
    }
  }

  protected T CreatePrefabSlot<T>(string path) where T : PrefabUI
  {
    GameObject prefab = ResourceManager.Instance.LoadResource<GameObject>(path);

    if (prefab == null)
    {
      Debug.LogError($"Prefab not found at path: {path}");
    }

    GameObject prefabUI = Instantiate(prefab, this.transform);

    prefabUI.gameObject.SetActive(false);

    return prefabUI.GetComponent<T>();
  }

  protected void OnGetPrefabUI<T>(T prefabUI) where T : PrefabUI
  {
    prefabUI.OnActive();

  }

  protected void OnReleasePrefabUI<T>(T prefabUI) where T : PrefabUI
  {
    prefabUI.transform.SetParent(this.transform);
    
    prefabUI.OnDeActive();
  }

  protected void OnDestroyPrefabUI<T>(T prefabUI) where T : PrefabUI
  {
    Destroy(prefabUI.gameObject);
  }

  public T GetPrefabUI<T>() where T : PrefabUI
  {
    return prefabUISlotDict[typeof(T)].Get() as T;
  }

  public void ClearReturnPool<T>(Transform tfParent) where T : PrefabUI
  {
    ObjectPool<PrefabUI> pool = prefabUISlotDict[typeof(T)];

    for (int i = tfParent.childCount - 1; i >= 0; i--)
    {
      if(tfParent.GetChild(i).TryGetComponent(out T prefabUI))
        pool.Release(prefabUI);
    }
  }

  #endregion
}
