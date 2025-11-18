using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleSprite : MonoBehaviour
{
  public Toggle toggle;
  public Image targetImage;

  [Space(10f)]
  public string onSpirteName;
  public string offSpirteName;

  private void Awake()
  {
    toggle.onValueChanged.AddListener(OnValueChange);
  }

  private void OnValueChange(bool isOn)
  {
    targetImage.sprite = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI, isOn ? onSpirteName : offSpirteName);
  }
}
