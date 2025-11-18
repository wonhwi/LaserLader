using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BundlePreset : MonoBehaviour
{
  public TextMeshProUGUI presetText;
  public TextMeshProUGUI projectText;
  public TextMeshProUGUI partText;
  public TextMeshProUGUI positionText;
  public TextMeshProUGUI modeText;

  public void SetBundlePreset(PresetData presetData, string positionName)
  {
    presetText.text = presetData.presetName;
    projectText.text = presetData.projectName;
    partText.text = presetData.partName;
    positionText.text = positionName;
    modeText.text = presetData.isAuto ? "자동" : "수동";

  }

  public void SetBundlePosition(string positionName)
  {
    positionText.text = positionName;
  }
}
