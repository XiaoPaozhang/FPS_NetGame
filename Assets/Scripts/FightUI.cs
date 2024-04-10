using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightUI : MonoBehaviour
{
  void Start()
  {

  }

  //更新子弹个数显示
  public void UpdateBulletCount(int count)
  {
    transform.Find("bullet/Text").GetComponent<Text>().text = count.ToString();
  }

  //更新血量显示
  public void UpdateHp(float cur, float max)
  {
    transform.Find("hp/fill").GetComponent<Image>().fillAmount = cur / max;
    transform.Find("hp/Text").GetComponent<Text>().text = cur + "/" + max;
  }
}
