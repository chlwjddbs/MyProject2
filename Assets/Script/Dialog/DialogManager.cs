using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private QuestManager questManager;
    
    public string curXmlFile;

    //xml
    private XmlNodeList allNodes;
    private Dictionary<string, XmlNodeList> nodeAll = new Dictionary<string, XmlNodeList>();
    public Queue<Dialog> dialogs = new Queue<Dialog>();

    private string dialogSentence;

    public bool isOpen = false;

    public DialogUI dialogUI;

    private void Start()
    {
        questManager = QuestManager.instance;
        //LoadDialogXml(xmlFile);
    }

    public void RegistDialogXml(string fileName)
    {
        TextAsset xmlFile = Resources.Load(fileName) as TextAsset;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlFile.text);
        allNodes = xmlDoc.SelectNodes("root/dialog");
        nodeAll[fileName] = xmlDoc.SelectNodes("root/dialog");
    }

    public void StartDialog(string xmlName, int dialogNumber)
    {
        curXmlFile = xmlName;
        //dialogNumber와 일치하는 모든 node를 queue에 저장하여 대화내용을 하나씩 보여줄수 있도록 한다.
        foreach (XmlNode node in nodeAll[xmlName])
        {
            int num = int.Parse(node["number"].InnerText);
            if (num == dialogNumber)
            {
                Dialog dialog = new Dialog();
                dialog.logNum = num;
                dialog.talkerImage = node["character"].InnerText;
                dialog.talkerName = node["name"].InnerText;
                dialog.sentence = node["sentence"].InnerText;
                dialog.nextlogNum = int.Parse(node["next"].InnerText);

                dialogs.Enqueue(dialog);
            }
        }
        dialogUI.StartDialog(dialogs);
    }
}
