using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UnityEngine;

public class NewTray
{
    private const int WIDTH = 40;
    private const int HEIGHT = 40;
    private IntPtr hwnd;

    private NotifyIcon notifyIcon;    // 托盘图标
    private ContextMenuStrip contextMenu;    // 上下文菜单
    private ToolStripMenuItem menuItem_ShowWindow;    // 显示窗口
    private ToolStripMenuItem menuItem_HideWindow;    // 隐藏这个窗口
    private ToolStripMenuItem menuItem_Quit;          // 退出程序

    public void setTip()
    {
        this.notifyIcon.ShowBalloonTip(0, "番茄钟", "倒计时结束", ToolTipIcon.Info);
    }

    public void InitTray()
    {
        int displayLength = Display.displays.Length;
        this.hwnd = Win32API_SetWindow.GetForegroundWindow();
        this.notifyIcon = new NotifyIcon();
        this.contextMenu = new ContextMenuStrip();
        this.menuItem_ShowWindow = new ToolStripMenuItem();
        this.menuItem_HideWindow = new ToolStripMenuItem();
        this.menuItem_Quit = new ToolStripMenuItem();
        this.contextMenu.SuspendLayout();
        //
        // notifyIcon
        //
        this.notifyIcon.ContextMenuStrip = contextMenu;
        this.notifyIcon.Text = "VeinsVx的List番茄钟";
        this.notifyIcon.Icon = new System.Drawing.Icon(SystemIcons.WinLogo, 40, 40);//托盘图标
        this.notifyIcon.MouseClick += this.NotifyIcon_MouseClick;
        this.notifyIcon.Visible = true;
        //
        // contextMenu
        //
        List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>
            {
                this.menuItem_ShowWindow,
                this.menuItem_HideWindow,
                this.menuItem_Quit
            };
        this.contextMenu.Items.AddRange(menuItems.ToArray());
        this.contextMenu.Size = new Size(181, (menuItems.Count * 22) + 20);
        // 
        // menuItem_MainWindow
        // 
        ShowWindows();
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


    public void ShowWindows()
    {
        this.menuItem_ShowWindow.Size = new Size(180, 22);
        this.menuItem_ShowWindow.Text = "显示窗口";//显示窗口
        this.menuItem_ShowWindow.Click += (sender, e) => Win32API_SetWindow.Show(this.hwnd);
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

    public void Dispose()
    {
        DestroyClick();
        notifyIcon?.Dispose();
        contextMenu?.Dispose();
        menuItem_ShowWindow?.Dispose();
        menuItem_HideWindow?.Dispose();
        menuItem_Quit?.Dispose();
        this.hwnd = IntPtr.Zero;
    }
}
