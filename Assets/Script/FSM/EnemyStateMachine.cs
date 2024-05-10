using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    private Enemy_FSM enemy;
    public EnemyStateMachine(Enemy_FSM _enemy, State initState)
    {
        enemy = _enemy;

        //초기 상태 등록
        RegisterEState(initState);
        //현재 상태를 초기 상태로 등록
        currentState = initState;

        //초기 상태 시작
        currentState.OnEnter();
        elapsedTime = 0;
    }

    public void RegisterEState(State setState)
    {
        ((EnemyStates)setState).SetEStateMachine(enemy,this);

        states[setState.GetType()] = setState;
    }
}


