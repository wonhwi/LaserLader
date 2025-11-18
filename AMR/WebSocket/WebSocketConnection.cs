using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;

public class WebSocketConnection : MonoBehaviour
{
  [SerializeField] private string IP = "127.0.0.1";
  [SerializeField] private string PORT = "13360";

  private string _url;
  private WebSocket _socket;
  private readonly float _reconnectTime = 3f;
  public bool IsConnected => _socket == null ? false : (_socket.IsAlive ? true : false);

  private Action<MessageEventArgs> _onMessage;
  private Action _onOpen;
  private Action<CloseEventArgs> _onClose;

  private bool _isReconnecting = false;

  public void Initialize(Action<MessageEventArgs> onMessage, Action onOpen, Action<CloseEventArgs> onClose)
  {
    _url = $"ws://{this.IP}:{this.PORT}";

    _onMessage = onMessage;
    _onOpen = onOpen;
    _onClose = onClose;

    InitializeSocket();
  }

  private void InitializeSocket()
  {
    try
    {
      _socket = new WebSocket(_url);
      _socket.OnMessage += (sender, e) => _onMessage(e);
      _socket.OnOpen += (sender, e) => _onOpen();
      _socket.OnClose += (sender, e) =>
      {
        _onClose(e);
        HandleDisconnect();
      };
    }
    catch (Exception ex)
    {
      Debug.LogError($"웹소켓 초기화 실패: {ex.Message}");
    }
  }

  private bool isTryReconnect = false;

  private async void ReconnectSocket()
  {
    isTryReconnect = true;

    Connect();

    await UniTask.Delay(2000);

    if (_socket.IsAlive)
    {
      Debug.Log("재연결 성공!");
      isTryReconnect = false;
    }
    else
    {
      Debug.Log("재연결 실패!");

      ReconnectSocket();
    }
  }

  private void HandleDisconnect()
  {
#if !UNITY_EDITOR
    if(!isTryReconnect)
    {
      Debug.Log($"{this.IP} : {this.PORT}통신 연결이 끊어졌습니다. 재연결을 시도합니다...");

      ReconnectSocket();
    }
#endif

  }

  public void Connect()
  {
    try
    {
      if (_socket == null || !_socket.IsAlive)
        _socket.Connect();

    }
    catch (Exception e)
    {
      Debug.Log(e.ToString());
    }
  }

  public void DisconncectServer()
  {
    try
    {

      if (_socket == null)
        return;

      if (_socket.IsAlive)
        _socket.Close();


    }
    catch (Exception e)
    {
      Debug.Log(e.ToString());
    }
  }

  public void SendMessage(string msg)
  {
    if (!_socket.IsAlive)
    {
      Debug.LogError("소켓이 현재 Alive상태가 아니라 Return");
      return;
    }
      
    try
    {

      _socket.Send(msg);

    }
    catch (Exception ex)
    {
      Debug.LogError($"예기치 않은 오류: {ex.Message}");
    }

  }

}
