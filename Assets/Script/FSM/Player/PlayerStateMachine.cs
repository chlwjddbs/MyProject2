using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    private Player player;
    public PlayerStateMachine(Player _player, State _initState)
    {
        player = _player;

        //�ʱ� ���� ���
        RegisterPState(_initState);
        //���� ���¸� �ʱ� ���·� ���
        currentState = _initState;

        //�ʱ� ���� ����
        currentState.OnEnter();
        elapsedTime = 0;
    }

    public void RegisterPState(State setState)
    {
        ((PlayerStates)setState).SetStateMachine(player, this);

        states[setState.ToString()] = setState;
    }
}
