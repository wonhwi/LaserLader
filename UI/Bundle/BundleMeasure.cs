using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class BundleMeasure : MonoBehaviour
{
  private List<PrefabMeasureSlot> measureSlotList = new List<PrefabMeasureSlot>();

  public ScrollRect scrollRect;
  public RectTransform content;

  private int totalCount = 0;

  [SerializeField] TextMeshProUGUI measureResults;

  public void SetMeasureSlot(List<string> itemList)
  {
    for (int i = 0; i < itemList.Count; i++)
    {
      int index = i;
      PrefabMeasureSlot measureSlot = PoolManager.Instance.GetPrefabUI<PrefabMeasureSlot>();

      measureSlot.transform.SetParent(content);

      measureSlot.SetData(itemList[index]);
      measureSlot.OnSelectSlot = OnSelectPositionSlot;

      this.measureSlotList.Add(measureSlot);
    }

    totalCount = measureSlotList.Count;

    UpdateResultData();
  }

  /// <summary>
  /// 현재 진행중인지 백그라운드 표시 함수
  /// </summary>
  /// <param name="index"></param>
  public void SetMeasureProcessingState(int index)
  {
    if (measureSlotList.Count > index)
    {
      for (int i = 0; i < measureSlotList.Count; i++)
        measureSlotList[i].SetProcessingBackGround(false);

      measureSlotList[index].SetProcessingBackGround(true);

      // 레이아웃 업데이트를 강제로 실행
      Canvas.ForceUpdateCanvases();
      content.ForceUpdateRectTransforms();

      // 현재 아이템의 RectTransform
      RectTransform itemRect = measureSlotList[index].GetComponent<RectTransform>();

      // Vertical Layout Group의 Spacing 값 가져오기
      float spacing = content.GetComponent<VerticalLayoutGroup>().spacing;

      // 아이템의 높이 (spacing 포함)
      float itemHeight = itemRect.rect.height + spacing;

      // 컨텐츠의 전체 높이
      float contentHeight = content.rect.height;
      // 뷰포트의 높이
      float viewportHeight = scrollRect.viewport.rect.height;

      // 목표 위치 계산 (현재 아이템이 맨 아래에 오도록)
      // spacing을 포함한 각 아이템의 전체 높이를 고려
      float targetPosition = (index * itemHeight) - viewportHeight + itemRect.rect.height;
      targetPosition = Mathf.Max(0, targetPosition);

      // 정규화된 스크롤 위치 계산
      float normalizedPosition = targetPosition / (contentHeight - viewportHeight);
      normalizedPosition = Mathf.Clamp01(normalizedPosition);

      // 스크롤 이동
      scrollRect.verticalNormalizedPosition = 1f - normalizedPosition;
    }
  }


  /// <summary>
  /// 결과 값 출력
  /// </summary>
  /// <param name="macroData"></param>
  /// <param name="index"></param>
  public void SetMeasureResult(MacroData macroData, int index)
  {
    if(measureSlotList.Count > index)
    {
      for (int i = 0; i < measureSlotList.Count; i++)
        measureSlotList[i].SetProcessingBackGround(false);

      measureSlotList[index].SetResultData(macroData);
      

      UpdateResultData();
    }
  }

  public void UpdateResultData()
  {
    int passCount = 0;
    int failedCount = 0;
    int errorCount = 0;

    foreach (var measureSlot in measureSlotList)
    {
      if (measureSlot.result == MacroResultType.OK)
        passCount++;
      else if (measureSlot.result == MacroResultType.ERROR)
        errorCount++;
      else if (measureSlot.result == MacroResultType.NG)
        failedCount++;
    }

    measureResults.text = $"전체 {totalCount}개 <color=#21CFA5>PASS {passCount}개</color> | <color=#EC3738>FAILED {failedCount}개</color> | <color=#F7FA63>ERROR {errorCount}개</color>";
  }

  public void OnSelectPositionSlot(PrefabMeasureSlot slot)
  {
    CanvasPopUp.Instance.OpenGeneral(NotificationType.Infomation, $"{slot.meausreSlotName.text}을 재 측정하시겠습니까?", async () =>
    {
      Debug.LogError($"{slot.meausreSlotName.text} / {slot.transform.GetSiblingIndex()}");
      await ManagerController.Instance.MeasureStartItem(slot.meausreSlotName.text, slot.transform.GetSiblingIndex());
      await ManagerController.Instance.MacroReadyStart();

    });
  }

  public void ClearRuturnPool()
  {
    this.totalCount = 0;
    this.measureSlotList.Clear();
    PoolManager.Instance.ClearReturnPool<PrefabMeasureSlot>(content);
    UpdateResultData();
  }
}
