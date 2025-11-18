using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIStatus : MonoBehaviour
{
  [SerializeField] private Image statusImage;
  [SerializeField] private TextMeshProUGUI statusText;
  
  public void SetUIStatus(DeviceStatusType statusType)
  {
    statusImage.sprite = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI, CommonCode.GetDeviceStatusImage(statusType));
    statusText.text = CommonCode.GetDeviceStatusText(statusType);
  }
}
