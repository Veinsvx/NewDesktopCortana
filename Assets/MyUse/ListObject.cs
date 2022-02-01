using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListObject : MonoBehaviour
{
    public string objName;
    public int index;

    public GameObject writeSonInfo;
    public GameObject showSonInfo;
    public int countSon=0;//添加子菜单数量

    public List<SubListClass> sublistcalss;
    public List<SubListObject> subListObjects;

    public GameObject myWriteSon;
    int ccount=0;//用来记录用户按下按键次数  

    private void Start()
    {
        this.GetComponentInChildren<Text>().text = objName;
        myWriteSon = Instantiate(writeSonInfo, this.transform.parent);
        myWriteSon.transform.GetComponentInChildren<Button>().onClick.AddListener(ButtonClickAddTreeInfoOK);
        myWriteSon.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);
        myWriteSon.SetActive(false);
    }


    public void setObjectInfo(string name, int index)
    {
        this.objName = name;
        this.index = index;
    }


    public void ButtonClickAddTreeInfoOK()
    {
        //用户输入完成信息后
        CreateSubListItem(myWriteSon.GetComponentInChildren<InputField>().text,
             false, countSon,true);
        myWriteSon.GetComponentInChildren<InputField>().text = "";       
    }

    private void CreateSubListItem(string temp, bool isok,int loadIndex = 0,bool addClass=false)
    {
        GameObject subtemp = Instantiate(showSonInfo, this.transform.parent);
        SubListObject subtempObj = subtemp.GetComponent<SubListObject>();
        countSon += 1;
        subtemp.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1 + countSon);
        if(addClass==true)
        {
            SubListClass subtempClas = new SubListClass(temp, loadIndex, isok);
            sublistcalss.Add(subtempClas);
        }  
        int index = sublistcalss.Count;
        subtempObj.setSubObjectInfo(temp, loadIndex, isok,this.gameObject);
        subListObjects.Add(subtempObj);      
        CoreManage.Instance.SaveData("todo");
    }

    public void ButtonClickTree()
    {
        //检测用户按下次数，第一次按出现树形菜单，第二次隐藏
        ccount += 1;
        if(ccount==1)
        {
            ButtonClickTreeShow();
        }
        else if(ccount==2)
        {
            ButtonClickTreeHide();
            ccount = 0;
        }
    }

    public void ButtonClickTreeShow()
    {
        //调节content列表


        //生成子菜单
        myWriteSon.SetActive(true);

        for (int i = 0; i < sublistcalss.Count; i++)
        {
            CreateSubListItem(sublistcalss[i].objName, sublistcalss[i].isok, sublistcalss[i].index,false);
        }
    }

    private void ButtonClickTreeHide()
    {
        myWriteSon.SetActive(false);
        for (int i = subListObjects.Count; i > 0; i--)
        {
            Destroy(subListObjects[i-1].transform.gameObject);
        }
        subListObjects.Clear();
        countSon = 0;
    }

}
