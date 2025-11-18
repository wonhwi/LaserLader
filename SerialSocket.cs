using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UniRx;
using Debug = UnityEngine.Debug;

public class SerialSocket
{
  public SerialPort ioSocket;
  public static readonly double MAIN_MOTOR_VAL = 0.0888888f; // 메인 모터의 값 변환 상수
  public static readonly double BOTTOM_MOTOR_VAL = 0.025f;   // 하단 모터의 값 변환 상수
         
  public static readonly int FRONT_BOTTOM_MOTOR_ID = 0; // 전면 하단 모터 ID
  public static readonly int BACK_BOTTOM_MOTOR_ID = 1;  // 후면 하단 모터 ID
  public static readonly int MAIN_MOTOR_ID = 2;         // 메인 모터 ID

  public SerialSocket()
  {
    // 시리얼 포트 초기화
    ioSocket = new SerialPort
    {
      BaudRate = 19200, // 보드레이트 설정
      DataBits = 8,     // 데이터 비트 설정
      StopBits = StopBits.One, // 스톱 비트 설정
      Parity = Parity.None     // 패리티 설정
    };
  }

  public void Close()
  {
    if (ioSocket != null && ioSocket.IsOpen)
    {
      ioSocket.Close(); // 포트가 열려 있으면 닫습니다.
    }
  }

  // 지정된 포트 이름으로 시리얼 포트를 엽니다.
  public async void Open(string port)
  {
    Debug.LogError($"SerialPort Open : {port}");
    if (ioSocket.IsOpen)
    {
      ioSocket.Close(); // 포트가 열려 있으면 닫습니다.
    }
    ioSocket.PortName = port; // 포트 이름 설정

    try
    {

      ioSocket.Open(); // 포트 열기 시도
      Debug.LogError("Serial socket connection succeeded.");// 연결 성공 메시지

      ManagerLog.AddLog(LogType.Log, "HMI 프로그램 초기 설정 진행 중...");
      ManagerLog.AddLog(LogType.Log, "HMI 실행으로 인한 API, 고정 리프트 Home 실행");
      ManagerTransport.Instance.Lift_AllHome();
      await UniTask.Delay(5000);

#if !UNITY_EDITOR
      DeviceManager.Instance.LiftStatus.SetLiftType(LiftType.MainLift);
      await DeviceManager.Instance.LiftStatus.WaitHome(LiftType.MainLift);

      DeviceManager.Instance.LiftStatus.SetLiftType(LiftType.FrontLift);
      await DeviceManager.Instance.LiftStatus.WaitHome(LiftType.FrontLift);

      DeviceManager.Instance.LiftStatus.SetLiftType(LiftType.BackLift);
      await DeviceManager.Instance.LiftStatus.WaitHome(LiftType.BackLift);
#endif
      
      ManagerLog.AddLog(LogType.Log, $"리프트 위치를 기본 설정 위치로 이동 합니다.");

#if !UNITY_EDITOR
      await ManagerTransport.Instance.Lift_MoveMain(ConstantManager.APILiftHomeHeight);
      await ManagerTransport.Instance.Lift_MoveSide(ConstantManager.LiftDefaultHeight, ConstantManager.LiftDefaultHeight);
#endif

      ManagerLog.AddLog(LogType.Log, "HMI 프로그램 초기 설정이 완료 되었습니다.");

      CanvasMain.Instance.mainUI.measureStartButton.interactable = true;

    }
    catch (UnauthorizedAccessException)
    {
      CanvasErrorPopUp.Instance.OpenNotice($"포트 {port}에 접근할 수 없습니다.<br>다른 프로세스에서 사용 중일 수 있습니다.<br>해당 프로그램을 종료 후 HMI를 재시작 해주세요");
      Debug.LogError($"포트 {port}에 접근할 수 없습니다.<br>다른 프로세스에서 사용 중일 수 있습니다.");
      return;
    }
    catch (IOException ex)
    {
      CanvasErrorPopUp.Instance.OpenNotice($"포트 {port}를 열 수 없습니다.<br>장치관리자에서 Port를 확인 후 우측 상단 설정에서<br>포트를 다시 설정하고 저장을 눌러주세요.<br>그 후 HMI를 재시작해주세요.<br>{ex.Message}");
      Debug.LogError($"포트 {port}를 열 수 없습니다. 오류: {ex.Message}");
      return;
    }
    catch (Exception ex)
    {
      CanvasErrorPopUp.Instance.OpenNotice($"알 수 없는 오류가 발생했습니다<br>{ex.Message}");
      Debug.LogError($"알 수 없는 오류가 발생했습니다: {ex.Message}");
      return;
    }
    
  }

  public void MotorBreak(int id)
  {
    if (id < FRONT_BOTTOM_MOTOR_ID || id > MAIN_MOTOR_ID)
    {
      Debug.Log("Invalid id."); // 잘못된 ID 메시지
      return;
    }

    if (!ioSocket.IsOpen)
    {
      Debug.Log("Serial socket not connected."); // 연결되지 않은 경우 메시지
      return;
    }

    byte[] cmd = new byte[6];
    cmd[0] = 0xb7; // 명령 시작 바이트
    cmd[1] = 0xb8; // 명령 시작 바이트
    cmd[2] = (byte)id; // 모터 ID
    cmd[3] = 0x0a; // 명령 코드
    cmd[4] = 0x01; // 데이터 길이
    cmd[5] = 0x04; // 종료 바이트

    byte[] checkSum = CheckSum(cmd);

    ioSocket.Write(checkSum, 0, checkSum.Length); // 명령 전송
    Debug.Log($"Motor id:{id}, start break"); // 홈 시작 메시지
  }

