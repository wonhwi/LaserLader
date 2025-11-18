using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PrefabPositionToggle : PrefabUI
{
  public Toggle toggle;
  public TextMeshProUGUI toggleText;
  public Button toggleTextButton;

  public Action<PrefabPositionToggle> OnSelectToggle;

  public void Awake()
  {
    toggleTextButton.onClick.AddListener(() => OnSelectToggle?.Invoke(this));
  }

  public void SetSelect(bool isSelect)
  {
    toggleText.color = isSelect ? Color.red : Color.white;
  }

  public override void OnDeActive()
  {
    base.OnDeActive();

    OnSelectToggle = null;
    SetSelect(false);
  }

  public bool IsActivate()
    => toggle.isOn;

  public string GetToggleText()
    => toggleText.text;

  public void SetToggleText(string toggleValue)
  {
    toggleText.text = toggleValue;
  }

  public void SetToggle(bool isOn)
  {
    toggle.isOn = isOn;
  }
}
