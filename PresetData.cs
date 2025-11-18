using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 측정 설정에 들어가는 위치 포인트 클래스
/// </summary>
[System.Serializable]
public class MapNodeData
{
  public string id;
  public string info;
  public string[] links;
  public string name;
  public string pose;
  public string type;

  //public float[] GetOffset()
  //{
  //  MapNodeData mapData = mapNodeDataList.Find(n => n.name == testMapNodeName);
  //
  //  string[] infos = mapData.info.Split("\n");
  //
  //  float[] floatValues = new float[3];
  //  foreach (var info in infos)
  //  {
  //    if (info.StartsWith("BQR_OFFSET"))
  //    {
  //      // "BQR_OFFSET," 부분을 제거하고 숫자 부분만 추출
  //      string[] parts = info.Substring("BQR_OFFSET,".Length).Split(',');
  //
  //      // 문자열 배열을 float 배열로 변환
  //      floatValues = Array.ConvertAll(parts, float.Parse);
  //    }
  //  }
  //}
}

/// <summary>
/// 측정 설정에 들어가는 위치 포인트 클래스
/// </summary>
[System.Serializable]
public class MapPoint
{
  public string pointName;
  public float x;
  public float y;
}

[System.Serializable]
public class SettingData
{
  //사용하는 맵데이터의 노드 이름
  public string mapNode;

  //바닥 고정 리프트 자동/수동 여부
  public bool autoLift;

  //바닥 고정 리프트 설정 값
  public int FrontLiftHeight;
  public int BackLiftHeight;

  //Position 도달 시 API Lift 위치 설정
  public int APILiftHeight;

  public SettingData()
  {
    this.mapNode = "None";
    this.autoLift = false;

    this.FrontLiftHeight = -1;
    this.BackLiftHeight = -1;

    this.APILiftHeight = -1;
  }
}

/// <summary>
/// 폴리웍스에서 만드는 json DeSerialize 용 Class
/// </summary>
[System.Serializable]
public class PolyWorksPositionData
{
  public string PROJECT;
  public string PART;
  public string[] POSITION;
  public string[] RP;
  public string[] ITEM;
}

/// <summary>
/// 자체 로컬 저장용 Preset Deserialize 용 Class
/// </summary>
[System.Serializable]
public class PresetData
{
  public string presetName;
  public string projectName;      //프로젝트 이름
  public string partName;         //파트 이름

  public bool isAuto;         //전반적인 자동/수동 여부
  public string mapData;      //사용할 맵데이터 이름
  public List<PositionDetail> positionDetailList;
  public List<MapPoint> positionPointList;
  public List<MapPoint> rpPointList;

  /// <summary>
  /// 프리셋에서 측정할 모든 아이템들에 대한 값 반환 (체크한것들만)
  /// </summary>
  /// <returns></returns>
  //public List<string> GetAllItemList()
  //{
  //  List<string> list = new List<string>();
  //
  //  foreach (var positionDetail in positionDetailList)
  //  {
  //    foreach (var itemData in positionDetail.itemList)
  //    {
  //      if (itemData.isActive)
  //        list.Add(itemData.itemName);
  //    }
  //  }
  //
  //  return list;
  //}

  /// <summary>
  /// 중앙 하단에 위치하는 토탈 ItemList
  /// </summary>
  /// <returns></returns>
  public int GetTotalItemCount()
  {
    int count = 0;

    foreach (var positionDetail in positionDetailList)
    {
      if(positionDetail.positionData.isActive)
      {
        foreach (var itemData in positionDetail.itemList)
        {
          if (itemData.isActive)
            count++;
        }
      }
    }

    return count;
  }
}


[System.Serializable]
public class PositionDetail
{
  public PositionData positionData;
  public List<RPData> rpList;
  public List<ItemData> itemList;


  /// <summary>
  /// 해당 포지션에 사용할 RP 활성화한 데이터들 이름 반환
  /// </summary>
  /// <returns></returns>
  public List<string> GetActivePointList()
  {
    List<string> list = new List<string>();

    foreach (var rp in rpList)
    {
      if (rp.isActive)
        list.Add(rp.pointName);
    }

    return list;
  }

  /// <summary>
  /// 해당 포지션에 사용할 측정할 활성화한 아이템 데이터들 이름 반환
  /// </summary>
  /// <returns></returns>
  public List<string> GetActiveItemList()
  {
    List<string> list = new List<string>();

    foreach (var item in itemList)
    {
      if (item.isActive)
        list.Add(item.itemName);
    }

    return list;
  }
}

[System.Serializable]
public class PositionData
{
  public string positionName;
  public SettingData settingData;
  public bool isActive;

}

[System.Serializable]
public class RPData
{
  public string pointName;
  public bool isActive;

}

[System.Serializable]
public class ItemData
{
  public string itemName;
  public bool isActive;

}

public class MacroData
{
  public MacroType macroType;
  public MacroResultType result;
  public string data;
}



