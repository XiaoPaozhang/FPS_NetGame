using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : MonoBehaviour
{
  public int BulletCount = 10;
  public GameObject bulletPrefab;
  public GameObject casingPrefab;

  public Transform bulletTf;
  public Transform casingTF;
  void Start()
  {

  }

  public void Attack()
  {
    GameObject bulletObj = Instantiate(bulletPrefab);
    bulletObj.transform.position = bulletTf.position;
    bulletObj.GetComponent<Rigidbody>().AddForce(transform.forward * 100, ForceMode.Impulse);

    GameObject casingObj = Instantiate(casingPrefab);
    casingObj.transform.position = casingTF.transform.position;
  }
}
