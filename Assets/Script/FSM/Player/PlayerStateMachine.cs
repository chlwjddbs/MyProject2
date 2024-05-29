using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    private Player player;
    public PlayerStateMachine(Player _player, State _initState)
    {
        player = _player;

        //초기 상태 등록
        RegisterPState(_initState);
        //현재 상태를 초기 상태로 등록
        currentState = _initState;

        //초기 상태 시작
        currentState.OnEnter();
        elapsedTime = 0;
    }

    public void RegisterPState(State setState)
    {
        ((PlayerStates)setState).SetStateMachine(player, this);

        states[setState.ToString()] = setState;
    }
}
