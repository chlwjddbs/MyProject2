using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Localization.Components;

public class SkillBook : MonoBehaviour
{
    [System.Serializable]
    public class SetEquipSkill
    {
        public string keyCode;
        public SkillItem equipSkill;
        public float coolTime;

        public SetEquipSkill(string _keycode,SkillItem _skill = null , float _coolTime = 0)
        {
            keyCode = _keycode;
            equipSkill = _skill;
            coolTime = _coolTime;
        }
    }

    public static SkillBook instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //��� ��ų ����Ʈ
    public List<SkillItem> learnedSkill;

    //���� �� ��ų
    public SkillItem[] equipSkill;

    public GameObject skillPage;
    public GameObject skillSlot;
    public GameObject skillButtons;
    public SkillButton[] button;

    //��ų �������� ��� ������ ��ų�� ����
    public int maxAmount = 5;

    public LocalizeStringEvent sequencetextPrefab;
    public string s_text;
   
    private float countdown;
    private float coolTime = 4;

    public Button prevButton;
    public Button nextButton;
    public TextMeshProUGUI pageText;
    private int pageInt =1;

    private SkillBookUI skillbookUI;

    private bool usetext = true;

    public Dictionary<int, SetEquipSkill> skillButtonInfo = new Dictionary<int, SetEquipSkill>();

    public Sound[] pageSound;

    private void Start()
    {
        foreach (var s in pageSound)
        {
            AudioManager.instance.AddExternalSound(s);
        }
    }

    public void SetData()
    {
        //������ ��ų�� ������ ��밡���� ��ų ��ư�� �����̴�. (���� [A,S,D,F,G]�� 5��)
        equipSkill = new SkillItem[skillButtons.transform.childCount];

        //���� ��� ������ ��ų ��ư ��������
        button = skillButtons.GetComponentsInChildren<SkillButton>();

        AudioManager.instance.skillSound = new Sound[skillButtons.transform.childCount];

        skillbookUI = GetComponentInParent<SkillBookUI>();
        skillbookUI.SetData();

        if (DataManager.instance.newGame)
        {
            for (int i = 0; i < button.Length; i++)
            {
                button[i].SetData();
            }
        }
        else
        {
            LoadData();
            //Debug.Log(learnedSkill[0].skillName + learnedSkill[0].skillType + learnedSkill[0].skill);
        }

    }

    public void LoadData()
    {
        learnedSkill = new List<SkillItem>();
        //equipSkill = (SkillItem[])(DataManager.instance.userData.equipSkill).Clone();

        for (int i = 0; i < DataManager.instance.userData.learedSkill.Count; i++)
        {
            LearnSkill(DataManager.instance.userData.learedSkill[i]);
        }
        skillbookUI.CloseUI();

        for (int i = 0; i < skillButtons.transform.childCount; i++)
        {
            SetEquipSkill x = DataManager.instance.userData.skillButtonInfo[i];
            skillButtonInfo.Add(i, new SetEquipSkill(x.keyCode, x.equipSkill, x.coolTime));
            button[i].SetData();
        }
    }

    public void SaveData()
    {
        DataManager.instance.userData.learedSkill = learnedSkill;
        //DataManager.instance.userData.equipSkill = equipSkill;
        for (int i = 0; i < button.Length; i++)
        {
            button[i].SaveData();
            if (DataManager.instance.userData.skillButtonNum.Contains(i))
            {
                DataManager.instance.userData.skillButtonInfo[i] = skillButtonInfo[i];
            }
            else
            {
                DataManager.instance.userData.skillButtonNum.Add(i);
                DataManager.instance.userData.skillButtonInfo.Add(skillButtonInfo[i]);
            }
        }
        
    }

    /*
    private void Update()
    {
        if (countdown > 0)
        {
            countdown -= Time.deltaTime;
        }
    }
    */


    public bool isLearn(SkillItem _skillItem)
    {
        //��� ��ų ��Ͽ� �ִ� ��ų�̸�
        if (learnedSkill.Contains(_skillItem))
        {
            /*
            //���̻� ��� �� ����
            if (countdown <= 0)
            {
                SequenceText.instance.SetSequenceText(sequencetextPrefab, s_text);
                countdown = coolTime;
            }
            */

            if (usetext)
            {
                usetext = false;
                SequenceText.instance.SetSequenceText(sequencetextPrefab, s_text);
                Invoke("isUseText",4);

            }
            return false;
        }
        //��Ͽ� ���� ��ų�̸�
        else
        {
            //��� �� ����
            return true;
        }
    }

