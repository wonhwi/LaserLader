using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public static class LiftController
{
  public static SerialSocket socket;

  public static void ConnetSerialPort(string portName)
  {
    socket?.Close();

    socket = new SerialSocket();

    if (!string.IsNullOrEmpty(portName))
    {
      socket.Open(portName);
      DeviceManager.Instance.LiftStatus.serialSocket = socket;
    }
    else
      Debug.LogError($"{portName} 올바르지 않은 PortName");

  }

  public static async UniTask BreakSide()
  {
    socket.MotorBreak(SerialSocket.FRONT_BOTTOM_MOTOR_ID);
    await UniTask.Delay(500);
    socket.MotorBreak(SerialSocket.BACK_BOTTOM_MOTOR_ID);
  }

  public static async UniTask BreakMain()
  {
    socket.MotorBreak(SerialSocket.MAIN_MOTOR_ID);

  }

  public static async UniTask MoveSide(int frontValue, int backValue)
  {
    double frontVal = frontValue / SerialSocket.BOTTOM_MOTOR_VAL;

    socket.MovePosition(SerialSocket.FRONT_BOTTOM_MOTOR_ID, (int)frontVal);
    Debug.Log($"motor id:{SerialSocket.FRONT_BOTTOM_MOTOR_ID}, move abs position:{frontValue}mm");

    await UniTask.Delay(500);

    double backVal = backValue / SerialSocket.BOTTOM_MOTOR_VAL;
    socket.MovePosition(SerialSocket.BACK_BOTTOM_MOTOR_ID, (int)backVal);
    Debug.Log($"motor id:{SerialSocket.BACK_BOTTOM_MOTOR_ID}, move abs position:{backValue}mm");
    
    await UniTask.Delay(100);
  }

  public static void MoveMain(int heightValue)
  {
    double val = heightValue / SerialSocket.MAIN_MOTOR_VAL;

    socket.MovePosition(SerialSocket.MAIN_MOTOR_ID, (int)val);
    Debug.Log($"motor id:{SerialSocket.MAIN_MOTOR_ID}, move abs position:{heightValue}mm");
  }

  public static void MoveIncMain(int heightValue)
  {
    double val = heightValue / SerialSocket.MAIN_MOTOR_VAL;

    socket.MovePositionIncremental(SerialSocket.MAIN_MOTOR_ID, (int)val);
    Debug.Log($"motor id:{SerialSocket.MAIN_MOTOR_ID}, move abs position:{heightValue}mm");
  }

  public static void GetLiftHeight(int id)
  {
    socket.GetLiftHeight(id);
  }

  public static async void AllHome()
  {
    socket.Homing(SerialSocket.FRONT_BOTTOM_MOTOR_ID);
    await UniTask.Delay(200);
    socket.Homing(SerialSocket.BACK_BOTTOM_MOTOR_ID);
    await UniTask.Delay(200);
    socket.Homing(SerialSocket.MAIN_MOTOR_ID);
  }

  // 비상 정지 명령 실행
  public static async void LiftEMO()
  {
    socket.MovePositionIncremental(SerialSocket.FRONT_BOTTOM_MOTOR_ID, 0);
    await UniTask.Delay(200);
    socket.MovePositionIncremental(SerialSocket.BACK_BOTTOM_MOTOR_ID, 0);
    await UniTask.Delay(200);
    socket.MovePositionIncremental(SerialSocket.MAIN_MOTOR_ID, 0);

  }


}
