using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public partial class MessageData
{
  #region Move
  [MessageType(StatusType.move)]
  [MessageCommand(CommandType.target)]
  public class SendMoveTarget : MessageBase
  {
    public float x;
    public float y;
    public float z;
    public float rz;
    public int preset;
    public string method;
  }

  [MessageType(StatusType.move)]
  [MessageCommand(CommandType.station)]
  public class SendMoveStation : MessageBase
  {
    public float docking_offset_th;
    public float docking_offset_x;
    public float docking_offset_y;
    public string id;
    public string method;
    public int preset;

    public SendMoveStation(string mapNode)
    {
      this.docking_offset_th = 0f;
      this.docking_offset_x = 0f;
      this.docking_offset_y = 0f;
      this.id = mapNode;
      this.method = "hpp";
      this.preset = 0;
    }

  }

  [MessageType(StatusType.move)]
  [MessageCommand(CommandType.goal)]
  public class SendMoveGoal : MessageBase
  {
    public float docking_offset_th;
    public float docking_offset_x;
    public float docking_offset_y;
    public string id;
    public string method;
    public int preset;

    public SendMoveGoal(string mapNode)
    {
      this.docking_offset_th = 0f;
      this.docking_offset_x = 0f;
      this.docking_offset_y = 0f;
      this.id = mapNode;
      this.method = "hpp";
      this.preset = 0;
    }
  }

  [MessageType(StatusType.move)]
  [MessageCommand(CommandType.pause)]
  public class SendMovePause : MessageBase { }

  [MessageType(StatusType.move)]
  [MessageCommand(CommandType.resume)]
  public class SendMoveResume : MessageBase { }

  [MessageType(StatusType.move)]
  [MessageCommand(CommandType.stop)]
  public class SendMoveStop : MessageBase { }
  #endregion


  #region Map Load
  [MessageType(StatusType.mapLoad)]
  public class SendMapLoad
  {
    public string name;
    public string type;
    public long time;

    public SendMapLoad(string mapName)
    {
      this.name = mapName;
      this.type = "mapload";
      this.time = CommonCode.GetUTCTime();
    }
  }

  #endregion


}
