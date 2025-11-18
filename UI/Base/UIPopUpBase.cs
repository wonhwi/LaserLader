using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public abstract class UIPopUpBase<T> : UICanvasBase<T> where T : class
{
  

  [Header("[UI Component]")]
  [SerializeField] protected Image notificationImage;
  [SerializeField] protected TextMeshProUGUI messageText;

  [SerializeField] protected Button confirmButton;
  [SerializeField] protected Button cancelButton;
  [SerializeField] protected Button closeButton;

  protected virtual void Awake()
  {
    if (closeButton != null)
      closeButton.onClick.AddListener(OnClickButtonClose);
  }

  public void OpenNotice(NotificationType notificationType, string message)
  {
    notificationImage.sprite = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI_NOTIFICATION_ICON, notificationType.ToString());
    messageText.text = message;

    confirmButton.onClick.RemoveAllListeners();
    confirmButton.onClick.AddListener(OnClickButtonClose);

    SetActiveButton(confirmButton, true);
    SetActiveButton(cancelButton, false);

    base.Open();
  }


  public void OpenGeneral(NotificationType notificationType, string message, UnityAction confirmAction)
  {
    notificationImage.sprite = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI_NOTIFICATION_ICON, notificationType.ToString());

    messageText.text = message;

    confirmButton.onClick.RemoveAllListeners();
    confirmButton.onClick.AddListener(() => { 
      confirmAction?.Invoke();
      OnClickButtonClose();
    });

    cancelButton.onClick.RemoveAllListeners();
    cancelButton.onClick.AddListener(OnClickButtonClose);

    SetActiveButton(confirmButton, true);
    SetActiveButton(cancelButton, true);

    base.Open();

  }

  public void OpenGeneral(NotificationType notificationType, string message, UnityAction confirmAction = null, UnityAction cancelAction = null)
  {
    notificationImage.sprite = ResourceManager.Instance.LoadSprite(ResourcePath.PATH_UI_NOTIFICATION_ICON, notificationType.ToString());

    messageText.text = message;

    confirmButton.onClick.RemoveAllListeners();
    confirmButton.onClick.AddListener(() => {
      confirmAction?.Invoke();
      OnClickButtonClose();
    });

    cancelButton.onClick.RemoveAllListeners();
    cancelButton.onClick.AddListener(() => {
      cancelAction?.Invoke();
      OnClickButtonClose();
    });

    SetActiveButton(confirmButton, true);
    SetActiveButton(cancelButton, true);

    base.Open();

  }


  private void SetActiveButton(Button button, bool setActive)
  {
    if (button != null)
      button.gameObject.SetActive(setActive);
  }

  public void OnClickButtonClose()
  {
    base.Close();
  }
}
