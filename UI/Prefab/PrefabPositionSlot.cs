using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PrefabPositionSlot : PrefabUI
{
  [Header("[UI Sibling Transform]")]
  //측정 포인트들
  public Transform referencePointTitle;
  //분석할 아이템들
  public Transform measureItemTitle;

  [Header("[PositionData List]")]
  public List<PrefabPositionToggle> referencePointList  = new List<PrefabPositionToggle>();
  public List<PrefabPositionToggle> measureItemList     = new List<PrefabPositionToggle>();

  public List<PrefabPositionToggle> selectionToggleList = new List<PrefabPositionToggle>();

  [Header("[UI Component]")]
  public Toggle treeViewToggle;
  public Toggle autoToggle;
  public Button selectButton;
  public TextMeshProUGUI positonNameText;
  //창 확대, 축소 시 활성화 되는 게임 오브젝트
  public GameObject bundleDetail;

  [Header("[UI Setting Viewer]")]
  public TextMeshProUGUI mapDataText;
  public Image autoLiftOn;

  [Header("[UI Button Component]")]
  public Button settingButton;
  public Button upArrowButton;
  public Button downArrowButton;

  [Header("[UI Select RP/ITEM]")]
  public Toggle selectAllRPToggle;
  public Toggle selectAllItemToggle;

  public SettingData settingData = new SettingData();

  public string positionImagePath;

  public Action<PrefabPositionSlot> OnSelectSlot;
  
  private void Awake()
  {
    treeViewToggle.onValueChanged.AddListener(ison => { 
      bundleDetail.SetActive(ison);
      
      //이거 레이아웃 강제 업데이트 인데 번들 Detail 활성화했으니 이 슬롯이 레이아웃 가지고 있으니까 업데이트 하게해줘
      LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);

    });

    settingButton  .onClick.AddListener(OpenSetting);
    upArrowButton  .onClick.AddListener(OnClickUpArrowButton);
    downArrowButton.onClick.AddListener(OnClickDownArrowButton);

    selectButton.onClick.AddListener(() => OnSelectSlot?.Invoke(this));

    selectAllRPToggle  .onValueChanged.AddListener(OnValueChangeRPToggle);
    selectAllItemToggle.onValueChanged.AddListener(OnValueChangeItemToggle);


  }

  private void OnSelectToggle(PrefabPositionToggle toggle)
  {
    if (Input.GetKey(KeyCode.LeftShift))
    {
      if (selectionToggleList.Count == 0)
      {
        selectionToggleList.Add(toggle);
        toggle.SetSelect(true);
        //백그라운드 색상 추가
      }
      else
      {
        int firstIndex = selectionToggleList.OrderBy(n => n.transform.GetSiblingIndex()).FirstOrDefault().transform.GetSiblingIndex();
        int secondIndex = toggle.transform.GetSiblingIndex();

        int startIndex = Mathf.Min(firstIndex, secondIndex);
        int endIndex = Mathf.Max(firstIndex, secondIndex);
        //Debug.LogError($"{firstIndex}/{secondIndex}");
        //Debug.LogError($"{Mathf.Min(firstIndex, secondIndex)}/{Mathf.Max(firstIndex, secondIndex)}");

        Transform parent = selectionToggleList[0].transform.parent;

        selectionToggleList.Clear();

        for (int i = startIndex; i <= endIndex; i++)
        {
          var siblingSlot = parent.GetChild(i).GetComponent<PrefabPositionToggle>();

          if (siblingSlot != null)
          {
            siblingSlot.SetSelect(true);
            selectionToggleList.Add(siblingSlot);
          }
        }

        //Debug.LogError("번들 오브젝트 활성화");

        CanvasMeasurementSetting.Instance.measureSetting.bundleToggleSelection.OpenSelection(this.selectionToggleList, () => this.selectionToggleList.Clear());

      }
    }
  }


  /// <summary>
  /// 슬롯 우측에 있는 조그만 설정 버튼
  /// </summary>
  public void OpenSetting()
  {
    CanvasPositionSetting.Instance.SetData(this.settingData, CanvasMeasurementSetting.Instance.measurementMapData.mapData, UpdateUI);
  }

  private void OnValueChangeRPToggle(bool isOn)
  {
    foreach (var rp in referencePointList)
    {
      if (rp.toggle.isOn != isOn)
        rp.toggle.isOn = isOn;
    }
  }

  private void OnValueChangeItemToggle(bool isOn)
  {
    foreach (var rp in measureItemList)
    {
      if(rp.toggle.isOn != isOn)
        rp.toggle.isOn = isOn;
    }
  }

  private void OnClickUpArrowButton()
  {
    int limitIndex = 0;
    int index = transform.GetSiblingIndex();

    if(index == limitIndex)
    {
      Debug.LogError("Position Slot이 이미 최상단에 위치함");
      return;
    }

    transform.SetSiblingIndex(index - 1);
    Debug.LogError("Position Slot 순서 위로 이동");
  }

  private void OnClickDownArrowButton()
  {
    int limitIndex = transform.parent.childCount - 1;
    int index = transform.GetSiblingIndex();

    if (index == limitIndex)
    {
      Debug.LogError("Position Slot이 이미 최하단에 위치함");
      return;

    }

    transform.SetSiblingIndex(index + 1);
    Debug.LogError("Position Slot 순서 아래로 이동");
  }

  public void UpdateUI(SettingData settingData)
  {
    if(settingData != null)
    {
      mapDataText.text = $"[{settingData.mapNode}]";
      autoLiftOn.enabled = settingData.autoLift;

      this.settingData = settingData;
    }
  }


  /// <summary>
  /// Preset Data 기반 데이터 출력
  /// </summary>
  /// <param name="presetData"></param>
  /// <param name="index"></param>
  public void SetData(PresetData presetData, int index)
  {
    PositionDetail positionDetail = presetData.positionDetailList[index];

    settingData = positionDetail.positionData.settingData.DeepCopy();

    positonNameText.text = positionDetail.positionData.positionName;
    positionImagePath = CommonCode.GetFilePath(ConstantManager.PATH_SAVE_MEASURE_PRESET_IMAGE, $"{presetData.presetName}_{positionDetail.positionData.positionName}.png");

    for (int i = 0; i < positionDetail.rpList.Count; i++)
    {
      var referenctPoint = PoolManager.Instance.GetPrefabUI<PrefabPositionToggle>();

      referenctPoint.transform.SetParent(bundleDetail.transform);

      referenctPoint.transform.SetSiblingIndex(GetReferencePointSibling() + i + 1);
      referenctPoint.SetToggleText(positionDetail.rpList[i].pointName);
      referenctPoint.SetToggle(positionDetail.rpList[i].isActive);
      referenctPoint.OnSelectToggle = OnSelectToggle;

      referencePointList.Add(referenctPoint);
    }


    for (int i = 0; i < positionDetail.itemList.Count; i++)
    {
      var measureItem = PoolManager.Instance.GetPrefabUI<PrefabPositionToggle>();

      measureItem.transform.SetParent(bundleDetail.transform);

      measureItem.transform.SetSiblingIndex(GetMeasureItemSibling() + i + 1);
      measureItem.SetToggleText(positionDetail.itemList[i].itemName);
      measureItem.SetToggle(positionDetail.itemList[i].isActive);
      measureItem.OnSelectToggle = OnSelectToggle;

      measureItemList.Add(measureItem);
    }

    UpdateUI(settingData);

    autoToggle.isOn = positionDetail.positionData.isActive;
  }

  /// <summary>
  /// Polyworks PositionData 기반 데이터 출력
  /// </summary>
  /// <param name="positionData"></param>
  /// <param name="positionName"></param>
  public void SetData(PolyWorksPositionData positionData, string positionName, List<string> rpList, List<string> itemList)
  {
    positonNameText.text = positionName;

    settingData = new SettingData();

    for (int i = 0; i < positionData.RP.Length; i++)
    {
      var referenctPoint = PoolManager.Instance.GetPrefabUI<PrefabPositionToggle>();

      string rpName = positionData.RP[i].Split('.')[0];

      referenctPoint.transform.SetParent(bundleDetail.transform);

      referenctPoint.transform.SetSiblingIndex(GetReferencePointSibling() + i + 1);
      referenctPoint.SetToggleText(rpName);
      referenctPoint.OnSelectToggle = OnSelectToggle;

      if (rpList.Contains(rpName))
        referenctPoint.SetToggle(true);
      else
        referenctPoint.SetToggle(false);

      referencePointList.Add(referenctPoint);
    }


    for (int i = 0; i < positionData.ITEM.Length; i++)
    {
      var measureItem = PoolManager.Instance.GetPrefabUI<PrefabPositionToggle>();

      string itemName = positionData.ITEM[i].Split('.')[0];

      measureItem.transform.SetParent(bundleDetail.transform);

      measureItem.transform.SetSiblingIndex(GetMeasureItemSibling() + i + 1);
      measureItem.SetToggleText(itemName);
      measureItem.OnSelectToggle = OnSelectToggle;

      if (itemList.Contains(itemName))
        measureItem.SetToggle(true);
      else
        measureItem.SetToggle(false);

      measureItemList.Add(measureItem);
    }

    UpdateUI(settingData);

    autoToggle.isOn = false;

  }

  public int GetReferencePointSibling()
    => referencePointTitle.GetSiblingIndex();

  public int GetMeasureItemSibling()
    => measureItemTitle.GetSiblingIndex();


  public override void OnDeActive()
  {
    positonNameText.ChangeTMPMaterial(ResourcePath.PATH_FONT_DEFAULT_MAT);

    treeViewToggle.isOn = false;
    autoToggle.isOn = false;

    base.OnDeActive();

    positionImagePath = string.Empty;
    mapDataText.text = "[None]";
    autoLiftOn.enabled = false;

    selectAllRPToggle.SetIsOnWithoutNotify(true);
    selectAllRPToggle.SetIsOnWithoutNotify(true);

    
    referencePointList.Clear();
    measureItemList.Clear();
    selectionToggleList.Clear();

    PoolManager.Instance.ClearReturnPool<PrefabPositionToggle>(bundleDetail.transform);
  }

}
