using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

public class ToDoManager : MonoBehaviour
{
    [SerializeField]
    private Transform toDoList;
    [SerializeField]
    private Transform clockPanel;

    public InputField todoListInput;//用户输入的任务
    public Transform content;//存放添加后的任务列表
    public GameObject ListItemPreFab;//用户任务列表预制体

    string jsonName = "TodoList.json";
    Thread connectThread;
    //public List<ListObject> ListObjects = new List<ListObject>();

    bool buttonOk = false;
    int settingButtonCount = 0;
    public InputField settingIPInput;

    void Start()
    {
        if (PlayerPrefs.GetString("IP")!=null)
        {
            CoreManage.Instance.ServerIp = PlayerPrefs.GetString("IP");
        }
        
        buttonOk = true;
        //程序一开始找到json文件目录，并赋值给filepath；
        if (Application.platform == RuntimePlatform.Android)
        {
            CoreManage.Instance.todoFilePath = Path.Combine(Application.persistentDataPath, jsonName);
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            //CoreManage.Instance.todoFilePath = @"D:\todo.json";
            CoreManage.Instance.todoFilePath = $"{System.Environment.CurrentDirectory}" + "\\TodoList.json";
        }
        loadJsonData();
    }

    void Update()
    {
        if (CoreManage.Instance.todoDownOk)
        {
            CoreManage.Instance.todoDownOk = false;
            //从服务器上下载数据后更新一下内存信息
            loadJsonData();
        }
    }

    public void CreateNewItem()
    {
        string temp = todoListInput.text;
        Debug.Log(temp);
        CreateListItem(temp);
        todoListInput.text = "";
    }

    /// <summary>
    /// 创建实例化的按钮及更新信息
    /// 用户点击创建按钮时候，先将数据存到本地，按用户需求上传。
    /// </summary>
    /// <param name="temp"></param>
    /// <param name="loadIndex"></param>
    /// <param name="loding"></param>
    private void CreateListItem(string temp, int loadIndex = 0, bool loding = false, listItemClass listclass = null)
    {
        //实例化清单
        GameObject item = Instantiate(ListItemPreFab, content);
        ListObject itemObject = item.GetComponent<ListObject>();
        int index = 0;
        if (loding != true)
        {
            //index = ListObjects.Count;
            index = CoreManage.Instance.ListObjects.Count;
        }
        itemObject.setObjectInfo(temp, index);

        //为清单上的信息赋值
        item.GetComponentInChildren<Text>().text = temp;
        if (listclass != null)
        {   //清除之前的老信息，添加新信息

            //查找名字符合的

            itemObject.sublistcalss.Clear();
            for (int i = 0; i < listclass.sub.Count; i++)
            {
                SubListClass subobj = new SubListClass(listclass.sub[i].objName, listclass.sub[i].index, listclass.sub[i].isok);
                itemObject.sublistcalss.Add(subobj);
            }
        }
        //ListObjects.Add(itemObject);
        CoreManage.Instance.ListObjects.Add(itemObject);

        //为button的点击事件添加监听
        itemObject.transform.Find("abandon").GetComponent<Button>().onClick.AddListener((delegate { CheckItem(itemObject); }));
        itemObject.transform.Find("ShowTree").GetComponent<Button>().onClick.AddListener((delegate { itemObject.ButtonClickTree(); }));
        itemObject.transform.Find("clock").GetComponent<Button>().onClick.AddListener((delegate { InClock(temp); }));
        if (loding != true)
        {
            CoreManage.Instance.SaveData("todo");
        }
    }

    void InClock(string obj)
    {
        clockPanel.gameObject.SetActive(true);
        clockPanel.Find("InputTaskName").GetComponent<InputField>().text = obj;
        toDoList.gameObject.SetActive(false);
    }

    void CheckItem(ListObject item)
    {
        if (item.subListObjects != null)
        {
            for (int i = item.subListObjects.Count; i > 0; i--)
            {
                item.subListObjects[i - 1].gameObject.GetComponent<SubListObject>().DelButtonClick();
            }
            item.sublistcalss.Clear();
        }
        //ListObjects.Remove(item);
        CoreManage.Instance.ListObjects.Remove(item);
        Destroy(item.gameObject);
        Destroy(item.myWriteSon.gameObject);
        CoreManage.Instance.SaveData("todo");
    }

