using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MessageData
{
  public class ReceiveBase : MessageBase
  {
    public string result;
  }

  #region Move
  [MessageType(StatusType.move)]
  [MessageCommand(CommandType.goal)]
  public class ReceiveMoveGoal : ReceiveBase
  {
    public string id;
    public string message;
    public string method;
    public int preset;
  }

  [MessageType(StatusType.move)]
  [MessageCommand(CommandType.pause)]
  public class ReceiveMovePause : ReceiveBase { }

  [MessageType(StatusType.move)]
  [MessageCommand(CommandType.resume)]
  public class ReceiveMoveResume : ReceiveBase
  {
  }

  [MessageType(StatusType.move)]
  [MessageCommand(CommandType.stop)]
  public class ReceiveMoveStop : ReceiveBase
  {
  }
  #endregion


  #region Map Load
  [MessageType(StatusType.mapLoad)]
  [MessageCommand(CommandType.status)]
  public class ReceiveLoadMap
  {
    public string name;
    public string type;
    public long time;
    public string result;
  }

  public class ReceiveError
  {
    public string error_type;
    public long time;
    public string type;

  }

  #endregion

}
