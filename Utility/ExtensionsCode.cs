using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public static class ExtensionsCode
{
  public static int GetDropDownValueIndex(this TMP_Dropdown dropDown, string value)
  {
    return dropDown.options.FindIndex(option => option.text == value);
  }

  public static void SetValueWithoutNotify(this TMP_Dropdown dropDown, string value)
  {
    int findIndex = GetDropDownValueIndex(dropDown, value);

    dropDown.SetValueWithoutNotify(findIndex);
  }

  public static void SetValue(this TMP_Dropdown dropDown, string value)
  {
    int findIndex = GetDropDownValueIndex(dropDown, value);

    dropDown.onValueChanged?.Invoke(findIndex);
  }


  public static void ChangeTMPMaterial(this TextMeshProUGUI text, string materialPath)
  {
    text.fontMaterial = ResourceManager.Instance.LoadResource<Material>(materialPath);
  }

}
