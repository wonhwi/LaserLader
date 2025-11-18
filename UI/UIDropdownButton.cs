using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDropdownButton : MonoBehaviour
{
  [SerializeField] private Button deleteButton;

  private void Awake()
  {
    deleteButton.onClick.AddListener(OnClickDeleteButton);
  }


  private void OnClickDeleteButton()
  {
    CanvasPopUp.Instance.OpenGeneral(
     NotificationType.Infomation,
     "프리셋을 삭제하시겠습니까?",
     () =>
     {
       CanvasMeasurementSetting.Instance.DeletePreset(this.transform.GetSiblingIndex() - 1);
     });


    

  }
}
