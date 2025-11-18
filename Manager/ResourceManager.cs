using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
  [SerializeField] private Dictionary<string, Object> cachedResources = new Dictionary<string, Object>();

  /// <summary>
  /// 가장 기본적인 Sprite Load 함수
  /// </summary>
  /// <param name="path"></param>
  /// <param name="resourceName"></param>
  /// <returns></returns>
  public Sprite LoadSprite(string path, string resourceName)
  {
    return LoadResource<Sprite>(GetResourcePath(path, resourceName));
  }



  private string GetResourcePath(string path, string resourceName)
    => string.Format("{0}{1}", path, resourceName);

  /// <summary>
  /// 리소스 형식인지, 에셋번들인지, Addressable인지에 따라 코드 수정 필요 지금은 단순 Resource를 불러오는 방식
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="resourceName"></param>
  /// <returns></returns>
  public T LoadResource<T>(string resourceName) where T : Object
  {
    T obj = null;

    ///리소스가 캐싱 되어 있다면
    if (cachedResources.ContainsKey(resourceName))
    {
      obj = cachedResources[resourceName] as T;
    }
    ///
    else
    {
      obj = Resources.Load<T>(resourceName);

      if (obj != null)
      {
        cachedResources.Add(resourceName, obj);
      }
    }

    return obj;
  }


}

public class ResourcePath
{
  #region UI
  public const string PATH_UI = "UI/"; 
  public const string PATH_UI_NOTIFICATION_ICON = "UI/Notification/"; //원버튼

  public const string PREFAB_UI_POSITION_SLOT = "Prefab/Instantiate/PrefabPositionSlot";
  public const string PREFAB_UI_POSITION_TOGGLE = "Prefab/Instantiate/PrefabPositionToggle";
  public const string PREFAB_UI_MAP_POSITION_POINT = "Prefab/Instantiate/PrefabMapPointPosition";
  public const string PREFAB_UI_MAP_RP_POINT = "Prefab/Instantiate/PrefabMapPointRP";
  public const string PREFAB_UI_MEASURE_SLOT = "Prefab/Instantiate/PrefabMeasureSlot";


  public const string PATH_FONT_DEFAULT_MAT = "Font/Pretendard-SemiBold SDF";
  public const string PATH_FONT_GLOW_MAT = "Font/Pretendard-SemiBold SDF_Glow";


  #endregion



}
