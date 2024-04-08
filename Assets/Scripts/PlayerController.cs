using UnityEngine;

public class PlayerController : MonoBehaviour
{
  // 组件
  public Animator ani;
  public Rigidbody body;
  public Transform camTf; // 跟随的摄像机

  // 数值
  public int CurHp = 10;
  public int MaxHp = 10;
  public float MoveSpeed = 3.5f;
  public float H; // 水平值
  public float V; // 垂直值
  public Vector3 dir; // 方向
  public Vector3 offset;//摄像机与角色之间的偏移量

  public float mouse_X; // 鼠标水平值
  public float mouse_Y; // 鼠标垂直值
  public float scroll; // 滚轮值
  public float angle_X; // 角色的角度X
  public float angle_Y; // 角色的角度Y
  public Quaternion camRotaion; // 相机的旋转

  void Start()
  {
    angle_X = transform.eulerAngles.x;
    angle_Y = transform.eulerAngles.y;
    ani = GetComponent<Animator>();
    body = GetComponent<Rigidbody>();
    camTf = Camera.main.transform;
  }

  void Update()
  {
    UpdatePosition();
    UpDateRotation();
  }

  void LateUpdate()
  {
    ani.SetFloat("Horizontal", H);
    ani.SetFloat("Vertical", V);
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

    angle_X = ClamAngle(angle_X, -90, 90); // 限制角度在-90到90之间
    angle_Y = ClamAngle(angle_Y, -360, 360); // 限制角度在-360到360之间

    camRotaion = Quaternion.Euler(angle_X, angle_Y, 0); // 角色的旋转

    camTf.rotation = camRotaion; // 相机的旋转

    camTf.position = transform.position + offset; // 跟随角色的位置

    offset.z += scroll; // 滚轮控制相机的高度

    //限制角度在-360到360之间
    camTf.position = transform.position + camTf.rotation * offset; // 跟随角色的位置

    transform.eulerAngles = new Vector3(0, camTf.eulerAngles.y, 0); // 角色的角度
  }

  public float ClamAngle(float val, float min, float max)
  {
    if (val > 360)
      val -= 360;

    if (val < 0)
      val += 360;

    return Mathf.Clamp(val, min, max);
  }

  private void OnAnimatorIK(int layerIndex)
  {
    if (ani != null)
    {
      //设置头部的位置
      ani.SetBoneLocalRotation(HumanBodyBones.Chest, camRotaion * Quaternion.Euler(angle_X, 0, 0));
    }
  }
}