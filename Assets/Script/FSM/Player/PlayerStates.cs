using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : State
{
    protected Player player;
    protected Animator pAnim;
    protected PlayerState pState;

    public void SetStateMachine(Player _player, StateMachine _stateMachine)
    {
        player = _player;
        base.SetStateMachine(_stateMachine);
        //Initialize();
    }

    public override void Initialize()
    {
        pAnim = player.CallPlayerAnime();
    }

    public override void OnUpdate()
    {
        
    }
   
    public override void OnEnter()
    {
        pAnim.SetInteger("pState", (int)pState);
    }

    public virtual void ChangePAnime(PlayerState _playerState)
    {
        pAnim.SetInteger("pState", (int)_playerState);
    }
}
