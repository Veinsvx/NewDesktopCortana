using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class LayoutManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    private const int SW_HIDE = 0;  //hied task bar

    private const int SW_RESTORE = 9;//show task bar

    private static System.Windows.Forms.NotifyIcon _notifyIcon = new System.Windows.Forms.NotifyIcon();

    private static int _width = 100, _height = 100;

    private IntPtr window;

    private void Start()
    {
        HideTaskBar();
    }

    public void HideTaskBar()//最小化到托盘
    {
        try
        {
            window = GetForegroundWindow();

            ShowWindow(window, SW_HIDE);

            _notifyIcon.BalloonTipText = "AIScanner1.1.0";//托盘气泡显示内容

            _notifyIcon.Text = "AIScanner";//鼠标悬浮时显示的内容

            _notifyIcon.Visible = true;//托盘按钮是否可见

            _notifyIcon.Icon = CustomTrayIcon(Application.streamingAssetsPath + "/icon.png", _width, _height);//托盘图标

            _notifyIcon.ShowBalloonTip(2000);//托盘气泡显示时间

            _notifyIcon.MouseClick += notifyIcon_MouseClick;//双击托盘图标响应事件
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private static System.Drawing.Icon CustomTrayIcon(string iconPath, int width, int height)
    {
        System.Drawing.Bitmap bt = new System.Drawing.Bitmap(iconPath);

        System.Drawing.Bitmap fitSizeBt = new System.Drawing.Bitmap(bt, width, height);

        return System.Drawing.Icon.FromHandle(fitSizeBt.GetHicon());
    }

    private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)//点击托盘图标
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
        {
            _notifyIcon.MouseDoubleClick -= notifyIcon_MouseClick;

            _notifyIcon.Visible = false;

            ShowWindow(window, SW_RESTORE);
        }
    }

    private void OnDestroy()
    {
        _notifyIcon.MouseDoubleClick -= notifyIcon_MouseClick;
    }
}