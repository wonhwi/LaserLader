using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class UIVersion : MonoBehaviour
{
  public TextMeshProUGUI versionText;

  private void Awake()
    => SetVersion();

  void SetVersion()
  {
    // 기존 버전 가져오기
    string version = Application.version;
    string[] parts = version.Split('.');

    if (parts.Length < 3)
    {
      Debug.LogError("버전 형식이 잘못되었습니다. (예: 1.0.0)");
      return;
    }

    int major = int.Parse(parts[0]);
    int minor = int.Parse(parts[1]);
    int build = int.Parse(parts[2]);

    versionText.text = $"v {major}.{minor}.{build}";
  }
}
