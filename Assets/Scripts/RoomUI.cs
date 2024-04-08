using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

//房间详情页
public class RoomUI : MonoBehaviour, IInRoomCallbacks
{
  Transform startTf;//开始按钮
  Transform contentTf;//房间列表父物体
  GameObject roomPrefab;//房间预制体
  List<RoomItem> roomList;//房间列表
  void Awake()
  {
    contentTf = transform.Find("bg/Content");
    roomPrefab = transform.Find("bg/roomItem").gameObject;
    roomList = new List<RoomItem>();
    transform.Find("bg/title/closeBtn").GetComponent<Button>().onClick.AddListener(OnCloseBtn);
    transform.Find("bg/startBtn").GetComponent<Button>().onClick.AddListener(OnStartBtn);
  }

  void Start()
  {
    //生成房间里的玩家项
    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
    {
      Player player = PhotonNetwork.PlayerList[i];
      CreateRoomItem(player);
    }
  }

  void OnEnable()
  {
    PhotonNetwork.AddCallbackTarget(this);
  }

  void OnDisable()
  {
    PhotonNetwork.RemoveCallbackTarget(this);
  }

  public void OnCloseBtn()
  {
    //断开连接
    PhotonNetwork.Disconnect();
    Game.uIManager.CloseUI(gameObject.name);
    Game.uIManager.ShowUI<LoginUI>("LoginUI");
  }

  private void OnStartBtn()
  {
    Debug.Log("开始游戏");
  }

  //生成玩家
  public void CreateRoomItem(Player player)
  {
    GameObject obj = Instantiate(roomPrefab, contentTf);
    obj.SetActive(true);
    RoomItem item = obj.AddComponent<RoomItem>();
    item.ownerId = player.ActorNumber;
    roomList.Add(item);
  }

  //删除离开房间的玩家
  public void DeleteRoomItem(Player player)
  {
    RoomItem item = roomList.Find((RoomItem x) => player.ActorNumber == x.ownerId);
    if (item != null)
    {
      Destroy(item.gameObject);
      roomList.Remove(item);
    }
  }

  #region IInRoomCallbacks接口实现
  //房间内玩家加入
  public void OnPlayerEnteredRoom(Player newPlayer)
  {
    CreateRoomItem(newPlayer);
  }

  //房间内其他玩家离开
  public void OnPlayerLeftRoom(Player otherPlayer)
  {
    DeleteRoomItem(otherPlayer);
  }

  //房间属性更新
  public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
  {

  }

  //房间内玩家属性更新
  public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
  {

  }

  //房间内玩家自定义事件
  public void OnMasterClientSwitched(Player newMasterClient)
  {

  }
  #endregion
}
