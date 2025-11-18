using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

public static class ConstantManager
{
  public static readonly float API_STATUS_UPDATE_INTERVAL = 1f;

  public static readonly string AMR_UI_PROCESS_NAME = "pcmm_ui";

  public static readonly int SW_MAXIMIZE = 3; //최대화 및 전체화면
  public static readonly int SW_MINIMIZE = 6; //최소화
  public static readonly int SW_RESTORE  = 9;  //최대화 및 원래화면

  public static readonly Vector2 PositionMapSideScale = new Vector2(0.27f, 0.34f);

  #region 측정 설정에 들어가는 MapPoint Default Value
  public static readonly Vector2 POS_POSITION_MAP_POINT = new Vector2(300f, -200f);
  public static readonly Vector2 POS_RP_MAP_POINT = new Vector2(300f, -135f);
  #endregion

  #region 측정 설정에 들어가는 MapPoint Image 리소스 이름
  public static readonly string MapPointPositionDefaultSprite = "pos_icon_active";
  public static readonly string MapPointPositionSelectSprite = "pos_icon_deactive";

  public static readonly string MapPointRPDefaultSprite = "rp_icon_active";
  public static readonly string MapPointRPSelectSprite = "rp_icon_deactive";
  #endregion

  //측정 설정에서 사용하는 이미지 가져오기 필터 값
  public static readonly FileBrowser.Filter imageFilter = new FileBrowser.Filter("Images", ".png");

  public static readonly float MapImageWidth = 630.011f;
  public static readonly float MapImageHeight = 464.501f;

  //API Lift 오차허용범위
  public static readonly float APILiftHeightTolerance = 2f;

  public static readonly int APILiftHomeHeight = 0; //API Home 할때 0으로 가게
  public static readonly int LiftHomeHeight = 10;  //Front, Back Home실행시 10으로 가게
  public static readonly int LiftDefaultHeight = 140; //mm다


  #region 레지스트리 관련

  #region Lift 
  public static readonly string PlayerPrefsFrontLiftName = "FrontLift";
  public static readonly string PlayerPrefsBackLiftName = "BackLift";
  public static readonly string PlayerPrefsAPILiftName = "APILift";
  #endregion

  /// 폴리웍스 프로젝트 경로 (레지스트리 저장)
  public static readonly string PlayerPrefsProjectPath = "ProjectPath";
  public static readonly string PlayerPrefsMapPath = "MapPath";
  public static readonly string PlayerPrefsMacroTextPath = "MacroTextPath";


  

  /// Lift들을 사용할 USB Port Name
  public static readonly string PlayerPrefsSerialPort = "SerialPort";

  /// 가장 최근에 사용한 프리셋 Name
  public static readonly string PlayerPrefsLatestPresetName = "LatestPresetName";

  #endregion



  /// <summary>
  /// 프리셋 저장 경로
  /// </summary>
  public static readonly string PATH_SAVE_MEASURE_PRESET_DATA = $"{Application.persistentDataPath}/Project";

  /// <summary>
  /// 프리셋의 이미지 저장 경로
  /// </summary>
  public static readonly string PATH_SAVE_MEASURE_PRESET_IMAGE = $"{Application.persistentDataPath}/Image";


}
