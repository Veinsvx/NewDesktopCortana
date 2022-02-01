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
    // 1�������ʱģʽ�����������ʼ��ʱ/��ͣ���Ҽ��������棩
    // 2�ǹ���ģʽ ��˫������������Ч�������ƶ����Ҽ��ı��ӽǣ�
    // 3������ģʽ��������� ��ͣ/���ţ��Ҽ�������һ�ף�//��ʱ���ÿ�ݼ����Ƶķ�ʽ
    // 4�Ǳ�ֽģʽ ��������� ��һ�ţ��Ҽ�������һ�ţ�
    [SerializeField]
    public int count = 2;
    //
    [SerializeField]
    public Text infotext;

    private int todoCount = 0;

    public int Pcount = 1;//ͼƬ


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
              byte bvk,//�����ֵ ESC����Ӧ����27
              byte bScan,//0
              int dwFlags,//0Ϊ���£�1��ס��2�ͷ�
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
        if (count == 2)//����ģʽ
        {
            isRotate = true;
        }
        else if (count == 1)//��������ʱ
        {
            DialogResult result = MessageBox.Show("�Ƿ񱣴��ʱ���", "����", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                //ȷ����ť�ķ���
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
        //��������ѡ��״̬��
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

        if (count == 2)//����ģʽ
        {
            infotext.text = "����ģʽ";
        }
        else if (count == 1)//������ʱ
        {
            infotext.text = "��ʱģʽ";
        }
        else if (count == 3)
        {
            infotext.text = "����ģʽ";
        }
        else if (count == 4)
        {
            infotext.text = "��ֽģʽ";
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
