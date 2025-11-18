using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FilesBrowser : BaseFileBrowser
{
  public FilesBrowser(FileBrowser.Filter filter, Action<string> onSuccess) 
    : base(FileBrowser.PickMode.Files, filter, onSuccess)
  {

  }
}
