using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using System.Threading;
using System.Text;
using static MessageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;

public class WebSocketClient : Singleton<WebSocketClient>
{
  public WebSocketConnection m_WebSocketConnection;

  private MessageHandler _messageHandler;

  private void Awake()
  {
    MessageBroker.Default.Receive<ReceiveMoveGoal>().Subscribe(OnMoveGoal);
    MessageBroker.Default.Receive<ReceiveLoadMap>().Subscribe(OnMapLoad);
  }

  public void OnMoveGoal(ReceiveMoveGoal message){Debug.LogError("ReceiveMoveGoal");}
  public void OnMapLoad(ReceiveLoadMap message) { Debug.LogError("ReceiveLoadMap"); }

  private void Start()
  {
    InitializeWebSocket();
  }

  private void InitializeWebSocket()
  {
    Debug.LogError("AMR 연결 시도");
    _messageHandler = new MessageHandler();

    m_WebSocketConnection.Initialize(
        onMessage : (e) => MainThreadDispatcher.Enqueue(() => HandleMessage(e)),
        onOpen    : ( ) => Debug.Log("연결 성공"),
        onClose   : (_) => Debug.Log("연결이 종료되었습니다")
    );

    m_WebSocketConnection.Connect();
  }

  private void HandleMessage(MessageEventArgs e)
  {
    if (e.IsBinary)
    {
      _messageHandler.HandleByteArray(e.RawData);
    }
    else
    {
      _messageHandler.HandleMessage(e.Data);

    }
  }

  //서버로 데이터 전송할 함수
  public void SendMessage(string msg)
  {
    m_WebSocketConnection.SendMessage(msg);
  }

  public bool IsConnected()
    => m_WebSocketConnection.IsConnected;


  private void OnApplicationQuit()
  {
    if(m_WebSocketConnection.IsConnected)
      m_WebSocketConnection.DisconncectServer();
  }
}