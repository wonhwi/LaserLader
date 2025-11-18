using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class CommonCode
{
  /// <summary>
  /// IP 주소 확인
  /// </summary>
  /// <returns></returns>
  public static string GetIPAddress()
  {
    string localIP = "Unable to get local IP address.";
    try
    {
      // 모든 네트워크 인터페이스 가져오기
      var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
      foreach (var ip in host.AddressList)
      {
        // IPv4 주소만 필터링
        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
          localIP = ip.ToString();
          break; // 첫 번째 IPv4 주소 반환
        }
      }
    }
    catch (Exception ex)
    {
      Debug.LogError("Error getting local IP address: " + ex.Message);
    }
    return localIP;
  }

  [DllImport("user32.dll")]
  private static extern bool ShowWindow(int hWnd, int nCmdShow);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool SetForegroundWindow(IntPtr hWnd);

  /// <summary>
  /// AMR 프로그램 실행
  /// </summary>
  public static void OpenAMRUI()
  {
    ShowWindowProcess(ConstantManager.AMR_UI_PROCESS_NAME, ConstantManager.SW_RESTORE);
  }

  /// <summary>
  /// 프로세스 윈도우 활성화
  /// </summary>
  /// <param name="processName"></param>
  /// <param name="typeValue">최소화 = 6, 최대화 및 전체화면 = 3, 최대화 및 원래 화면 = 9</param>
  private static void ShowWindowProcess(string processName, int typeValue)
  {
    Process[] processes = Process.GetProcessesByName(processName);
    foreach (Process process in processes)
    {
      IntPtr hWnd = process.MainWindowHandle;

      ShowWindow(hWnd.ToInt32(), typeValue);
      SetForegroundWindow(hWnd);

      Marshal.FreeHGlobal(hWnd);
    }
  }

  /// <summary>
  /// 비트 시프트 및 논리합으로 정수 생성
  /// </summary>
  /// <param name="bytes"></param>
  /// <returns></returns>
  public static int ByteArrayToInt(byte[] bytes)
  {
    return (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];
  }

  /// <summary>
  /// 체크섬 반환
  /// </summary>
  /// <param name="id"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static int GetCheckSumLiftHeight(int id, byte[] value)
  {
    byte[] byteArray = new byte[] { 184, 183, (byte)id, 197, 4, value[0], value[1], value[2], value[3] };

    byte cs = 0;
    foreach (var b in byteArray)
    {
      cs += b; // 모든 바이트의 합 계산
    }
    cs = (byte)(~cs + 1); // 체크섬 계산
    return cs; // 체크섬이 추가된 데이터 반환
  }

  /// <summary>
  /// 시리얼라이징
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static string ConvertAllValuesToString<T>(T obj)
  {
    var stringProperties = new Dictionary<string, string>();
    var properties = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

    foreach (var prop in properties)
    {
      var value = prop.GetValue(obj);
      stringProperties.Add(prop.Name, value?.ToString() ?? "");
    }

    return JsonConvert.SerializeObject(stringProperties, Formatting.Indented);
  }

  /// <summary>
  /// UTC Time 반환
  /// </summary>
  /// <returns></returns>
  public static long GetUTCTime()
  {
    // Unix Epoch 시작 시간
    DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // UTC 시간과 Unix Epoch 간의 차이를 밀리초로 계산
    long milliseconds = (long)(DateTime.UtcNow - unixEpoch).TotalMilliseconds;

    return milliseconds;
  }

  /// <summary>
  /// 현재 시간 반환
  /// </summary>
  /// <returns></returns>
  public static string GetCurrentTime()
  {
    // 현재 날짜와 시간 가져오기
    DateTime now = DateTime.Now;

    // 원하는 형식으로 변환
    return $"{now:yyMMdd}_{now:HHmm}";
  }

  

  public static int GetRegistryIntValue(string key)
  {
    return PlayerPrefs.GetInt(key, 0);
  }

  public static string GetRegistryStringValue(string key)
  {
    return PlayerPrefs.GetString(key, "");
  }

  #region 이미지 관련

  /// <summary>
  /// 이미지 파일 삭제
  /// </summary>
  /// <param name="imageName"></param>
  public static void DeleteImage(string imageName)
  {
    string path = $"{ConstantManager.PATH_SAVE_MEASURE_PRESET_IMAGE}";

    string[] files = Directory.GetFiles(path);

    foreach (string file in files)
    {
      // 파일 이름과 확장자를 분리
      string fileName = Path.GetFileNameWithoutExtension(file);

      // 파일 이름이 일치하면 삭제
      if (fileName == imageName)
      {
        File.Delete(file);
        Debug.Log($"파일 삭제됨: {file}");
      }
    }
  }

  /// <summary>
  /// 이미지 로드
  /// </summary>
  /// <param name="imageName"></param>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <returns></returns>
  public static Texture2D LoadImageName(string imageName, int width = 400, int height = 400)
  {
    string path = GetFilePath(ConstantManager.PATH_SAVE_MEASURE_PRESET_IMAGE, $"{imageName}.png");

    // 파일이 존재하는지 확인
    if (File.Exists(path))
    {
      // 파일에서 이미지 데이터를 읽어옴
      byte[] fileData = File.ReadAllBytes(path);
      Texture2D texture = new Texture2D(width, height); // 임시 텍스처 생성
      texture.LoadImage(fileData); // 이미지 데이터를 텍스처에 로드
      return texture;
    }
    else
    {
      Debug.LogError("파일이 존재하지 않습니다: " + path);
      return null;
    }
  }

  /// <summary>
  /// 이미지 경로 로드
  /// </summary>
  /// <param name="imagePath"></param>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <returns></returns>
  public static Texture2D LoadImagePath(string imagePath, int width = 400, int height = 400)
  {
    // 파일이 존재하는지 확인
    if (File.Exists(imagePath))
    {
      // 파일에서 이미지 데이터를 읽어옴
      byte[] fileData = File.ReadAllBytes(imagePath);
      Texture2D texture = new Texture2D(width, height); // 임시 텍스처 생성
      texture.LoadImage(fileData); // 이미지 데이터를 텍스처에 로드
      return texture;
    }
    else
    {
      Debug.LogError("파일이 존재하지 않습니다: " + imagePath);
      return null;
    }

  }

  #endregion

  public static bool CreatePath(string path)
  {
    if(!Directory.Exists(path))
      Directory.CreateDirectory(path);

    return true;
  }

  public static string GetFilePath(string path, string fileName)
  {
    return Path.Combine(path, fileName).Replace('/', '\\');
  }

  public static string WriteTestMacroTextData(string data)
  {
    string path = GetFilePath(GetRegistryStringValue(ConstantManager.PlayerPrefsMacroTextPath), $"{PolyworksMacroType.PW}.txt");

    // UTF-16 인코딩으로 파일 저장
    using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.Unicode))
    {
      writer.Write(data);

    }

    return data;

  }

  
  public static string WritePolyworksTextData(string data)
  {
    string path = GetFilePath(GetRegistryStringValue(ConstantManager.PlayerPrefsMacroTextPath), $"{PolyworksMacroType.HMI}.txt");

    // UTF-16 인코딩으로 파일 저장
    using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.Unicode))
    {
      writer.Write(data);

    }

    return data;
  }

  /// <summary>
  /// 매크로 Text 값 반환
  /// </summary>
  /// <returns></returns>
  public static string[] ReadPolyworksTextData()
  {
    string path = GetFilePath(GetRegistryStringValue(ConstantManager.PlayerPrefsMacroTextPath), $"{PolyworksMacroType.PW}.txt");

    string[] splitData = File.ReadAllText(path).Split("\r\n");

    return splitData;
  }

  public static PolyWorksPositionData GetPositionData(string positionPath)
  {
    string fileData = GetJsonData($"{GetRegistryStringValue(ConstantManager.PlayerPrefsProjectPath)}/{positionPath}");

    return GetDeserializeObject<PolyWorksPositionData>(fileData);
  }

  public static PresetData GetPresetData(string presetName)
  {
    string fileData = GetJsonData($"{ConstantManager.PATH_SAVE_MEASURE_PRESET_DATA}/{presetName}");

    return GetDeserializeObject<PresetData>(fileData);
  }

  public static float[] GetMapNodeOffset(string mapName, string mapNode)
  {
    string fileData = GetJsonData($"{GetRegistryStringValue(ConstantManager.PlayerPrefsMapPath)}/{mapName}/topo");

    List<MapNodeData> mapNodeDataList = GetDeserializeObject<List<MapNodeData>>(fileData);

    MapNodeData mapData = mapNodeDataList.Find(n => n.name == mapNode);

    string[] infos = mapData.info.Split("\n");

    float[] floatValues = new float[3];
    foreach (var info in infos)
    {
      if (info.StartsWith("BQR_OFFSET"))
      {
        // "BQR_OFFSET," 부분을 제거하고 숫자 부분만 추출
        string[] parts = info.Substring("BQR_OFFSET,".Length).Split(',');

        // 문자열 배열을 float 배열로 변환
        floatValues = Array.ConvertAll(parts, float.Parse);
      }
    }
    return floatValues;
  }

  public static List<string> GetMapNodes(string mapName, string type)
  {
    string fileData = GetJsonData($"{GetRegistryStringValue(ConstantManager.PlayerPrefsMapPath)}/{mapName}/topo");

    List<MapNodeData> mapNodeDataList = GetDeserializeObject<List<MapNodeData>>(fileData);

    if (mapNodeDataList == null)
      return null;

    return mapNodeDataList.Where(n => n.type == type).Select(m => m.name).ToList();
  }

  public static List<string> GetPresetFiles()
  {
    string path = ConstantManager.PATH_SAVE_MEASURE_PRESET_DATA;

    string[] files = Directory.GetFiles(path, "*.json");

    return files.Select(n => Path.GetFileNameWithoutExtension(n)).ToList();
  }

  public static List<string> GetProjectFiles()
  {
    string path = GetRegistryStringValue(ConstantManager.PlayerPrefsProjectPath);

    string[] directories = Directory.GetDirectories(path);

    return directories.Select(n => Path.GetFileName(n)).ToList();
  }

  public static List<string> GetPartsFiles(string projectName)
  {
    string path = GetFilePath(GetRegistryStringValue(ConstantManager.PlayerPrefsProjectPath), projectName);

    // 모든 파일 가져오기
    string[] files = Directory.GetFiles(path, "*.json");


    return files.Select(n => Path.GetFileNameWithoutExtension(n)).ToList();
  }

  public static string GetJsonData(string jsonPath)
  {
    string jsonFile = $"{jsonPath.Replace('/', '\\')}.json";

    if (File.Exists(jsonFile))
      return File.ReadAllText(jsonFile);

    return "";
  }

  public static T GetDeserializeObject<T>(string data) where T : class
  {
    return JsonConvert.DeserializeObject<T>(data);
  }


  public static string GetDeviceStatusText(DeviceStatusType statusType) => statusType switch
  {
    DeviceStatusType.AMR_On  => "On",
    DeviceStatusType.AMR_Off => "OFF",
    DeviceStatusType.AMR_Stable => "정상",
    DeviceStatusType.AMR_Error => "Error",

    DeviceStatusType.AMR_Moving_Auto => "자동",
    DeviceStatusType.AMR_Moving_Manual => "수동",
    DeviceStatusType.AMR_Moving_None => "대기",
    DeviceStatusType.AMR_Moving_Docking => "도킹",

    DeviceStatusType.AMR_Docking_Wait => "대기",
    DeviceStatusType.AMR_Docking_Success => "도킹성공",
    DeviceStatusType.AMR_Docking_Fail => "도킹 실패",
    DeviceStatusType.AMR_Docking_Move => "도킹 중",

    DeviceStatusType.Connect => "연결",
    DeviceStatusType.DisConnect => "해제",

    DeviceStatusType.Lift_On => "On",
    DeviceStatusType.Lift_Home => "Home",
  };

  public static string GetDeviceStatusImage(DeviceStatusType statusType) => statusType switch
  {
    DeviceStatusType.AMR_On or 
    DeviceStatusType.AMR_Stable or 
    DeviceStatusType.Lift_On or
    DeviceStatusType.AMR_Moving_None
      => "common_blue_box",

    DeviceStatusType.AMR_Off or 
    DeviceStatusType.AMR_Error or 
    DeviceStatusType.Lift_Home
      => "common_red_box",

    DeviceStatusType.AMR_Moving_Manual or
    DeviceStatusType.AMR_Moving_Auto 
      => "common_yellow_box",
    //DeviceStatusType.AMR_Moving_Auto => "common_gray_box",
    DeviceStatusType.AMR_Moving_Docking 
      => "common_green_box",
  };
}
