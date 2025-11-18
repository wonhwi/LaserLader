using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using static MessageData;

public class MessageHandler : MonoBehaviour
{
  private readonly JsonSerializerSettings _jsonSettings;

  public MessageHandler()
  {
    _jsonSettings = new JsonSerializerSettings
    {
      Converters = new List<JsonConverter> { new StringToValueConverter() }
    };
  }

  /// <summary>
  /// 이미지 데이터 수신
  /// </summary>
  /// <param name="data"></param>
  public void HandleByteArray(byte[] data)
  {
    if (data.Length < 12)
    {
      Debug.LogError("Data length is less than 12 bytes.");
      return;
    }

    // 앞의 12바이트를 4바이트씩 나누어 int로 변환
    int[] headerValues = new int[3];
    for (int i = 0; i < 3; i++)
    {
      headerValues[i] = BitConverter.ToInt32(data, i * 4);
    }

    int width = headerValues[0];
    int height = headerValues[1];
    int channels = headerValues[2];

    // 나머지 바이트를 이미지 데이터로 사용
    byte[] imageData = new byte[data.Length - 12];
    Array.Copy(data, 12, imageData, 0, imageData.Length);

    // 추출한 int 값과 이미지 데이터 사용
    //Debug.Log($"Header Values: {headerValues[0]}, {headerValues[1]}, {headerValues[2]}");

    DeviceManager.Instance.AMRStatus.UpdateDisplay(imageData, width, height, channels);
  }

  /// <summary>
  /// 주기적 상태 데이터 수신
  /// </summary>
  /// <param name="messageData"></param>
  public void HandleMessage(string messageData)
  {
    //Debug.LogError(messageData);
    JObject jObject = JObject.Parse(messageData);

    string type = (string)jObject["type"];
    string command = (string)jObject["command"];

    //1초마다 주기적으로 받아오는 데이터
    if (type == StatusType.status.ToString() && command == CommandType.status.ToString())
    {
      ReceiveStatusMessage data = JsonConvert.DeserializeObject<ReceiveStatusMessage>(jObject.ToString(), _jsonSettings);
      MessageBroker.Default.Publish(data);
    }
    else if(type == StatusType.error.ToString())
    {
      ReceiveError data = JsonConvert.DeserializeObject<ReceiveError>(jObject.ToString(), _jsonSettings);
      MessageBroker.Default.Publish(data);
    }  
  }
}
