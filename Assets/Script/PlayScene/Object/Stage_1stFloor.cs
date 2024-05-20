using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stage_1stFloor : StageManager
{   
    public Enemy Olaf;
    public Enemy Gubne;

    public Area_C_Gimmick c_Gimmick;

    public List<Enemy_FSM> testSaveLoad = new List<Enemy_FSM>();

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

        for (int i = 0; i < testSaveLoad.Count; i++)
        {
            testSaveLoad[i].SetData();
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
                gameData.userData.eDatas_1F[i] = stageEnemy[i].SaveState();
            }
            catch
            {
                gameData.userData.eDatas_1F.Add(stageEnemy[i].SaveState());
            }
        }

        for (int i = 0; i < testSaveLoad.Count; i++)
        {
            try
            {
                gameData.userData.eDatas_1F_skeleton[i] = testSaveLoad[i].SaveData();

            }
            catch
            {

                gameData.userData.eDatas_1F_skeleton.Add(testSaveLoad[i].SaveData());
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

        /*
        Olaf.LoadState(0);
        Gubne.LoadState(0);

        for (int i = 0; i < spawnEnemy.Count; i++)
        {
            spawnEnemy[i].LoadState(i);
        }
        */
    }

    public override void LoadEnemy()
    {
        for (int i = 0; i < stageEnemy.Count; i++)
        {
            stageEnemy[i].LoadState(gameData.userData.eDatas_1F[i]);
            stageEnemy[i].isSet = true;
        }

        for (int i = 0; i < testSaveLoad.Count; i++)
        {
            testSaveLoad[i].SetData();
            testSaveLoad[i].LoadData(gameData.userData.eDatas_1F_skeleton[i]);
        }
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
}
