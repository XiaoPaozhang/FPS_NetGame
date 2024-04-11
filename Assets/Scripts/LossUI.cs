using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LossUI : MonoBehaviour
{
  public UnityAction onClickCallback;

  void Start()
  {
    transform.Find("resetBtn").GetComponent<Button>().onClick.AddListener(OnClickBtn);
  }

  private void OnClickBtn()
  {
    if (onClickCallback != null)
    {
      onClickCallback();
    }
    Game.uIManager.CloseUI(gameObject.name);
  }
}
