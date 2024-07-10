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

    QuestManager questManager;

    public string curXmlFile;

    //xml
    private XmlNodeList allNodes;
    private Dictionary<string, XmlNodeList> nodeAll = new Dictionary<string, XmlNodeList>();
    public Queue<Dialog> dialogs = new Queue<Dialog>();

    //UI
    public TextMeshProUGUI nameText;
    public Text sentenceText;
    public GameObject nextButton;
    public int nextDialog = -1;

    private string dialogSentence;

    public bool isOpen = false;
    public GameObject dialogUI;

    private QuestUI questUI;

    private void Start()
    {
        questManager = QuestManager.instance;
        //LoadDialogXml(xmlFile);
        ResetDialog();
        questUI = GetComponent<QuestUI>();
    }

    private void ResetDialog()
    {
        dialogs.Clear();

        //nameText.text = "";
        //sentenceText.text = "";
        //dialogSentence = "";

        //nextButton.SetActive(false);
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
        ResetDialog();
        curXmlFile = xmlName;
        //dialogNumber�� ��ġ�ϴ� ��� node�� queue�� �����Ͽ� ��ȭ������ �ϳ��� �����ټ� �ֵ��� �Ѵ�.
        foreach (XmlNode node in nodeAll[xmlName])
        {
            int num = int.Parse(node["number"].InnerText);
            if (num == dialogNumber)
            {
                Dialog dialog = new Dialog();
                dialog.logNum = num;
                dialog.imageNum = int.Parse(node["character"].InnerText);
                dialog.talkerName = node["name"].InnerText;
                dialog.sentence = node["sentence"].InnerText;
                dialog.nextlogNum = int.Parse(node["next"].InnerText);

                dialogs.Enqueue(dialog);
            }
        }

        //ù��° ��ȭ�� �����ش�.
        //StartCoroutine(OpenUI());
    }

    IEnumerator OpenUI()
    {
        isOpen = true;
        dialogUI.GetComponent<Animator>().SetBool("isOpen", true);

        yield return new WaitForSeconds(0.33f);

        DrawNext();
    }

    IEnumerator CloseUI()
    {
        isOpen = false;
        dialogUI.GetComponent<Animator>().SetBool("isOpen", false);

        if (questManager.currentQuest.questState == QuestState.Ready || questManager.cuurentState == QuestState.Complete)
        {
            //questUI.OpenUI();
        }
        yield return new WaitForSeconds(0.33f);

        ResetDialog();
    }

    public void DrawNext()
    {
        if (dialogs.Count == 0)
        {
            EndDialog();
            return;
        }

        nextButton.SetActive(false);

        //��ȭ ���� ����
        Dialog dialog = dialogs.Dequeue();

        nameText.text = dialog.talkerName;

        dialogSentence = dialog.sentence;

        nextDialog = dialog.nextlogNum;

        //��ȭ�� Ÿ���� ����
        StartCoroutine(typingSentence(dialog.sentence));
    }

    IEnumerator typingSentence(string typingText)
    {
        sentenceText.text = "";

        foreach (char latter in typingText)
        {
            sentenceText.text += latter;
            yield return new WaitForSeconds(0.05f);
        }

        nextButton.SetActive(true);
    }

    public void DrawSentece()
    {
        if (dialogSentence == "")
            return;

        StopAllCoroutines();
        sentenceText.text = dialogSentence;
        nextButton.SetActive(true);
    }

    private void EndDialog()
    {
        //���� ��ȭ�� �ִ��� üũ
        if (nextDialog > -1)
        {
            StartDialog(curXmlFile, nextDialog);
        }
        else 
        {
            StartCoroutine(CloseUI());
        }
    }
}
