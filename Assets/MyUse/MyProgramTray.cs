using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyProgramTray : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
	NewTray tray;
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		tray = new NewTray();
		tray.InitTray();
	}
	private void OnApplicationQuit()
	{
		tray?.Dispose();
		tray = null;
	}

	public void SetTipp()
	{
		tray.setTip();
	}

#endif
}
