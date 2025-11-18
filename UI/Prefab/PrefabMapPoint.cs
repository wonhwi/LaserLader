using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public abstract class PrefabMapPoint : PrefabUI
{
  private MeasurementMapData measurementMapData;
  public MapPoint mapPoint = new MapPoint();

  [SerializeField] protected Image pointImage;
  [SerializeField] private TextMeshProUGUI pointNameText;
  [SerializeField] private RectTransform rect;
  private RectTransform parentRect;

  private void Awake()
  {
    AddDragFunctionality(this.gameObject);

    measurementMapData = CanvasMeasurementSetting.Instance.measurementMapData;
  }

  /// <summary>
  /// 활성화/비활성화 상태 변경 함수
  /// </summary>
  public virtual void SetSelectImage() { }

  public virtual void SetOriginImage() { }

  public void SetPoint(MapPoint mapPoint, RectTransform parent)
  {
    string[] name = mapPoint.pointName.Split('_');

    this.transform.SetParent(parent);
    this.mapPoint = mapPoint;
    this.parentRect = parent;
    this.pointNameText.text = $"{name[0]}<br>{name[1]}";
    this.rect.anchoredPosition = new Vector2(
      mapPoint.x * parent.rect.width  / ConstantManager.MapImageWidth, 
      mapPoint.y * parent.rect.height / ConstantManager.MapImageHeight
    );

    base.OnActive();
  }


  void AddDragFunctionality(GameObject imageObject)
  {
    EventTrigger trigger = imageObject.AddComponent<EventTrigger>();

    EventTrigger.Entry dragEntry = new EventTrigger.Entry();
    dragEntry.eventID = EventTriggerType.Drag;
    dragEntry.callback.AddListener((data) => {
      //if(measurementMapData.isEditMode)
        OnDrag((PointerEventData)data, imageObject); 
    });
    trigger.triggers.Add(dragEntry);
  }

  void OnDrag(PointerEventData data, GameObject imageObject)
  {
    Vector2 localPoint;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, data.position, data.pressEventCamera, out localPoint);

    // Calculate the bounds of the parent
    Vector2 minBounds = parentRect.rect.min;
    Vector2 maxBounds = parentRect.rect.max;

    // Clamp the position to the bounds of the parent
    localPoint.x = Mathf.Clamp(localPoint.x, minBounds.x, maxBounds.x);
    localPoint.y = Mathf.Clamp(localPoint.y, minBounds.y, maxBounds.y);

    rect.anchoredPosition = localPoint;

    mapPoint.x = Mathf.Round(localPoint.x * 100) * 0.01f;
    mapPoint.y = Mathf.Round(localPoint.y * 100) * 0.01f;
  }

  public override void OnDeActive()
  {
    SetOriginImage();
    base.OnDeActive();
  }

}
