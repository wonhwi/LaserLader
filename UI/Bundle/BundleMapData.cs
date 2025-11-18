using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BundleMapData : MonoBehaviour
{
  public List<PrefabMapPosition> mapPositionList = new List<PrefabMapPosition>();
  public List<PrefabMapRP> mapRPList = new List<PrefabMapRP>();

  [Header("[Map Image]")]
  private Texture2D texture2D;                 //맵 이미지 Texture
  [SerializeField] private RawImage rawImage;  //맵 이미지 출력 및 MapPoint 부모

  public Texture2D GetTexture() => texture2D;

  public string imagePath = string.Empty;

  /// <summary>
  /// File Path기반 이미지 출력
  /// 이미지 로드 버튼을 통해서 텍스쳐 설정시 실행
  /// </summary>
  /// <param name="path"></param>
  public void LoadImage(string path)
  {
    if(imagePath == path)
    {
      return;
    }

    imagePath = path;

    texture2D = CommonCode.LoadImagePath(path);

    rawImage.texture = texture2D;

    Debug.Log($"{path} Path Image Load");
  }

  /// <summary>
  /// 프리셋 데이터 기반 이미지 출력
  /// </summary>
  public void LoadPresetImage(string presetName)
  {
    texture2D = CommonCode.LoadImageName(presetName);

    rawImage.texture = texture2D;

    Debug.Log($"{presetName} Preset Image Load");
  }

  public void ReturnPool()
  {
    this.mapPositionList.Clear();
    this.mapRPList.Clear();

    PoolManager.Instance.ClearReturnPool<PrefabMapPosition>(rawImage.rectTransform);
    PoolManager.Instance.ClearReturnPool<PrefabMapRP>(rawImage.rectTransform);
  }
}
