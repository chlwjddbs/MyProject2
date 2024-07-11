using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogUI : MonoBehaviour
{
    private DialogManager dialogManager;
    public Image talkerImage;
    public TextMeshProUGUI talkerName;
    public TextMeshProUGUI sentenceArea;
    public Button nextSentenceButton;

    private Queue<Dialog> dialogs;
    private Dialog dialog;

    private bool isOpen;
    public RectTransform dialogRect;

    private bool typing;

    // Start is called before the first frame update
    void Start()
    {
        dialogManager = DialogManager.instance;
        ResetDialogUI();
        CloseUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOpen)
        {
            return;
        }

        DialogControll();
    }

    public void OpenUI()
    {
        isOpen = true;
        dialogRect.anchoredPosition = Vector3.zero;
    }

    public void CloseUI()
    {
        isOpen = false;
        dialogRect.anchoredPosition = new Vector3(0, -1080f, 0);
    }

    public void ResetDialogUI()
    {
        talkerImage.sprite = null;
        talkerName.text = null;
        sentenceArea.text = null;
    }

    public void StartDialog(Queue<Dialog> _dialogs)
    {
        dialogs = _dialogs;
        OpenUI();
        DrawDialog();
    }

    public void DrawDialog()
    {
        if (dialogs.Count == 0)
        {
            EndDialog();
            return;
        }

        ResetDialogUI();

        dialog = dialogs.Dequeue();

        talkerImage.sprite = Resources.Load<Sprite>("Dialog/CharacterImage/" + dialog.talkerImage);

        talkerName.text = dialog.talkerName;

        //sentenceArea.text = dialog.sentence;

        //대화문 타이핑 연출
        StartCoroutine(typingSentence(dialog.sentence));
    }

    IEnumerator typingSentence(string typingText)
    {
        typing = true;

        sentenceArea.text = "";

        foreach (char latter in typingText)
        {
            sentenceArea.text += latter;
            yield return new WaitForSeconds(0.05f);
        }

        yield return null;
        typing = false;
    }

    public void DrawAllSentece()
    {
        if (typing)
        {
            typing = false;
            StopAllCoroutines();
            sentenceArea.text = dialog.sentence;
        }
    }

    public void NextSentence()
    {
        if (!typing)
        {
            DrawDialog();
        }
    }

    public void DialogControll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (typing)
            {
                DrawAllSentece();
            }
            else
            {
                NextSentence();
            }
        }
    }

    private void EndDialog()
    {
        //이어지는 대화가 있는지 체크
        if (dialog.nextlogNum != -1)
        { 
            dialogManager.StartDialog(dialogManager.curXmlFile, dialog.nextlogNum);
        }
        else
        {
            CloseUI();
        }
    }
}