  // 모터를 절대 위치로 이동시킵니다.
  public void MovePosition(int id, int val)
  {
    if (id < FRONT_BOTTOM_MOTOR_ID || id > MAIN_MOTOR_ID)
    {
      Debug.Log("Invalid id."); // 잘못된 ID 메시지
      return;
    }

    if (!ioSocket.IsOpen)
    {
      Debug.Log("Serial socket not connected."); // 연결되지 않은 경우 메시지
      return;
    }

    byte[] cmd = new byte[9];
    cmd[0] = 0xb7; // 명령 시작 바이트
    cmd[1] = 0xb8; // 명령 시작 바이트
    cmd[2] = (byte)id; // 모터 ID
    cmd[3] = 0xf3; // 명령 코드
    cmd[4] = 0x04; // 데이터 길이

    // val 값을 바이트 배열로 변환하여 cmd에 추가
    byte[] valBytes = BitConverter.GetBytes(val);
    Array.Copy(valBytes, 0, cmd, 5, 4); // 6번째 바이트부터 val 값을 복사

    byte[] checkSum = CheckSum(cmd);

    ioSocket.Write(checkSum, 0, checkSum.Length); // 명령 전송
  }

  // 모터를 증가적으로 이동시킵니다.
  public void MovePositionIncremental(int id, int val)
  {
    if (id < FRONT_BOTTOM_MOTOR_ID || id > MAIN_MOTOR_ID)
    {
      Debug.Log("Invalid id."); // 잘못된 ID 메시지
      return;
    }

    if (!ioSocket.IsOpen)
    {
      Debug.Log("Serial socket not connected."); // 연결되지 않은 경우 메시지
      return;
    }

    byte[] cmd = new byte[9];
    cmd[0] = 0xb7; // 명령 시작 바이트
    cmd[1] = 0xb8; // 명령 시작 바이트
    cmd[2] = (byte)id; // 모터 ID
    cmd[3] = 0xf4; // 명령 코드
    cmd[4] = 0x04; // 데이터 길이

    // val 값을 바이트 배열로 변환하여 cmd에 추가
    byte[] valBytes = BitConverter.GetBytes(val);
    Array.Copy(valBytes, 0, cmd, 5, 4); // 6번째 바이트부터 val 값을 복사

    byte[] checkSum = CheckSum(cmd);

    ioSocket.Write(checkSum, 0, checkSum.Length); // 명령 전송
  }

  /// <summary>
  /// 모터를 홈 위치로 이동시킵니다.
  /// 리프트 장비가 꺼졌을때 실행해줘야함
  /// </summary>
  /// <param name="id"></param>
  public void Homing(int id)
  {
    if (id < FRONT_BOTTOM_MOTOR_ID || id > MAIN_MOTOR_ID)
    {
      Debug.Log("Invalid id."); // 잘못된 ID 메시지
      return;
    }

    if (!ioSocket.IsOpen)
    {
      Debug.Log("Serial socket not connected."); // 연결되지 않은 경우 메시지
      return;
    }

    byte[] cmd = new byte[6];
    cmd[0] = 0xb7; // 명령 시작 바이트
    cmd[1] = 0xb8; // 명령 시작 바이트
    cmd[2] = (byte)id; // 모터 ID
    cmd[3] = 0x0a; // 명령 코드
    cmd[4] = 0x01; // 데이터 길이
    cmd[5] = 0x5a; // 종료 바이트

    byte[] checkSum = CheckSum(cmd);

    ioSocket.Write(checkSum, 0, checkSum.Length); // 명령 전송
    Debug.Log($"Motor id:{id}, start homing"); // 홈 시작 메시지
  }

  public void GetLiftHeight(int id)
  {
    if (id < FRONT_BOTTOM_MOTOR_ID || id > MAIN_MOTOR_ID)
    {
      Debug.Log("Invalid id."); // 잘못된 ID 메시지
      return;
    }
    
    if (!ioSocket.IsOpen)
    {
      Debug.Log("Serial socket not connected."); // 연결되지 않은 경우 메시지
      return;
    }

    byte[] cmd = new byte[6];

    cmd[0] = 0xb7; // 명령 시작 바이트
    cmd[1] = 0xb8; // 명령 시작 바이트
    cmd[2] = (byte)id; // 모터 ID
    cmd[3] = 0x04; // 명령 코드
    cmd[4] = 0x01; // 데이터 길이
    cmd[5] = 0xc5; // 종료 바이트

    byte[] checkSum = CheckSum(cmd);

    ioSocket.Write(checkSum, 0, checkSum.Length); // 명령 전송
    //Debug.Log($"Motor id:{id}, get Height"); // 홈 시작 메시지
  }

  // 명령의 체크섬을 계산하여 데이터의 무결성을 확인합니다.
  private byte[] CheckSum(byte[] data)
  {
    byte cs = 0;
    foreach (var b in data)
    {
      cs += b; // 모든 바이트의 합 계산
    }
    cs = (byte)(~cs + 1); // 체크섬 계산
    Array.Resize(ref data, data.Length + 1); // 데이터 배열 크기 증가
    data[data.Length - 1] = cs; // 체크섬 추가
    return data; // 체크섬이 추가된 데이터 반환
  }
}