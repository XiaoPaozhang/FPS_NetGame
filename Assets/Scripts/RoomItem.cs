using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
  public int ownerId;//玩家编号
  public bool isReady;//玩家是否准备

  void Start()
  {
    if (ownerId == PhotonNetwork.LocalPlayer.ActorNumber)
    {
      transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnReadyBtn);
    }
    else
    {
      transform.Find("Button").GetComponent<Image>().color = Color.black;
    }
  }

  public void OnReadyBtn()
  {
    isReady = !isReady;

    ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
    table.Add("isReady", isReady);

    PhotonNetwork.LocalPlayer.SetCustomProperties(table);//设置玩家自定义属性

    ChangeReady(isReady);
  }

  public void ChangeReady(bool isReady)
  {
    transform.Find("Button/Text").GetComponent<Text>().text = isReady ? "准备好了" : "还没准备好";
  }
}
