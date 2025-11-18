using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonHighLight : UISelectable
{
  public Button button;
  public Animator animator;

  public void SetHighlight(bool isOn)
  {
    base.SetBlockRaycasts(isOn);

    animator.SetInteger("OnHighLight", isOn ? 1 : 0);
  }
}
