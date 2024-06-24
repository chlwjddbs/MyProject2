using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerAnimControl : MonoBehaviour
{
    //public PlayerController player;
    //public GameObject weaponSlot;

    public Player player;
    private int actionState;

    private AudioManager audioManager;
    //public Sound[] playerSound;

    public Animator playerAnime;

    public void SetData()
    {
        audioManager = AudioManager.instance;

        actionState = Animator.StringToHash("Base Layer.Footman_Action");
    }

    public void StartAttack()
    {     
        player.AttackCollider.enabled = true;
    }

    public void EndAttack()
    {
        player.ResetAttack();
        player.SetDamage();
    }

    public void SetCastStay()
    {
        playerAnime.SetFloat("MotionProccess", 0.5f);
    }

    public void ResetCastMotion()
    {
        playerAnime.SetFloat("MotionProccess", 0f);
        player.ChangeState(new IdlePState());
    }

    public void Replay(float _point)
    {
       playerAnime.Play("Footman_Action", -1, _point);
    }

    public void CheckNomalized()
    {
        Debug.Log(playerAnime.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    public void PlayerSound(string _playerSound)
    {
        audioManager.PlayExternalSound(_playerSound);
    }

    public void ChangeState(State _newState)
    {
        player.ChangeState(_newState);
    }
}
/*
    private void Update()
    {       
        if(playerAnime.GetCurrentAnimatorStateInfo(0).fullPathHash == actionState)
        {
            EndAction();
        }
    }

    public void EndAction()
    {
        if (playerAnime.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.85)
        {
            player.isAction = false;
            //player.SetState(PlayerState.Idle);
        }
    }

    public void CheckAction()
    {
        player.isAction = false;
    }

     public void CheckAttack()
   {
       weaponSlot.GetComponent<Weapon>().SetAttackDamage();
   }
*/