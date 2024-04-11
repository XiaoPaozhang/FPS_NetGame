using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Game : MonoBehaviour
{
  public static UIManager uIManager;
  public static bool isLoaded = false;
  void Awake()
  {
    if (isLoaded)
    {
      Destroy(gameObject);
    }
    else
    {
      isLoaded = true;
      DontDestroyOnLoad(gameObject);
      uIManager = new UIManager();
      uIManager.Init();

      //设置发送 接受消息频率 降低延迟
      PhotonNetwork.SendRate = 50;
      PhotonNetwork.SerializationRate = 50;
    }
  }

  void Start()
  {
    //显示登录界面
    uIManager.ShowUI<LoginUI>("LoginUI");
  }
}
