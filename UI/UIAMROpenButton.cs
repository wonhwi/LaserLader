using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAMROpenButton : MonoBehaviour
{
  private void Awake()
  {
    this.GetComponent<Button>().onClick.AddListener(OnClickMaximizeAMRProgramUI);
  }
  private void OnClickMaximizeAMRProgramUI()
  {
    CommonCode.OpenAMRUI();
  }

}
