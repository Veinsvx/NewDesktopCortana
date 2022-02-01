using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;
using System;
using System.Collections;


#region 番茄钟json格式
[Serializable]
public class ClockList
{
    public int totalTimer;
    public List<SubClockList> sub;
}

[Serializable]
public class SubClockList
{
    public string subName;
    public int subTimer;
}
#endregion

#region 任务清单json格式
[Serializable]
public class listItemClass
{
    public string objName;
    public int index;
    public List<SubListClass> sub;
    public listItemClass(string name, int index, List<SubListClass> sudata)
    {
        this.objName = name;
        this.index = index;
        //list被json转化后的string字符串
        this.sub = sudata;
    }
}

[Serializable]
public class SubListClass
{
    public string objName;
    public int index;
    public bool isok;
    public SubListClass(string name, int index, bool isok)
    {
        this.objName = name;
        this.index = index;
        this.isok = isok;
    }
}
#endregion



public class CoreManage
{

    static CoreManage instance;

    public static CoreManage Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CoreManage();
            }
            return instance;
        }
    }

    public string ServerIp;

    public bool todoDownOk = false;
    public bool clockDownOk = false;

    public string todoFilePath;
    public string clockFilePath;

    public ClockList clockList = new ClockList();//因为线程不能控制Unity的一些资源，所以这个作为副本存储，方便调用
    public List<ListObject> ListObjects = new List<ListObject>();//因为线程不能控制Unity的一些资源，所以这个作为副本存储，方便调用
    public List<SubClockList> subClockList = new List<SubClockList>();//因为线程不能控制Unity的一些资源，所以这个作为副本存储，方便调用

    #region 暂时废弃，因为之前服务端是python需要自己手动解码
    //public string Decodeing(string s)
    //{
    //    Regex reUnicode = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);
    //    return reUnicode.Replace(s, m =>
    //    {
    //        short c;
    //        if (short.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out c))
    //        {
    //            return "" + (char)c;
    //        }
    //        return m.Value;
    //    });
    //}
    #endregion

    /// <summary>
    /// 读取本地todoJson文件，将其转换为string类型
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public string ReadJsonFun(string strPath)
    {
        string dataAsJson = "";
        dataAsJson = File.ReadAllText(strPath, Encoding.UTF8);
        if (dataAsJson != null)
        {
            return dataAsJson;
        }
        return dataAsJson;
    }


    public bool SaveData(string str)
    {
        if (str == "todo")
        {
            string contents = "";
            List<SubListClass> templist = new List<SubListClass>();
            for (int i = 0; i < ListObjects.Count; i++)
            {
                if (ListObjects[i].sublistcalss.Count > 0)
                {
                    for (int j = 0; j < ListObjects[i].sublistcalss.Count; j++)
                    {
                        templist.Add(ListObjects[i].sublistcalss[j]);
                    }
                }
                listItemClass temp2 = new listItemClass(ListObjects[i].objName, ListObjects[i].index, templist);
                contents += JsonUtility.ToJson(temp2) + "\n";
                templist.Clear();
            }
            Debug.Log(contents);
            File.WriteAllText(CoreManage.Instance.todoFilePath, contents);
            return true;
        }
        else if (str == "clock")
        {
            string contents = "";
            contents = JsonUtility.ToJson(clockList) + "\n";
            Debug.Log(contents);
            File.WriteAllText(CoreManage.Instance.clockFilePath, contents);
            return true;
        }
        return false;
    }


}
