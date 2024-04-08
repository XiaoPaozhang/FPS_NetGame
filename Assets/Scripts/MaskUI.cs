using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//tips界面
public class MaskUI : MonoBehaviour
{
  public void ShowMsg(string msg)
  {
    transform.Find("msg/bg/Text").GetComponent<Text>().text = msg;
  }
}