    public void loadJsonData()
    {
        if (content.childCount > 0)
        {
            for (int i = 0; i < content.childCount; i++)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
        //ListObjects.Clear();
        CoreManage.Instance.ListObjects.Clear();
        string dataAsJson = "";
        dataAsJson = CoreManage.Instance.ReadJsonFun(CoreManage.Instance.todoFilePath);

        // 正确解析json文件
        string[] splitContents = dataAsJson.Split('\n');
        foreach (string content in splitContents)
        {
            if (content.Trim() != "")
            {
                listItemClass temp = JsonUtility.FromJson<listItemClass>(content.Trim());
                //先只创建父级任务，然后将子任务信息存到父级任务的listObject脚本中去。
                CreateListItem(temp.objName, temp.index, true, temp);
            }
        }

    }

    public void ButtonClick(string info)
    {

        if (buttonOk)
        {
            StartCoroutine(WaitTime());
            if (info == "下载")
            {
                connectThread = new Thread(new ThreadStart(CouldDown));
                connectThread.Start();
            }
            else if(info=="上传")
            {
                connectThread = new Thread(new ThreadStart(UpdateCould));
                connectThread.Start();
            }
            else if (info == "设置")
            {
                settingButtonCount += 1;
                if (settingButtonCount == 1)//打开
                {
                    settingIPInput.transform.gameObject.SetActive(true);
                    if(PlayerPrefs.GetString("IP")!=null)
                    {
                        settingIPInput.text = PlayerPrefs.GetString("IP");
                    }              
                }
                else if (settingButtonCount == 2)//关闭
                {
                    CoreManage.Instance.ServerIp = settingIPInput.text;
                    PlayerPrefs.SetString("IP", settingIPInput.text);
                    PlayerPrefs.Save();
                    settingIPInput.transform.gameObject.SetActive(false);
                    settingButtonCount = 0;
                }
            }
        }
        else
        {
            Debug.Log("请隔3秒后再试");
        }
    }

    IEnumerator WaitTime()
    {
        buttonOk = false;
        yield return new WaitForSeconds(3f);
        buttonOk = true;
        yield break;
    }

    void UpdateCould()
    {
        CoreManage.Instance.SaveData("todo");
        CoreManage.Instance.SaveData("clock");
        NetCon.UpdateCould();
    }



    void CouldDown()
    {
        NetCon.CouldDown();
        //下载完成后应该加载一遍,用bool控制update
        CoreManage.Instance.todoDownOk = true;
        CoreManage.Instance.clockDownOk = true;

        #region 之前的废弃代码，之前与python通信，需要手动转义；现在服务端用.netCore实现，通过IPAddress.HostToNetworkOrder转换
        ////发送完下载命令后，准备接收服务端发过来的数据
        //string recvStr = "";
        //byte[] recvBytes = new byte[10240];
        //int bytes;
        //bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
        //recvStr += Encoding.UTF8.GetString(recvBytes, 0, bytes);    

        ////格式化字符串，因为从服务端传过来的数据太乱了
        //Debug.Log("从服务端获取的数据为：" + recvStr);
        //string contents = CoreManage.Instance.Decodeing(recvStr);
        //contents = contents.Replace(@"\\n", "\n");
        //contents = contents.Replace(@"\\", "");
        //contents = contents.Replace(@"\", "");
        //contents = contents.Replace("\'\"", "");
        //contents = contents.Replace("\"\'", "");
        //Debug.Log("从服务端获取的数据解析后为：" + contents);
        //File.WriteAllText(CoreManage.Instance.todoFilePath, contents);
        //clientSocket.Close();
        //downOk = true;

        //if (connectThread != null)
        //{
        //    connectThread.Interrupt();
        //    connectThread.Abort();
        //}
        #endregion

    }
}

