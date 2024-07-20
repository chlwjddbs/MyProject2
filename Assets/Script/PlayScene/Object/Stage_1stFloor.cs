using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stage_1stFloor : StageManager
{   
    //public Enemy_FSM Olaf;
    //public Enemy_FSM Gubne;

    public Area_C_Gimmick c_Gimmick;

    //public List<Enemy_FSM> testSaveLoad = new List<Enemy_FSM>();

    public override void NewStage()
    {
        startStage = "1st_Floor";
        currentStage = startStage;
        startArea = "≈æ ¿‘±∏";
        currentArea = startArea;
    }

    public override void SetEnemy()
    {
        for (int i = 0; i < stageEnemy.Count; i++)
        {
            stageEnemy[i].SetData();
        }

        /*
        for (int i = 0; i < testSaveLoad.Count; i++)
        {
            testSaveLoad[i].SetData();
            Olaf.SetData();
            Gubne.SetData();
        }
        */
    }

    public override void SetNpc()
    {
        for (int i = 0; i < stageNpc.Count; i++)
        {
            stageNpc[i].SetData();
        }
    }

    public override void SaveData()
    {
        base.SaveData();
        c_Gimmick.SaveData();
    }

    public override void SaveEnemy()
    {
        for (int i = 0; i < stageEnemy.Count; i++)
        {
            try
            {
                gameData.userData.eDatas_1F[i] = stageEnemy[i].SaveData();
            }
            catch
            {
                gameData.userData.eDatas_1F.Add(stageEnemy[i].SaveData());
            }
        }
    }

    public override void SaveItem()
    {
        for (int i = 0; i < startFieldItems.Count; i++)
        {
            if (startFieldItems[i] == null)
            {
                try
                {
                    gameData.userData.startFieldItem_1F[i] = false;
                }
                catch
                {
                    gameData.userData.startFieldItem_1F.Add(false);
                }
            }
            else
            {
                try
                {
                    gameData.userData.startFieldItem_1F[i] = true;
                }
                catch
                {
                    gameData.userData.startFieldItem_1F.Add(true);
                }
            }
        }
        gameData.userData.FiledItme_1F.Clear();

        for (int i = 0; i < dropItemManager.transform.childCount; i++)
        {
            AddItem saveItem = dropItemManager.transform.GetChild(i).GetComponentInChildren<AddItem>();
            gameData.userData.FiledItme_1F.Add(new DropItemManager.AddDropItem(saveItem, saveItem.transform.position));
        }
    }

    public override void LoadData()
    {
        base.LoadData();
        c_Gimmick.LoadData();
    }

    public override void LoadEnemy()
    {
        for (int i = 0; i < gameData.userData.eDatas_1F.Count; i++)
        {
            stageEnemy[i].LoadData(gameData.userData.eDatas_1F[i]);
        }

        /*
        for (int i = 0; i < testSaveLoad.Count; i++)
        {
            testSaveLoad[i].SetData();
            testSaveLoad[i].LoadData(gameData.userData.eDatas_1F_skeleton[i]);
        }
        */
    }

    public override void LoadItem()
    {
        for (int i = 0; i < gameData.userData.startFieldItem_1F.Count; i++)
        {
            if (!gameData.userData.startFieldItem_1F[i])
            {
                Destroy(startFieldItems[i]);
            }
        }

        for (int i = 0; i < gameData.userData.FiledItme_1F.Count; i++)
        {
            GameObject field = Instantiate(gameData.userData.FiledItme_1F[i].fieldItem.FieldObject, dropItemManager.transform);
            field.transform.position = gameData.userData.FiledItme_1F[i].itemPos;
            field.GetComponentInChildren<AddItem>().quantity = gameData.userData.FiledItme_1F[i].quantity;
        }
    }

    public override void LoadQuest()
    {
        base.LoadQuest();
    }

    public override void LoadNpc()
    {
        for (int i = 0; i < stageNpc.Count; i++)
        {
            stageNpc[i].LoadData();
        }
    }
}
