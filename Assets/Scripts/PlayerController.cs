using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
  // 组件
  public Animator ani;
  public Rigidbody body;
  public Transform camTf; // 跟随的摄像机

  // 数值
  public int CurHp = 10;
  public int MaxHp = 10;
  public float MoveSpeed = 10f;
  public float H; // 水平值
  public float V; // 垂直值
  public Vector3 dir; // 方向
  public Vector3 offset;//摄像机与角色之间的偏移量

  public float mouse_X; // 鼠标水平值
  public float mouse_Y; // 鼠标垂直值
  public float scroll; // 滚轮值
  public float angle_X; // 角色的角度X
  public float angle_Y; // 角色的角度Y
  public Quaternion camRotation; // 相机的旋转
  public Gun gun; // 武器
  public AudioClip reloadClip; // 填充子弹音效
  public AudioClip shootClip; // 射击音效
  public bool isDead; // 是否死亡
  public Vector3 currentPos; // 当前位置
  public Quaternion currentRotation; // 当前旋转

  void Start()
  {
    angle_X = transform.eulerAngles.x;
    angle_Y = transform.eulerAngles.y;
    ani = GetComponent<Animator>();
    body = GetComponent<Rigidbody>();
    gun = GetComponentInChildren<Gun>();
    camTf = Camera.main.transform;
    currentPos = transform.position;
    currentRotation = transform.rotation;

    if (photonView.IsMine)
    {
      Game.uIManager.GetUI<FightUI>("FightUI").UpdateHp(CurHp, MaxHp);
    }
  }

  void Update()
  {
    //判断是否是本地玩家
    if (photonView.IsMine)
    {
      if (isDead) return;

      UpdatePosition();
      UpDateRotation();
      InputCtrl();
    }
    else
    {
      UpdateLogic();
    }
  }

  //其他角色更新发送过来的数据(位置 旋转)
  public void UpdateLogic()
  {
    transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * MoveSpeed);
    transform.rotation = Quaternion.Slerp(transform.rotation, currentRotation, Time.deltaTime);
  }

  void LateUpdate()
  {
    ani.SetFloat("Horizontal", H);
    ani.SetFloat("Vertical", V);
    ani.SetBool("isDie", isDead);
  }

  //更新位置
  public void UpdatePosition()
  {
    H = Input.GetAxisRaw("Horizontal");
    V = Input.GetAxisRaw("Vertical");
    dir = camTf.forward * V + camTf.right * H;
    body.MovePosition(transform.position + dir * Time.deltaTime * MoveSpeed);
  }

  //更新旋转 (同时设置摄像机的旋转值)
  public void UpDateRotation()
  {
    mouse_X = Input.GetAxisRaw("Mouse X");
    mouse_Y = Input.GetAxisRaw("Mouse Y");
    scroll = Input.GetAxis("Mouse ScrollWheel");

    angle_X = angle_X - mouse_Y;
    angle_Y = angle_Y + mouse_X;

    angle_X = ClamAngle(angle_X, -60, 60); // 限制角度在-60到90之间
    angle_Y = ClamAngle(angle_Y, -360, 360); // 限制角度在-360到360之间

    camRotation = Quaternion.Euler(angle_X, angle_Y, 0); // 角色的旋转

    camTf.rotation = camRotation; // 相机的旋转

    offset.z += scroll; // 滚轮控制相机的高度

    //限制角度在-360到360之间
    camTf.position = transform.position + camTf.rotation * offset; // 跟随角色的位置

    transform.eulerAngles = new Vector3(0, camTf.eulerAngles.y, 0); // 角色的角度
  }

  //角色操作
  public void InputCtrl()
  {

    //如果正在播放填充子弹动画，则不能再射击
    if (ani.GetCurrentAnimatorStateInfo(1).IsName("Reload"))
      return;

    //按下鼠标左键，射击
    if (Input.GetMouseButtonDown(0))
    {

      //判断子弹个数
      if (gun.BulletCount > 0)
      {

        gun.BulletCount--;
        Game.uIManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);

        //播放枪口动画
        ani.Play("Fire", 1, 0);
        //发射子弹
        StartCoroutine(AttackCo());
      }
    }

    //按下R键，填充子弹
    if (Input.GetKeyDown(KeyCode.R))
    {
      //填充子弹
      AudioSource.PlayClipAtPoint(reloadClip, transform.position);//播放填充子弹音效
      ani.Play("Reload");
      gun.BulletCount = 10;
      Game.uIManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);
    }
  }

  //攻击协程
  IEnumerator AttackCo()
  {
    //延迟0.1秒播放攻击动画
    yield return new WaitForSeconds(0.1f);

    //播放射击音效
    AudioSource.PlayClipAtPoint(shootClip, transform.position);

    //射线检测 鼠标中心点发送射线
    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, Input.mousePosition.z));

    if (Physics.Raycast(
      ray,
      out RaycastHit hit,
      10000,
      LayerMask.GetMask("Player")))
    {
      Debug.Log($"射到角色;{hit.transform.name}");
      hit.transform.GetComponent<PlayerController>().GetHit(1);
    }

    photonView.RPC("AttackRpc", RpcTarget.All);//所有玩家执行AttackRpc函数
  }

  [PunRPC]
  public void AttackRpc()
  {
    gun.Attack();
  }

  //受伤
  public void GetHit(int damage)
  {
    if (isDead) return;

    photonView.RPC("GetHitRPC", RpcTarget.All, damage);
  }

  [PunRPC]
  public void GetHitRPC(int damage)
  {
    CurHp -= damage;
    if (CurHp <= 0)
    {
      CurHp = 0;
      isDead = true;

    }
    if (photonView.IsMine)
    {
      FightUI fightUI = Game.uIManager.GetUI<FightUI>("FightUI");
      fightUI.UpdateHp(CurHp, MaxHp);
      fightUI.UpdateBlood();

      if (CurHp == 0)
      {
        Invoke("GameOver", 2f); // 2秒后游戏结束
      }
    }
  }

  private void GameOver()
  {
    //显示鼠标
    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;

    //显示失败界面
    Game.uIManager.ShowUI<LossUI>("LossUI").onClickCallback = OnReset;
  }

  //复活吧,我的爱人
  public void OnReset()
  {
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;

    photonView.RPC("OnResetRPC", RpcTarget.All);
  }

  [PunRPC]
  public void OnResetRPC()
  {
    isDead = false;
    Debug.Log($"是否死亡: {isDead}");
    CurHp = MaxHp;
    if (photonView.IsMine)
    {
      Game.uIManager.GetUI<FightUI>("FightUI").UpdateHp(CurHp, MaxHp);
    }
  }

  public float ClamAngle(float val, float min, float max)
  {
    if (val > 360)
      val -= 360;

    if (val < -360)
      val += 360;

    return Mathf.Clamp(val, min, max);
  }

  private void OnAnimatorIK(int layerIndex)
  {
    if (ani != null) // 检查动画组件是否存在
    {
      // 获取胸部骨骼当前的本地旋转角度
      Vector3 angle = ani.GetBoneTransform(HumanBodyBones.Chest).localEulerAngles;
      angle.x = angle_X; // 设置新的X轴旋转角度，angle_X是外部定义的变量

      // 设置胸部骨骼的本地旋转，使用Quaternion.Euler将Vector3的旋转角度转换为四元数
      ani.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Euler(angle));
    }
  }


  //会在Photon的序列化时调用 用来同步数据
  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
  {
    if (stream.IsWriting)
    {
      //发送数据
      stream.SendNext(H);
      stream.SendNext(V);
      stream.SendNext(angle_X);
      stream.SendNext(transform.position);
      stream.SendNext(transform.rotation);
    }
    else
    {
      //接收数据
      H = (float)stream.ReceiveNext();
      V = (float)stream.ReceiveNext();
      angle_X = (float)stream.ReceiveNext();
      currentPos = (Vector3)stream.ReceiveNext();
      currentRotation = (Quaternion)stream.ReceiveNext();
    }
  }
}
