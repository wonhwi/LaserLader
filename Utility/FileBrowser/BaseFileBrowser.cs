using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.IO;
using System.Collections;
using SimpleFileBrowser;

public abstract class BaseFileBrowser
{
  private FileBrowser.PickMode pickMode;
  private FileBrowser.Filter filter;

  private Action<string> OnSuccess;

  public BaseFileBrowser(FileBrowser.PickMode pickMode, FileBrowser.Filter filter = null, Action<string> onSuccess = null)
  {
    this.pickMode = pickMode;
    this.filter = filter;
    this.OnSuccess = onSuccess;
  }

  public virtual void LoadDiaLog()
  {
    if(filter != null)
      FileBrowser.SetFilters(false, filter);

    FileBrowser.ShowLoadDialog(
      (paths) => OnSuccess?.Invoke(paths[0]),
      null,
      pickMode, 
      title: "Select Folder", 
      loadButtonText: "Select");
  }
}
