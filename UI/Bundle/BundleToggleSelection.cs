using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BundleToggleSelection : MonoBehaviour
{
  public Button selectButton;
  public Button deSelectButton;

  public Vector2 offsetVector = new Vector2(80, 60);

  public void OpenSelection(List<PrefabPositionToggle> toggles, Action action)
  {
    Vector2 mousePosition = Input.mousePosition;

    // UI 좌표로 변환 (스크린 좌표 → 월드 좌표)
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        CanvasMeasurementSetting.Instance.transform as RectTransform, // 캔버스의 RectTransform
        mousePosition,                     // 마우스 포인터 위치
        null,                // 렌더링 카메라
        out Vector2 localPoint);           // 변환된 UI 로컬 좌표

    // UI 오브젝트 위치 설정
    this.transform.localPosition = localPoint + offsetVector;


    this.gameObject.SetActive(true);

    selectButton.onClick.RemoveAllListeners();
    deSelectButton.onClick.RemoveAllListeners();

    selectButton.onClick.AddListener(() =>
    {
      foreach (var toggle in toggles)
      {
        toggle.SetToggle(true);
        toggle.SetSelect(false);
      }

      action?.Invoke();

      this.gameObject.SetActive(false);
    });
    deSelectButton.onClick.AddListener(() => 
    {
      foreach (var toggle in toggles)
      {
        toggle.SetToggle(false);
        toggle.SetSelect(false);
      }

      action?.Invoke();

      this.gameObject.SetActive(false);
    });
  }

}
