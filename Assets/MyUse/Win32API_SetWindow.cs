using System;
using System.Runtime.InteropServices;

public class Win32API_SetWindow
{
	#region Win32Api

	/// <summary>
	/// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getforegroundwindow
	/// </summary>
	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern IntPtr GetForegroundWindow();

	private const int
		SW_HIDE = 0,            // 隐藏窗口和任务栏
		SW_SHOW = 5;            // 显示当前大小和位置

	/// <summary>
	/// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
	/// </summary>
	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

	private static readonly IntPtr
		HWND_TOP = new IntPtr(0);           // 放在最顶部
	private const uint
		SWP_NOSIZE = 0x0001,                // 保留当前大小（忽略cx和cy参数）。
		SWP_NOZORDER = 0x0004;              // 保留当前的Z顺序（忽略hWndInsertAfter参数）。

	/// <summary>
	/// 设置窗口位置
	/// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
	/// </summary>
	[DllImport("user32.dll")]
	private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
	#endregion

	#region Public

	#region ShowWindow
	public static void Hide(IntPtr hwnd) => ShowWindow(hwnd, SW_HIDE);
	public static void Show(IntPtr hwnd) => ShowWindow(hwnd, SW_SHOW);
	#endregion

	#region SetWindowPos
	public static void SetWindowPosOnDisplay2(IntPtr hWnd, int left, int top)
	{
		SetWindowPos(hWnd, HWND_TOP, left, top, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
	}
	#endregion
	#endregion

}
