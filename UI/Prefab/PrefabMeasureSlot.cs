using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrefabMeasureSlot : PrefabUI
{
  public MacroResultType result = MacroResultType.ING;
  public GameObject processingBackGround;
  public Button RetryButton;
  public TextMeshProUGUI meausreSlotName;
  public TextMeshProUGUI deviationText;
  public Image buttonImage;
  public Image processingImage;

  public Action<PrefabMeasureSlot> OnSelectSlot;

  private void Awake()
  {
    RetryButton.onClick.AddListener(() => OnSelectSlot?.Invoke(this));
  }

  /// <summary>
  /// 현재 작업중인지 판단하는 백그라운드 활성화
  /// </summary>
  public void SetProcessingBackGround(bool isActive)
  {
    processingBackGround.SetActive(isActive);
  }

  public void SetData(string meausreName)
  {
    meausreSlotName.text = meausreName;
  }

  public void SetResultData(MacroData macroData)
  {
    deviationText.text = macroData.data;
    this.result = macroData.result;

    processingImage.enabled = true;

    if (macroData.result == MacroResultType.OK)
    {
      processingImage.sprite = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI, "progress_current_circle");
    }
    else if (macroData.result == MacroResultType.NG)
    {
      processingImage.sprite = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI, "red_check_icon");

    }
    else if (macroData.result == MacroResultType.ERROR)
    {
      processingImage.sprite = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI, "error_small_icon");

    }
  }

  public override void OnDeActive()
  {
    base.OnDeActive();

    processingImage.enabled = false;
    deviationText.text = string.Empty;

    SetProcessingBackGround(false);
    this.result = MacroResultType.ING;
  }
}
