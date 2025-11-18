using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabMapPosition : PrefabMapPoint
{
  public override void SetSelectImage()
  {
    pointImage.sprite 
      = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI, ConstantManager.MapPointPositionDefaultSprite);
  }

  public override void SetOriginImage()
  {
    pointImage.sprite
      = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI, ConstantManager.MapPointPositionSelectSprite);
  }
}
