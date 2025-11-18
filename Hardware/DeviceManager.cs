using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceManager : Singleton<DeviceManager>
{
  [Header("[Device Status]")]
  public DeviceAMRStatus AMRStatus;         //AMR 상태
  public DeviceLADARStatus LADARStatus;     //측정기 상태
  public DeviceLiftStatus LiftStatus;       // Lift

}
