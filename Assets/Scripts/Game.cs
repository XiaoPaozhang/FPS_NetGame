using System.Collections;
using System.Collections.Generic;
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
    }
  }

  void Start()
  {
    //显示登录界面
    uIManager.ShowUI<LoginUI>("LoginUI");
  }
}
