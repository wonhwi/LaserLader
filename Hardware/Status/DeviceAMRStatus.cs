using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using static MessageData;

public class DeviceAMRStatus : MonoBehaviour
{
  public string amrState = "";      //Moving State
  public string autoState = "";     //Auto State
  public string dockingState = "";  //Docking State

  #region 
  [Header("[AMR 상태]")]
  public UIStatus UIAMRPower;        //AMR 전원 상태
  public UIStatus UIAMRStatus;       //AMR 상태
  public UIStatus UIAMRMovingStatus; //주행상태
  //public UIStatus UIAMRDockingStatus; //주행상태


  [Header("[AMR Map UI]")]
  [SerializeField] private RawImage videoImage;
  [HideInInspector] private Texture2D videoTexture;

  public void Awake()
  {
    MessageBroker.Default.Receive<ReceiveStatusMessage>().Subscribe(OnReceiveStatus);
    MessageBroker.Default.Receive<ReceiveError>().Subscribe(OnReceiveStatus);
  }


  public string GetAMRState()        => amrState;
  public string GetAMRAutoState()    => autoState;
  public string GetAMRDockingState() => dockingState;

  public void UpdateDisplay(byte[] imageData, int width, int height, int channels)
  {
    // BGR to RGB 변환
    byte[] rgbData = new byte[imageData.Length];
    for (int i = 0; i < imageData.Length - 2; i += 3)
    {
      rgbData[i] = imageData[i + 2];     // R
      rgbData[i + 1] = imageData[i + 1]; // G
      rgbData[i + 2] = imageData[i];     // B
    }

    videoTexture = new Texture2D(width, height, TextureFormat.RGB24, false);

    videoTexture.LoadRawTextureData(rgbData);
    videoTexture.Apply();

    // 텍스처를 RawImage에 적용
    if (videoImage != null)
    {
      videoImage.texture = videoTexture;
    }
    else
    {
      Debug.LogError("RawImage component is not assigned.");
    }
  }

  


  public void OnReceiveStatus(ReceiveError message)
  {
    if (!string.IsNullOrEmpty(message.error_type))
      UIAMRStatus.SetUIStatus(DeviceStatusType.AMR_Error);
    else
      UIAMRStatus.SetUIStatus(DeviceStatusType.AMR_Stable);
  }

  public void OnReceiveStatus(ReceiveStatusMessage message)
  {
    if (message.state.power == "true")
      UIAMRPower.SetUIStatus(DeviceStatusType.AMR_On);
    else
      UIAMRPower.SetUIStatus(DeviceStatusType.AMR_Off);

    if (message.condition.moving_state == "auto")
      UIAMRMovingStatus.SetUIStatus(DeviceStatusType.AMR_Moving_Auto);
    else if (message.condition.moving_state == "manual")
      UIAMRMovingStatus.SetUIStatus(DeviceStatusType.AMR_Moving_Manual);
    else if (message.condition.moving_state == "docking")
      UIAMRMovingStatus.SetUIStatus(DeviceStatusType.AMR_Moving_Docking);
    else if (message.condition.moving_state == "none")
      UIAMRMovingStatus.SetUIStatus(DeviceStatusType.AMR_Moving_None);
      
    CanvasMain.Instance.mainUI.bundleControll.SetChargeValue(message.power.bat_per);

    amrState     = message.condition.moving_state;
    autoState    = message.condition.auto_state;
    dockingState = message.condition.docking_state;

    Debug.LogError($"movingState, {amrState} AutoState {autoState}, DockingState = {dockingState}");
  }
  #endregion


}
