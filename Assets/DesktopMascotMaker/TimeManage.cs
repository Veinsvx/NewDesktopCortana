using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TimeManage : MonoBehaviour
{
    [SerializeField]
    private Text jstimeText;//用来显示计时器时间
    private int jsButtonCount = 0;//计时器按钮按下计数 
    private int jsTime;//计时器时间


    public void JSButtonOneDown()
    {
        jsButtonCount += 1;
        if (jsButtonCount == 1)//开始
        {
            jstimeText.text += "计时中";
            StartCoroutine("JSTimeCoro");
        }
        else if (jsButtonCount == 2)//暂停
        {
            jstimeText.text += "暂停中";
            StopCoroutine("JSTimeCoro");
            jsButtonCount = 0;
        }

    }

    public void JSButtonTwoDown()
    {
        //用户按下结束键，停止计时，时间归零
        SaveTimeInfo();

        StopCoroutine("JSTimeCoro");
        jsTime = 0;
        jstimeText.text = "";
        jsButtonCount = 0;        
    }

    private void SaveTimeInfo()
    {
        string FilePath="";
        if (Application.platform == RuntimePlatform.Android)
        {
          FilePath = Path.Combine(Application.persistentDataPath, "TimeCount.txt");
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
          FilePath = $"{System.Environment.CurrentDirectory}" + "\\TimeCount.txt";
        }

        if ((FilePath != null) || (FilePath != ""))
        {
            StreamWriter sw = File.AppendText(FilePath);
            sw.WriteLine(DateTime.Now.ToString() + "—————————>持续时间" + (jsTime / 60) + "min");
            sw.Flush();
            sw.Close();
        }
        
    }

    private IEnumerator JSTimeCoro()
    {
        while (true)
        {
            TimeSpan ts = new TimeSpan(0, 0, jsTime);
            //文本显示时间
            jstimeText.text = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
            yield return new WaitForSeconds(1f);
            jsTime += 1;
        }
    }
}
