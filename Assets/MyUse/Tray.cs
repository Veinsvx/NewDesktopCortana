using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;

public class Tray : IDisposable
{
    private const int WIDTH = 40;
    private const int HEIGHT = 40;
    private IntPtr hwnd;

    private NotifyIcon notifyIcon;    // 托盘图标
    private ContextMenuStrip contextMenu;    // 上下文菜单
    private ToolStripMenuItem menuItem_ShowWindow;    // 显示窗口
    private ToolStripMenuItem menuItem_Windowed;      // 窗口化
    private ToolStripMenuItem menuItem_Display1;      // 在主屏幕上全部显示
    private ToolStripMenuItem menuItem_Display2;      // 在屏幕2全屏显示
    private ToolStripMenuItem menuItem_HideWindow;    // 隐藏这个窗口
    private ToolStripMenuItem menuItem_Quit;          // 退出程序

    public void InitTray()
    {
        int displayLength = Display.displays.Length;
        this.hwnd = Win32API_SetWindow.GetForegroundWindow();
        this.notifyIcon = new NotifyIcon();
        this.contextMenu = new ContextMenuStrip();
        this.menuItem_ShowWindow = new ToolStripMenuItem();
        this.menuItem_Windowed = new ToolStripMenuItem();
        if (displayLength > 1)
        {
            this.menuItem_Display1 = new ToolStripMenuItem();
            this.menuItem_Display2 = new ToolStripMenuItem();
        }
        this.menuItem_HideWindow = new ToolStripMenuItem();
        this.menuItem_Quit = new ToolStripMenuItem();
        this.contextMenu.SuspendLayout();
        //
        // notifyIcon
        //
        this.notifyIcon.ContextMenuStrip = contextMenu;
        this.notifyIcon.Text = "Hello 我是一个图标";
        var iconPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "Icon.png");
        if (File.Exists(iconPath))
            this.notifyIcon.Icon = this.CustomTrayIcon(iconPath, WIDTH, HEIGHT);
        else
        {
            //var bytes = Resources.Load<Texture2D>("Icon/Icon").EncodeToPNG();
            //this.notifyIcon.Icon = CustomTrayIcon(ByteArrayToImage(bytes), WIDTH, HEIGHT);
            this.notifyIcon.Icon = new System.Drawing.Icon(SystemIcons.WinLogo, 40, 40);//托盘图标


        }
        this.notifyIcon.MouseClick += this.NotifyIcon_MouseClick;
        this.notifyIcon.Visible = true;
        this.notifyIcon.ShowBalloonTip(2000, "气泡Title", "气泡Tip", ToolTipIcon.Info);
        //
        // contextMenu
        //
        List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>
            {
                this.menuItem_ShowWindow,
                this.menuItem_Windowed,
                this.menuItem_HideWindow,
                this.menuItem_Quit
            };
        if (displayLength > 1)
        {
            menuItems.Insert(2, this.menuItem_Display1);
            menuItems.Insert(3, this.menuItem_Display2);
        }
        this.contextMenu.Items.AddRange(menuItems.ToArray());
        this.contextMenu.Size = new Size(181, (menuItems.Count * 22) + 20);
        // 
        // menuItem_MainWindow
        // 
        this.menuItem_ShowWindow.Size = new Size(180, 22);
        this.menuItem_ShowWindow.Text = "显示窗口";//显示窗口
        this.menuItem_ShowWindow.Click += (sender, e) => Win32API_SetWindow.Show(this.hwnd);
        // 
        // menuItem_Windowed
        // 
        this.menuItem_Windowed.Size = new Size(180, 22);
        this.menuItem_Windowed.Text = "窗口化";
        this.menuItem_Windowed.Click += (sender, e) =>
        {
            UnityEngine.Screen.SetResolution(1280, 720, false);
        };

        if (displayLength > 1)
        {
            // 
            // menuItem_Maximize
            // 
            this.menuItem_Display1.Size = new Size(180, 22);
            this.menuItem_Display1.Text = "屏幕1全屏显示";
            this.menuItem_Display1.Click += (sender, e) =>
            {
                Win32API_SetWindow.SetWindowPosOnDisplay2(this.hwnd, 0, 0);
                UnityEngine.Screen.SetResolution(Display.displays[0].systemWidth, Display.displays[0].systemHeight, true);
            };
            // 
            // menuItem_Maximize
            // 
            this.menuItem_Display2.Size = new Size(180, 22);
            this.menuItem_Display2.Text = "屏幕2全屏显示";
            this.menuItem_Display2.Click += (sender, e) =>
            {
                Win32API_SetWindow.SetWindowPosOnDisplay2(this.hwnd, Display.displays[0].systemWidth, 0);
                UnityEngine.Screen.SetResolution(Display.displays[1].systemWidth, Display.displays[1].systemHeight, true);
            };
        }
        // 
        // menuItem_HideWindow
        // 
        this.menuItem_HideWindow.Size = new Size(180, 22);
        this.menuItem_HideWindow.Text = "隐藏窗口";
        this.menuItem_HideWindow.Click += (sender, e) => Win32API_SetWindow.Hide(this.hwnd);
        // 
        // menuItem_Quit
        // 
        this.menuItem_Quit.Size = new Size(180, 22);
        this.menuItem_Quit.Text = "退出";
        this.menuItem_Quit.Click += (sender, e) =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
            UnityEngine.Application.Quit();
        };

        this.contextMenu.ResumeLayout(false);
    }


    private Icon CustomTrayIcon(string iconPath, int width, int height)
    {
        Bitmap bt = new Bitmap(iconPath);
        Bitmap fitSizeBt = new Bitmap(bt, width, height);
        return Icon.FromHandle(fitSizeBt.GetHicon());
    }

    private Icon CustomTrayIcon(Image img, int width, int height)
    {
        Bitmap bt = new Bitmap(img);
        Bitmap fitSizeBt = new Bitmap(bt, width, height);
        return Icon.FromHandle(fitSizeBt.GetHicon());
    }

    private Image ByteArrayToImage(byte[] byteArrayIn)
    {
        if (byteArrayIn == null)
            return null;
        using (MemoryStream ms = new MemoryStream(byteArrayIn))
        {
            Image returnImage = Image.FromStream(ms);
            ms.Flush();
            return returnImage;
        }
    }

    private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            Win32API_SetWindow.Show(this.hwnd);
        }
    }

    private void DestroyClick() => notifyIcon.MouseDoubleClick -= NotifyIcon_MouseClick;

    public void ShowTray() => notifyIcon.Visible = true;//托盘按钮是否可见

    public void HideTray() => notifyIcon.Visible = false;//托盘按钮是否可见

    public void Dispose()
    {
        DestroyClick();
        notifyIcon?.Dispose();
        contextMenu?.Dispose();
        menuItem_ShowWindow?.Dispose();
        menuItem_Windowed?.Dispose();
        menuItem_Display1?.Dispose();
        menuItem_Display2?.Dispose();
        menuItem_HideWindow?.Dispose();
        menuItem_Quit?.Dispose();
        this.hwnd = IntPtr.Zero;
    }
}
