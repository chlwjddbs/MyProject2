using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    private Enemy_FSM enemy;
    public EnemyStateMachine(Enemy_FSM _enemy, State initState)
    {
        enemy = _enemy;

        //�ʱ� ���� ���
        RegisterEState(initState);
        //���� ���¸� �ʱ� ���·� ���
        currentState = initState;

        //�ʱ� ���� ����
        currentState.OnEnter();
        elapsedTime = 0;
    }

    public void RegisterEState(State setState)
    {
        ((EnemyStates)setState).SetEStateMachine(enemy,this);

        states[setState.GetType()] = setState;
    }
}


