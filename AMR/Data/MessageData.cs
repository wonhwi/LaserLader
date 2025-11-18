using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[Serializable]
public abstract class MessageBase
{
  public string type;
  public string command;
  public long time;

  protected MessageBase()
  {
    type = GetType().GetCustomAttribute<MessageTypeAttribute>()?.Type.ToString();
    command = GetType().GetCustomAttribute<MessageCommandAttribute>()?.Command.ToString();
    time = CommonCode.GetUTCTime();
  }
}

[AttributeUsage(AttributeTargets.Class)]
public class MessageTypeAttribute : Attribute
{
    public StatusType Type { get; }
    public MessageTypeAttribute(StatusType type) => Type = type;
}

[AttributeUsage(AttributeTargets.Class)]
public class MessageCommandAttribute : Attribute
{
    public CommandType Command { get; }
    public MessageCommandAttribute(CommandType command) => Command = command;
}


    
public partial class MessageData
{
  #region Status
  [MessageType(StatusType.status)]
  [MessageCommand(CommandType.status)]
  public class ReceiveStatusMessage : MessageBase
  {
    public AMRCondition condition;
    public AMRPower power;
    public AMRState state;
  }

  

  public class AMRCondition
  {
    public string auto_state;     //none, move, done //stop pause는 해당 API호출시 나오는거
    public string docking_state;  //docking_wait, docking_move, docking_success, docking_fail
    public float inlier_error;
    public float inlier_ratio;
    public float mapping_error;
    public float mapping_ratio;
    public string moving_state;   //none, move, auto, docking, manual
    public string obs_state;
  }

  public class AMRPower
  {
    public float bat_current;
    public float bat_in;
    public float bat_out;
    public float charge_current;
    public float contact_voltage;
    public float power;
    public float total_power;
    public int bat_per; //배터리 퍼센트
  }

  public class AMRState
  {
    public string charge;       //충전상태 (정확히는 충전케이블 연결 상
    public string emo;          //비상전원스위치 (true면 안눌림, falie)
    public string localization; //맵 상에서 로봇의 현재 위치 찾음
    public string map;          //맵 상에서 로봇의 현재 위치 찾음
    public string power;        //전원 인가 상태 (true면 모터단에 전원
  }
  #endregion




}
