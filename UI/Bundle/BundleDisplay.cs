using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BundleDisplay : MonoBehaviour
{
  [System.Serializable]
  public enum MainDisplayType
  {
    Position_Map,
    API_9D,
    AMR_MAP,
  }

  public MainDisplayType mainDisplayType;

  public GameObject bundleDisplay;
  public GameObject bundleNone;

  public Button displayButton;
  public GameObject bundleText;

  public RectTransform parentRect;
  [HideInInspector] public RectTransform rect;

  private void Awake()
  {
    rect = GetComponent<RectTransform>();

    if(displayButton != null)
      displayButton.onClick.AddListener(SetMainDisplay);
  }

  public void SetMainDisplay()
  {
    CanvasMain.Instance.mainUI.SetMainDisplay(this);
  }


  public void SetParent(RectTransform parent)
  {
    rect.SetParent(parent);

    parentRect = parent;
  }

  public void UpdateUI(bool isMain)
  {
    rect.anchoredPosition = Vector2.zero;
    rect.sizeDelta = Vector2.zero;

    if(isMain)
    {
      if (mainDisplayType == MainDisplayType.Position_Map)
      {
        bundleDisplay.transform.localScale = Vector3.one;
        bundleNone.transform.localScale = Vector3.one;
      }
      else
      {
        bundleNone.transform.localScale = Vector3.one * 2;
      }

      bundleText.gameObject.SetActive(false);
      displayButton.interactable = false;
    }
    else
    {
      if (mainDisplayType == MainDisplayType.Position_Map)
      {
        bundleDisplay.transform.localScale = ConstantManager.PositionMapSideScale;
        bundleNone.transform.localScale = Vector3.one * 0.6f;
      }
      else
      {
        bundleNone.transform.localScale = Vector3.one;
      }

      bundleText.gameObject.SetActive(true);
      displayButton.interactable = true;

    }

  }

  /// <summary>
  /// 데이터 있는지 없는지에 따라 번들들 활성화
  /// </summary>
  /// <param name="isActive"></param>
  public void ActiveMapBundle(bool isValidData)
  {
    bundleDisplay.SetActive(isValidData);
    bundleNone.SetActive(!isValidData);
    
  }

}
