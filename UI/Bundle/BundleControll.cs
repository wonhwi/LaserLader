using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class BundleControll : MonoBehaviour
{
  [Header("[파트 공정률]")]
  [SerializeField] private Image progressBar;
  [SerializeField] private TextMeshProUGUI progressText;  //파트 공정률 Text

  [Header("[충전률/가동시간]")]
  [SerializeField] private Image chargeBar;
  [SerializeField] private TextMeshProUGUI chargeTimeText; //충전률/가동 시간
  [SerializeField] private int chargeValue = 80;           //충전 값
  [HideInInspector] private float startTime;               //시작 시간

  [Header("[중앙 하단 함수 버튼]")]
  public Toggle autoLiftToggle;
  public UIButtonHighLight measureEndButton;     //측정 완료 버튼
  public UIButtonHighLight reportFinishButton;   //리포트, 종료 버튼
  public UIButtonHighLight moveAMRButton;
  public UIButtonHighLight stopAMRButton;
  public GameObject[] bundleAutoMode;
  public GameObject[] bundleManualMode;

  public Toggle autoAMRMoveToggle;


  [Header("[우측 에러 발생 시 계속 실행 토글]")]
  public Toggle continueToggle;

  [Header("[우측 하단 프로세스종료/응급 중지]")]
  public UISelectable processStopButton;
  public Button emergencyButton;

  public void Awake()
  {
    processStopButton.GetComponent<Button>().onClick.AddListener(OnClickProcessStop);
    emergencyButton  .onClick.AddListener(OnClickEmergency);
  }

  private void Start() => startTime = Time.time;

  private void Update() => SetAMRChargeTimeText();

  #region 파트 공정률
  /// <summary>
  /// 파트 공정률
  /// </summary>
  public void UpdateProcessPercent(int totalCount, int completeCount)
  {
    int percent = (int)((float)completeCount / (float)totalCount * 100f);

    progressBar.fillAmount = Mathf.Clamp((float)completeCount / (float)totalCount, 0.02f, 1f);
    progressText.text = $"{percent}% (작업 {completeCount}개/전체 {totalCount}개)";
  }
  #endregion

  #region 충전률/가동시간
  public void SetChargeValue(int chargeValue)
  {
    this.chargeValue = chargeValue;
  }

  /// 충전률/가동시간 출력
  /// </summary>
  private void SetAMRChargeTimeText()
  {
    chargeBar.fillAmount = Mathf.Clamp((float)chargeValue / 100f, 0.02f, 1f);
    chargeTimeText.text = $"<color={GetBatteryColor(chargeValue)}>{chargeValue}%</color> ({GetOperatingTime()})";
  }

  private string GetOperatingTime()
  {
    float elapsedTime = Time.time - startTime;
    int hours = (int)(elapsedTime / 3600);
    int minutes = (int)((elapsedTime % 3600) / 60);
    return $"{hours:D2}시간 {minutes:D2}분";
  }

  
  private string GetBatteryColor(int remainValue)
  {
    if (remainValue <= 20)
    {
      return "red";
    }
    //else if (remainValue <= 50)
    //{
    //  return Color.yellow.ToHexString(); // 20% 초과 50% 이하일 때 노란색
    //}
    else
    {
      return "green";//return Color.green.ToHexString(); // 50% 초과일 때 초록색
    }
  }
  #endregion

  #region AMR 수동/자동 판단으로 버튼 활성화/비활성화
  public void SetAMRMode(bool isAuto)
  {
    for (int i = 0; i < bundleAutoMode.Length; i++)
      bundleAutoMode[i].SetActive(isAuto);
    for (int i = 0; i < bundleManualMode.Length; i++)
      bundleManualMode[i].SetActive(!isAuto);
  }
  #endregion

  /// <summary>
  /// 프로세스 중지
  /// </summary>
  private void OnClickProcessStop()
  {
    CanvasPopUp.Instance.OpenGeneral(
      NotificationType.Infomation, 
      "프로세스를 중단하시겠습니까?",
      confirmAction : ManagerController.Instance.StopProcess
      );
  }

  /// <summary>
  /// 응급 정지
  /// 1. AMR 이동 정지 명령 단발성
  /// 2. 리프트 응급 정지 명령
  /// 3. 기존 작동되던 프로세서 작동 중지
  /// </summary>
  private async void OnClickEmergency()
  {
    Debug.LogError("EmergencyStop");

    ManagerLog.AddLog(LogType.Error, "응급 정지");
#if !UNITY_EDITOR
    ManagerTransport.Instance.AMR_Stop();
    ManagerTransport.Instance.Lift_EMO();
#endif

    CanvasPopUp.Instance.OpenNotice(
      NotificationType.Infomation,
      "긴급 중단 버튼을 눌렀습니다."
      );

    ManagerController.Instance.StopProcess();
  }

}
