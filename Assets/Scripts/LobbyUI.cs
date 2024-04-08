using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;

//大厅UI
public class LobbyUI : MonoBehaviourPunCallbacks
{
  Timer _timer;
  TypedLobby fpsLobby;//大厅对象
  private Transform contentTf;
  private GameObject roomItemPrefab;

  void Start()
  {
    transform.Find("content/title/closeBtn").GetComponent<Button>().onClick.AddListener(OnCloseBtn);
    transform.Find("content/createBtn").GetComponent<Button>().onClick.AddListener(OnCreateRoomBtn);
    transform.Find("content/updateBtn").GetComponent<Button>().onClick.AddListener(OnUpdateRoomBtn);

    contentTf = transform.Find("content/Scroll View/Viewport/Content");
    roomItemPrefab = transform.Find("content/Scroll View/Viewport/item").gameObject;

    //1.大厅名称, 2.大厅类型. SqlLobby(可搜索)
    fpsLobby = new TypedLobby("fpsLobby", LobbyType.SqlLobby);
    PhotonNetwork.JoinLobby(fpsLobby);//进入大厅

  }

  private void AddTimerToRefreshRoomList()
  {
    //刷新房间列表
    PhotonNetwork.GetCustomRoomList(fpsLobby, "1");//执行该方法,会触发 OnRoomListUpdate() 回调
    _timer = Timer.Register(
     duration: 3f,
     onComplete: () =>
     {
       //刷新房间列表
       PhotonNetwork.GetCustomRoomList(fpsLobby, "1");//执行该方法,会触发 OnRoomListUpdate() 回调
     },
     isLooped: true,
     useRealTime: true);
  }

  public override void OnDisable()
  {
    base.OnDisable();

    Timer.Cancel(_timer);
  }

  //进入大厅的回调
  public override void OnJoinedLobby()
  {
    base.OnJoinedLobby();

    Debug.Log("进入大厅成功");

    AddTimerToRefreshRoomList();
  }

  private void OnCloseBtn()
  {
    PhotonNetwork.Disconnect();
    Game.uIManager.CloseUI(gameObject.name);

    //显示登录界面
    Game.uIManager.ShowUI<LoginUI>("LoginUI");
  }

  private void OnCreateRoomBtn()
  {
    Game.uIManager.ShowUI<CreateRoomUI>("CreateRoomUI");

    Timer.Cancel(_timer);
  }

  //刷新房间按钮
  public void OnUpdateRoomBtn()
  {
    Game.uIManager.ShowUI<MaskUI>("MaskUI").ShowMsg("不要着急,刷新一下,马上回来...");

    //刷新房间列表
    PhotonNetwork.GetCustomRoomList(fpsLobby, "1");//执行该方法,会触发 OnRoomListUpdate() 回调
  }

  //清除已经存在的房间物体
  private void ClearRoomList()
  {
    while (contentTf.childCount > 0)
    {
      DestroyImmediate(contentTf.GetChild(0).gameObject);
    }
  }

  //刷新房间后的回调
  public override void OnRoomListUpdate(List<RoomInfo> roomList)
  {
    Game.uIManager.CloseUI("MaskUI");
    base.OnRoomListUpdate(roomList);

    Debug.Log("刷新房间成功");
    ClearRoomList();

    for (int i = 0; i < roomList.Count; i++)
    {
      GameObject obj = Instantiate(roomItemPrefab, contentTf);
      obj.SetActive(true);
      string roomName = roomList[i].Name;//房间名称
      obj.transform.Find("roomName").GetComponent<Text>().text = roomName;
      obj.transform.Find("joinBtn").GetComponent<Button>().onClick.AddListener(() =>
      {
        Debug.Log($"房间号:{roomName}");

        //加入房间
        Game.uIManager.ShowUI<MaskUI>("MaskUI").ShowMsg("加入房间中...");

        PhotonNetwork.JoinRoom(roomName);//加入房间
        Timer.Cancel(_timer);
      });
    }

  }

  //加入房间回调
  public override void OnJoinedRoom()
  {
    base.OnJoinedRoom();

    Game.uIManager.CloseAllUI();
    Game.uIManager.ShowUI<RoomUI>("RoomUI");
  }

  //加入房间失败的回调
  public override void OnJoinRoomFailed(short returnCode, string message)
  {
    base.OnJoinRoomFailed(returnCode, message);

    Game.uIManager.CloseAllUI();
  }
}
