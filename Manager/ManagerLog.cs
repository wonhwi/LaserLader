using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManagerLog : MonoBehaviour
{
  [SerializeField] private Text logTextClone;
  [SerializeField] private ScrollRect scrollRectClone;

  [SerializeField] private static Text logText;
  [SerializeField] private static ScrollRect scrollRect;

  public static bool visualFullLog = true;
  private static string fullText;
  private static string newText;

  private void Awake()
  {
    logText = logTextClone;
    scrollRect = scrollRectClone;
  }

  public static void AddLog(LogType logType, string message)
  {
    string timeStamp = System.DateTime.Now.ToString("[HH:mm:ss]");
    string newEntry = $"<color={GetLogColor(logType)}>{timeStamp} {message}</color>";

    newText = newEntry;

    // 새로운 로그 추가 (새 줄 문자로 구분)
    if (string.IsNullOrEmpty(logText.text))
    {
      fullText = newEntry;
    }
    else
    {
      fullText += "\n" + newEntry;
    }

    if(visualFullLog)
    {
      logText.text = fullText;
    }
    else
    {
      logText.text = newText;
    }

    // 다음 프레임에서 스크롤을 최하단으로 이동
    Canvas.ForceUpdateCanvases();
    scrollRect.verticalNormalizedPosition = 0f;

    string GetLogColor(LogType logType) => logType switch
    {
      LogType.Log   => "#FFFFFF",
      LogType.Error => "#FF0000",
    };
  }

  public static void ToggleLog()
  {
    visualFullLog = !visualFullLog;

    if (visualFullLog)
      logText.text = fullText;
    else
    {
      logText.text = newText;
    }

    // 다음 프레임에서 스크롤을 최하단으로 이동
    Canvas.ForceUpdateCanvases();
    scrollRect.verticalNormalizedPosition = 0f;

  }

  

  // 로그 초기화
  public static void ClearLog()
  {
    fullText = string.Empty;
    newText = string.Empty;

    logText.text = string.Empty;
  }
}
