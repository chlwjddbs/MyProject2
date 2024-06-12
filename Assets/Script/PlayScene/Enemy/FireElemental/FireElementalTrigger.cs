using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElementalTrigger : MonoBehaviour
{
    public Animator FireRoad;
    public ParticleSystem SpawnFireElementalEffect;
    public ParticleSystem FireAura;
    public FireElemental fireElemental;
    public SetCursorImage cursorAction;
    public GameObject spawnTrigger;

    public List<BoxCollider> gate;

    public GameObject Obstacle;

    private bool isDeath = false;

    public Sound[] sound;

    private void Start()
    {
        foreach (var s in sound)
        {
            AudioManager.instance.AddExternalSound(s);
        }
    }

    private void Update()
    {
        FireElementalDeath();

        if (!fireElemental.isDeath)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name == "SpawnTrigger")
                    {
                        if (cursorAction.theDistance < cursorAction.actionDis)
                        {
                            //fireElemental.FireAura.SetActive(true);
                            fireElemental.FireAura.Play();
                            fireElemental.enabled = true;
                            Destroy(cursorAction.gameObject, .3f);
                            StartCoroutine("SpawnFireElemental");
                        }
                    }

                }
            }
        }
        /*
        if (FireElemental.GetComponent<FireElemental>().isPase_2)
        {

        }
        */
    }

    IEnumerator SpawnFireElemental()
    {
        AudioManager.instance.StopBGM(1.5f);
        Obstacle.SetActive(true);
        FireRoad.SetBool("Ontrigger", true);
        LockTheGate();
        SpawnFireElementalEffect.Play();
        AudioManager.instance.PlayExternalSound("gn_awake");

        yield return new WaitForSeconds(1f);

        AudioManager.instance.PlayBGM("Fight_Gubne",1f);
        fireElemental.gameObject.SetActive(true); 
        FireAura.Play();
        yield return new WaitForSeconds(1f);

        SpawnFireElementalEffect.Stop();

        yield return new WaitForSeconds(1f);
        Destroy(SpawnFireElementalEffect);

        //fireElemental.isActive = true;
        fireElemental.enabled = true;
        fireElemental.HitBox.enabled = true;
    }

    void LockTheGate()
    {
        for (int i = 0; i < gate.Count; i++)
        {
            gate[i].isTrigger = true;
        }
    }

    void OpenTheGate()
    {
        for (int i = 0; i < gate.Count; i++)
        {
            gate[i].isTrigger = false;
        }
    }

    public void FireElementalDeath()
    {
        if (fireElemental.isDeath && !isDeath)
        {
            isDeath = true;
            FireRoad.SetBool("Offtrigger", true);
            Obstacle.SetActive(false);
            OpenTheGate();
            enabled = false;
            spawnTrigger.SetActive(false);
        }
    }
}
