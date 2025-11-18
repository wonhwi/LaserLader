using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectable : MonoBehaviour
{
  public CanvasGroup canvasGroup;

  private void Reset()
  {
    canvasGroup = GetComponent<CanvasGroup>();
  }

  public void SetBlockRaycasts(bool isBlock)
  {
    canvasGroup.blocksRaycasts = isBlock;
  }

  /// <summary>
  /// 선택가능하게 할껀지 여부
  /// </summary>
  /// <param name="isSelectable"></param>
  public void SetSelectable(bool isSelectable)
  {
    this.SetBlockRaycasts(isSelectable);

    canvasGroup.alpha = isSelectable ? 1 : 0.2f;
  }
}
