using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabUI : MonoBehaviour
{
  public virtual void OnActive()
  {
    this.gameObject.SetActive(true);
  }

  public virtual void OnDeActive()
  {
    this.gameObject.SetActive(false);
  }
}