    public void LearnSkill(SkillItem _skillItem)
    {
        //������ ��ų ��Ͽ� �߰�
        learnedSkill.Add(_skillItem);
        skillbookUI.OpenUI();
        //���ο� ��ų�� ����� ��ų�� ������ �������� ������ (��ų���� ������ ������)
        Transform _skillPage = transform.GetChild(transform.childCount - 1);

        //��ų �������� ���ٸ� ����.
        if(_skillPage == null)
        {
            Instantiate(skillPage, transform);
            Debug.Log("��ų ����");
        }
        
        //��ų �������� ��ϵ� ��ų�� ������ �ִ� ���� ������ ���� �ʾ��� ��
        if (_skillPage.childCount < maxAmount)
        {
            //��ų ����
            GameObject _skillPrefab = Instantiate(skillSlot, transform.GetChild(transform.childCount - 1));
            SkillSlot _skillSlot = _skillPrefab.GetComponent<SkillSlot>();
            _skillSlot.AddSkill(_skillItem);

            transform.GetChild(int.Parse(pageText.text) - 1).gameObject.SetActive(false);
            pageText.text = transform.childCount.ToString();
            transform.GetChild(int.Parse(pageText.text) - 1).gameObject.SetActive(true);

        }
        //��ų �������� ��ϵ� ��ų�� ������ �ִ� ���� ������ �Ѿ��� ��
        else if (_skillPage.childCount == 5 && _skillPage.GetComponent<SkillPage>().isPull == false)
        {
            _skillPage.GetComponent<SkillPage>().isPull = true;

            //���ο� ��ų ������ ����
            Instantiate(skillPage, transform);            

            //���ο� ��ų ������ ���� �� ��ų ����
            GameObject _skillPrefab = Instantiate(skillSlot, transform.GetChild(transform.childCount - 1));
            SkillSlot _skillSlot = _skillPrefab.GetComponent<SkillSlot>();
            _skillSlot.AddSkill(_skillItem);

            //�� ��ų �������� off�ϰ� ���ο� ��ų �������� �����ش�.
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                pageText.text = transform.childCount.ToString();
            }
        }
    }

    //��ų�� ������ �ش� ��ų�� ��ų�� ������ ��ư�� ��ȣ�� �޾ƿ´�.
    //_skillItem�� ������ ��ų , _dropButtonNum�� ������ ��ư(��ų�Ͽ��� ��ų�� �巡���Ͽ� ����� ��ư�� ��ȣ)
    public void isEquip(SkillItem _skillItem, int _dropButtonNum)
    {
        for (int i = 0; i < equipSkill.Length; i++)
        {
            //������ ��ų�� �̹� ������ ��ų�̰� 
            if (equipSkill[i] == _skillItem)
            {
                //_skillItem�� ���� skillItem�� ������ ��ġ(equipSkill[i]�� i)��
                //��ų ������ �õ��� ��ų ��ư�� ��ġ(_dropButtonNum) �� ���ٸ�
                if (i == _dropButtonNum)
                {
                    //���� ĭ�� ���� ��ų�� �����϶�� �ϴ°��̱� ������ ��ȭ ����.

                    //button[i].UnequipSkill();
                    //equipSkill[i] = null;
                    break;
                }
                //���� �� ��ų�� ��ư�� �ִ� ��ų�� �ٸ��ٸ� (A��ų�� B��ų�� �ִ� ��ư�� �����Ϸ��� �Ѵٸ�)
                else
                {
                    //���� �� ��ư�� �ִ� ��ų�� ���� ���� ��ų�� ��ġ�� �ٲ��ش�. (A��ų�� B��ų�� �ִ� �ڸ��� �Ű��ְ�, B��ų�� A��ų�� �ִ� �ڸ��� �ű��.)
                    //Debug.Log(this.name);
                    button[i].swapSkill(button[_dropButtonNum]);
                }
                break;
            }
            //������ ��ų ��Ͽ� �ش� ��ų�� ���ٸ� (���ο� ��ų�� ������ ��)
            //for���� ���� ���� ������ ĭ�� ���޽�
            else if (i == equipSkill.Length - 1)
            {
                //������ĭ�� �˻��ؼ� ������ ��ų�� ���ų�, ��ų�� �־ ���� ������ ��ų�� �ƴҽ�
                //(���ο� ��ų�̶�� ��, �ߺ��ȴٸ� ������ ������ ��ų�̱� ������  if (equipSkill[i] == _skillItem) �������� �ɷ�����.)
                if (equipSkill[i] == null || equipSkill[i] != _skillItem)
                {
                    //��ų�� �����Ѵ�.
                    button[_dropButtonNum].UnequipSkill();
                    button[_dropButtonNum].EquipSkill(_skillItem);
                }
            }
        }
    }

    private void isUseText()
    {
        usetext = true;
    }

    public void PrevPage()
    {
        if (int.Parse(pageText.text) > 1)
        {
            transform.GetChild(int.Parse(pageText.text)-1).gameObject.SetActive(false);
            pageText.text = (int.Parse(pageText.text) - 1).ToString();
            transform.GetChild(int.Parse(pageText.text)-1).gameObject.SetActive(true);
            AudioManager.instance.PlayExSound("pageChage");
        }
    }

    public void NextPage()
    {
        if(int.Parse(pageText.text) < transform.childCount)
        {
            transform.GetChild(int.Parse(pageText.text)-1).gameObject.SetActive(false);
            pageText.text = (int.Parse(pageText.text) + 1).ToString();
            transform.GetChild(int.Parse(pageText.text)-1).gameObject.SetActive(true);
            AudioManager.instance.PlayExSound("pageChage");
        }
    }
    
}
