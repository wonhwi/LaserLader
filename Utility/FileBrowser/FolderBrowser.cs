using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolderBrowser : BaseFileBrowser
{
  public FolderBrowser(Action<string> onSuccess) : base(FileBrowser.PickMode.Folders, onSuccess : onSuccess)
  {

  }

}
