using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabMapRP : PrefabMapPoint
{
  public override void SetSelectImage()
  {
    pointImage.sprite
      = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI, ConstantManager.MapPointRPDefaultSprite);
  }

  public override void SetOriginImage()
  {
    pointImage.sprite
      = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI, ConstantManager.MapPointRPSelectSprite);
  }
}
