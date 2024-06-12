using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceElementalTrigger : MonoBehaviour
{
    public IceElemental IceElemental;
    public ParticleSystem Snow;
    //전투 시작시 플레이어를 못 돌아가게 막음
    public GameObject obstacleWall;

    [SerializeField] private Collider awakeTrigger;

    private void Awake()
    {
        IceElemental.deathAction += IceElementalDeath;
        obstacleWall.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {      
        if(Player.isDeath)
        {
            AudioManager.instance.StopAm("Snow");
        }
    }

    public void IceElementalDeath()
    {
        Snow.Stop();
        AudioManager.instance.StopAm("Snow");
        obstacleWall.SetActive(false);
        awakeTrigger.enabled = false;
        AudioManager.instance.StopBGM(1f);
        AudioManager.instance.PlayBGM("PlayScene_Floor_1");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IceElemental.isDeath)
        {
            return;
        }

        Debug.Log(other.name);
        if (other.CompareTag("Player"))
        {
            IceElemental.SetAwake();
            StartCoroutine(ChangeBGM());
            obstacleWall.SetActive(true);
            IceElemental.iceEffect.SetActive(true);
            //IceElemental.isActive = true;
            IceElemental.PlayESound("encounterOlaf");
            //IceElemental.GetComponent<IceElementalAction>().enabled = true;
            Snow.Play();
            awakeTrigger.enabled = false;
        }
    }

    
    IEnumerator ChangeBGM()
    {
        AudioManager.instance.StopBGM(1f);
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlayBGM("Fight_Olf", 1f);
        AudioManager.instance.PlayAmSond("Snow");
    }
}
