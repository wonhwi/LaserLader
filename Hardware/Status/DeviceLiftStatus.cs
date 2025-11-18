using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class DeviceLiftStatus : MonoBehaviour
{
  public SerialSocket serialSocket;

  public LiftType liftType = LiftType.MainLift;

  public bool IsGetHeight = false;

  [Header("[리프트 현재 위치 값]")]
  public double frontLiftHeight;
  public double backLiftHeight;
  public double mainLiftHeight;

  private byte[] frontHeightByte = new byte[]{255, 255, 255, 255};
  private byte[] backHeightByte  = new byte[]{255, 255, 255, 255};
  private byte[] mainHeightByte  = new byte[]{255, 255, 255, 255};


  [Header("[UI Status]")]
  public UIStatus LiftStatus;
  public UIStatus APILiftStatus;
  public TextMeshProUGUI APILiftHeightText;

  public async UniTask WaitHome(LiftType liftType)
  {
    while(true)
    {
      byte[] bytes = GetTargetByte(liftType);

      bool isHome = true;

      for (int i = 0; i < bytes.Length; i++)
      {
        if (bytes[i] != 0)
        {
          isHome = false;
          break;
        }
      }

      if(isHome)
      {
        ManagerLog.AddLog(LogType.Log, $"{liftType} Home 도착");
        break;
      }
      else
      {
        ManagerLog.AddLog(LogType.Log, $"{liftType} Home 이동 중 현재 위치 {GetLiftHeight(liftType):F2}");
      }

      await UniTask.Delay(2000);
    }
  }

  public double GetLiftHeight(LiftType liftType) => liftType switch
  {
    LiftType.FrontLift => frontLiftHeight,
    LiftType.BackLift  => backLiftHeight,
    LiftType.MainLift  => mainLiftHeight,
  };

  private byte[] GetTargetByte(LiftType liftType) => liftType switch
  {
    LiftType.FrontLift => frontHeightByte,
    LiftType.BackLift  => backHeightByte,
    LiftType.MainLift  => mainHeightByte,
  };
  
  public void SetUIAPILiftHeight(int height)
  {
    APILiftHeightText.text = $"{height} mm";
  }

  public void UpdateUILiftStatus()
  {
    if(this.frontLiftHeight < ConstantManager.LiftHomeHeight + ConstantManager.APILiftHeightTolerance && 
       this.backLiftHeight  < ConstantManager.LiftHomeHeight + ConstantManager.APILiftHeightTolerance)
      LiftStatus.SetUIStatus(DeviceStatusType.Lift_Home);
    else
      LiftStatus.SetUIStatus(DeviceStatusType.Lift_On);

    if (this.mainLiftHeight < ConstantManager.APILiftHeightTolerance)
      APILiftStatus.SetUIStatus(DeviceStatusType.Lift_Home);
    else
      APILiftStatus.SetUIStatus(DeviceStatusType.Lift_On);
  }


  private IEnumerator Start()
  {
    yield return new WaitUntil(() => serialSocket != null);
    yield return new WaitUntil(() => serialSocket.ioSocket != null && serialSocket.ioSocket.IsOpen);
    while (true)
    {
      if (IsGetHeight)
        LiftController.GetLiftHeight((int)liftType);
      yield return YieldInstructionCache.WaitForSeconds(1f);
    }
  }

  public void SetLiftType(LiftType liftType)
  {
    this.liftType = liftType;

    this.IsGetHeight = true;
  }

  public bool IsArrive(int id, int targetHeight)
  {
    if (id == (int)LiftType.FrontLift)
    {
      if(Mathf.Abs((float)(targetHeight - frontLiftHeight)) < ConstantManager.APILiftHeightTolerance)
      {
        this.IsGetHeight = false;

        return true;
      }
      return false;
    }
    else if (id == (int)LiftType.BackLift)
    {
      if (Mathf.Abs((float)(targetHeight - backLiftHeight)) < ConstantManager.APILiftHeightTolerance)
      {
        this.IsGetHeight = false;

        return true;
      }
      return false;
    }
    else if (id == (int)LiftType.MainLift)
    {
      if (Mathf.Abs((float)(targetHeight - mainLiftHeight)) < ConstantManager.APILiftHeightTolerance)
      {
        this.IsGetHeight = false;

        return true;
      }
      return false;
    }

    return false;
  }


  public void Update()
  {
    if (serialSocket != null && serialSocket.ioSocket != null && serialSocket.ioSocket.IsOpen)
    {
      if (serialSocket.ioSocket.BytesToRead > 0)
      {
        int byteToRead = serialSocket.ioSocket.BytesToRead;

        byte[] buffer = new byte[byteToRead];
        serialSocket.ioSocket.Read(buffer, 0, buffer.Length); // byte[] 배열로 데이터 수신

        GetTargetHeight(buffer, (int)this.liftType);
      }
    }

    UpdateUILiftStatus();
  }

  private void GetTargetHeight(byte[] buffer, int id)
  {
    byte[] targetSequence = new byte[] { 184, 183, (byte)id, 197, 4 };

    // 시퀀스의 시작 인덱스 찾기
    int startIndex = Array.IndexOf(buffer, targetSequence[0]);

    byte[] target = new byte[4];

    if (startIndex != -1)
    {
      if (startIndex + targetSequence.Length + target.Length + 1 <= buffer.Length)
      {
        bool isMatchSequence = true;

        for (int i = 0; i < targetSequence.Length; i++)
        {
          if (buffer[startIndex + i] != targetSequence[i])
          {
            isMatchSequence = false;
            break;
          }
        }

        if (isMatchSequence)
        {
          for (int i = 0; i < target.Length; i++)
          {
            int targetIndex = startIndex + targetSequence.Length + i;

            if (targetIndex < buffer.Length)
              target[i] = buffer[targetIndex];
          }
          int lastValue = buffer[startIndex + targetSequence.Length + target.Length];
          int checkSum = CommonCode.GetCheckSumLiftHeight(id, target);

          if (lastValue == checkSum)
          {
            if (id == (int)LiftType.FrontLift)
            {

              this.frontHeightByte = target;
              this.frontLiftHeight = CommonCode.ByteArrayToInt(target) * SerialSocket.BOTTOM_MOTOR_VAL;
              Debug.LogError($"{(LiftType)id} Height = {CommonCode.ByteArrayToInt(target)} / 변환 상수를 통한 최종 값 {this.frontLiftHeight}");
              Debug.LogError($"높이 Byte 배열 {target[0]}/ {target[1]} / {target[2]} / {target[3]}");

              

            }
            else if (id == (int)LiftType.BackLift)
            {
              this.backHeightByte = target; 
              this.backLiftHeight = CommonCode.ByteArrayToInt(target) * SerialSocket.BOTTOM_MOTOR_VAL;
              Debug.LogError($"{(LiftType)id} Height = {CommonCode.ByteArrayToInt(target)} / 변환 상수를 통한 최종 값 {this.backLiftHeight}");
              Debug.LogError($"높이 Byte 배열 {target[0]}/ {target[1]} / {target[2]} / {target[3]}");

            }
            else if (id == (int)LiftType.MainLift)
            {
              this.mainHeightByte = target;
              this.mainLiftHeight = CommonCode.ByteArrayToInt(target) * SerialSocket.MAIN_MOTOR_VAL;
              Debug.LogError($"{(LiftType)id} Height = {CommonCode.ByteArrayToInt(target)} / 변환 상수를 통한 최종 값 {this.mainLiftHeight}");
              Debug.LogError($"높이 Byte 배열  {target[0]}/ {target[1]} / {target[2]} / {target[3]}");
            }

          }
        }
      }

    }
  }
}
