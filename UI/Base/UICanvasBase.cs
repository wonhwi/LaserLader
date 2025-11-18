using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasBase<T> : Singleton<T> where T : class
{
  public virtual void Open()
  {
    this.gameObject.SetActive(true);
  }
  public virtual void Close()
  {
    this.gameObject.SetActive(false);
  }
}
