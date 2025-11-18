using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildManager
{
  private static BuildOptions buildOption = BuildOptions.Development;
  public const string buildPath = @"C:\GitHub\Build";

  [MenuItem("Tools/Increase Version and Build")] // Unity Editor에서 실행 버튼 추가
  public static void IncreaseVersionAndBuild()
  {
    // 기존 버전 가져오기
    string version = PlayerSettings.bundleVersion;
    string[] parts = version.Split('.');

    if (parts.Length < 3)
    {
      Debug.LogError("버전 형식이 잘못되었습니다. (예: 1.0.0)");
      return;
    }

    int major = int.Parse(parts[0]);
    int minor = int.Parse(parts[1]);
    int build = int.Parse(parts[2]);

    // 빌드 버전 증가
    build++;
    string newVersion = $"{major}.{minor}.{build}";
    PlayerSettings.bundleVersion = newVersion;

    string targetPath = buildPath + @$"\HMI_v{newVersion}";

    if (!Directory.Exists(targetPath))
    {
      Directory.CreateDirectory(targetPath);
    }


    Debug.Log($"빌드 버전 증가: {newVersion}");
    // 빌드 실행 (필요하면 경로 수정)
    BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, targetPath + @$"\{PlayerSettings.productName}.exe", BuildTarget.StandaloneWindows, buildOption);
  }
}
