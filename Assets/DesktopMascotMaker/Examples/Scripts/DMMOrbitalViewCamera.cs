using UnityEngine;
using System.Collections;
using System.Windows.Forms;
using DesktopMascotMaker;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(MascotMaker))]
public class DMMOrbitalViewCamera : MonoBehaviour
{
    // 1是正向计时模式（左键单击开始计时/暂停，右键单击保存）
    // 2是观赏模式 （双击触发动作特效，单击移动，右键改变视角）
    // 3是音乐模式（左键单击 暂停/播放，右键单击下一首）//暂时采用快捷键控制的方式
    // 4是壁纸模式 （左键单击 上一张，右键单击下一张）
    [SerializeField]
    public int count = 2;
    //
    [SerializeField]
    public Text infotext;

    private int todoCount = 0;

    public int Pcount = 1;//图片


    // If you want to use this script with MascotMakerMulti,
    // Uncomment the following line and replace 'MascotMaker.Instance' to 'mascotMakerMulti'.
    //public MascotMakerMulti mascotMakerMulti; // Assign MascotMakerMulti's instance to this variable.

    // Camera's target to look at
    public Transform target;

    // rotation speed
    public float speed = 0.3f;

    // vertical rotation limit
    public float yMinLimit = -60f;
    public float yMaxLimit = 80f;

    // for mascot's display size
    public float minSize = 0.88f; // minimum size of orthographic view. this parameter is for orthographic camera only.
    public float maxSize = 2.00f; // maximum size of orthographic view. this parameter is for orthographic camera only.
    public float nearDistance = 1.20f; // minimum distance between camera and target. this parameter is for perspective camera only. 
    public float farDistance = 2.00f;  // maximum distance between camera and target. this parameter is for perspective camera only. 

    // for Rotate
    private bool isRotate = false;
    private int xPos0;
    private int yPos0;
    private float xRot = 0.0f;
    private float yRot = 0.0f;

    private Camera mascotCamera;
    [SerializeField]
    private float distance;

    void Start()
    {
        Debug.Assert(target != null, "Desktop Mascot Maker error (DMMOrbitalViewCamera) : Target is not assined.", transform);

        mascotCamera = GetComponent<Camera>();

        Vector3 angles = transform.eulerAngles;
        xRot = angles.y;
        yRot = angles.x;

        // Assign events to MascotMaker's EventHandler
        MascotMaker.Instance.OnRightMouseDown += RightMouseOn;
        MascotMaker.Instance.OnRightMouseUp += RightMouseUp;
        MascotMaker.Instance.OnMouseWheel += MouseWheel;

        mascotCamera.orthographicSize = 2.3f;
        distance = (nearDistance + farDistance) / 2.0f;
    }

    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    public static extern void Keybd_event(
              byte bvk,//虚拟键值 ESC键对应的是27
              byte bScan,//0
              int dwFlags,//0为按下，1按住，2释放
              int dwExtraInfo//0
              );

    [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
    public static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni
            );


    public void RightMouseOn(object sender, MouseEventArgs e)
    {
        xPos0 = System.Windows.Forms.Cursor.Position.X;
        yPos0 = System.Windows.Forms.Cursor.Position.Y;
        if (count == 2)//观赏模式
        {
            isRotate = true;
        }
        else if (count == 1)//调整倒计时
        {
            DialogResult result = MessageBox.Show("是否保存计时结果", "标题", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                //确定按钮的方法
                this.GetComponent<TimeManage>().JSButtonTwoDown();
                this.GetComponent<Swapper>().SwitchSkin();
            }
            else
            {
                return;
            }
        }
        else if (count == 3)
        {
            Keybd_event(17, 0, 0, 0);
            Keybd_event(18, 0, 0, 0);
            Keybd_event(39, 0, 0, 0);
            Keybd_event(39, 0, 2, 0);
            Keybd_event(18, 0, 2, 0);
            Keybd_event(17, 0, 2, 0);
        }
        else if (count == 4)
        {
            Pcount += 1;
            if(Pcount>8)
            {
                Pcount = 1;
            }
            string FilePath = "";
            if (UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer || UnityEngine.Application.platform == RuntimePlatform.WindowsEditor)
            {
                FilePath = $"{System.Environment.CurrentDirectory}" + string.Format("\\WPManage\\{0}.jpg",Pcount);
            }
            SystemParametersInfo(20, 0, FilePath, 1);
        }
    }

    void RightMouseUp(object sender, MouseEventArgs e)
    {
        isRotate = false;
    }


    void MouseWheel(object sender, MouseEventArgs e)
    {
        //用来控制选择状态的
        float temp = (float)e.Delta;
        if (temp >= 110)
        {
            count += 1;
            if (count > 4)
            {
                count = 1;
            }

        }
        else
        {
            count -= 1;
            if (count < 1)
            {
                count = 4;
            }
        }

        if (count == 2)//观赏模式
        {
            infotext.text = "观赏模式";
        }
        else if (count == 1)//调整计时
        {
            infotext.text = "计时模式";
        }
        else if (count == 3)
        {
            infotext.text = "音乐模式";
        }
        else if (count == 4)
        {
            infotext.text = "壁纸模式";
        }
        Debug.Log(count);
    }

    void Update() //LateUpdate()
    {
        if (isRotate)
        {
            int xPosTmp = System.Windows.Forms.Cursor.Position.X;
            int yPosTmp = System.Windows.Forms.Cursor.Position.Y;

            xRot += (xPosTmp - xPos0) * speed;
            yRot += (yPosTmp - yPos0) * speed;
            xPos0 = xPosTmp;
            yPos0 = yPosTmp;

            yRot = ClampAngle(yRot, yMinLimit, yMaxLimit);
        }

        Quaternion rotation = Quaternion.Euler(yRot, xRot, 0);

        Vector3 distanceVector = new Vector3(0.0f, 0.0f, -distance); //2);		 
        Vector3 position = rotation * distanceVector + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
