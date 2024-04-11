using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightUI : MonoBehaviour
{
  private Image bloodImg;
  void Start()
  {
    bloodImg = transform.Find("blood").GetComponent<Image>();
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

  //更新血量
  public void UpdateBlood()
  {
    StopAllCoroutines();
    StartCoroutine(UpdateBloodCo());
  }

  public IEnumerator UpdateBloodCo()
  {
    bloodImg.color = Color.white;
    Color color = bloodImg.color;
    float t = 0.35f;
    while (t >= 0)
    {
      t -= Time.deltaTime;
      color.a = Mathf.Abs(MathF.Sin(Time.realtimeSinceStartup));
      bloodImg.color = color;
      yield return null;
    }
    color.a = 0;
    bloodImg.color = color;
  }


}
