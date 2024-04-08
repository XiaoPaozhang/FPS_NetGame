using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour, IConnectionCallbacks

{
  void Start()
  {
    transform.Find("startBtn").GetComponent<Button>().onClick.AddListener(OnStarBtn);
    transform.Find("quitBtn").GetComponent<Button>().onClick.AddListener(OnQuitBtn);
  }

  void OnEnable()
  {
    PhotonNetwork.AddCallbackTarget(this);//添加回调监听
  }

  void OnDisable()
  {
    PhotonNetwork.RemoveCallbackTarget(this);//移除回调监听
  }

  #region ui events
  private void OnStarBtn()
  {
    Game.uIManager.ShowUI<MaskUI>("MaskUI").ShowMsg("加载中...(转呀转,嘿嘿)");

    PhotonNetwork.ConnectUsingSettings();//连接Master服务器,成功后会执行 OnConnectedToMaster 函数
  }

  private void OnQuitBtn()
  {
#if UNITY_EDITOR
    // 在Unity编辑器中停止游戏模式
    EditorApplication.isPlaying = false;
#else
    // 在构建的游戏中退出应用程序
    Application.Quit();
#endif
  }
  #endregion

  #region pun2生命周期
  //连接到Master服务器失败
  public void OnConnected()
  {

  }

  //连接到Master服务器成功
  public void OnConnectedToMaster()
  {
    Game.uIManager.CloseAllUI();//关闭所有界面
    Debug.Log("连接服务器成功");

    Game.uIManager.ShowUI<LobbyUI>("LobbyUI");//打开大厅界面
  }

  //断开与Master服务器的连接,连接超时或其他原因导致
  public void OnDisconnected(DisconnectCause cause)
  {
    Debug.Log("断开与服务器的连接:" + cause.ToString());
  }

  //收到服务器的区域列表
  public void OnRegionListReceived(RegionHandler regionHandler)
  {

  }

  //收到服务器的用户列表
  public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
  {

  }

  //自定义认证失败
  public void OnCustomAuthenticationFailed(string debugMessage)
  {

  }
  #endregion

  #region main methods


  #endregion

}
