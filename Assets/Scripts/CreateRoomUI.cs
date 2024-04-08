using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviourPunCallbacks
{
  InputField roomNameInputField;//房间名称
  void Start()
  {
    transform.Find("bg/title/closeBtn").GetComponent<Button>().onClick.AddListener(OnCloseBtn);
    transform.Find("bg/okBtn").GetComponent<Button>().onClick.AddListener(OnCreateBtn);
    roomNameInputField = transform.Find("bg/InputField").GetComponent<InputField>();

    //随机一个房间名称
    roomNameInputField.text = "room_" + Random.Range(1000, 9999);
  }

  public void OnCreateBtn()
  {
    Game.uIManager.ShowUI<MaskUI>("MaskUI").ShowMsg("创建房间中...");
    RoomOptions roomOptions = new RoomOptions()
    {
      MaxPlayers = 2,
    };
    //1.房间名称 2.房间参数对象
    PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
  }

  public void OnCloseBtn()
  {
    Game.uIManager.CloseUI(gameObject.name);
  }

  #region PUN Callbacks
  //创建房间成功
  public override void OnCreatedRoom()
  {
    base.OnCreatedRoom();

    Debug.Log("创建房间成功");
    Game.uIManager.CloseAllUI();

    //显示进入房间ui
    Game.uIManager.ShowUI<RoomUI>("RoomUI");
  }

  //创建失败
  public override void OnCreateRoomFailed(short returnCode, string message)
  {
    base.OnCreateRoomFailed(returnCode, message);

    Game.uIManager.CloseUI("MaskUI");
  }

  #endregion
}
