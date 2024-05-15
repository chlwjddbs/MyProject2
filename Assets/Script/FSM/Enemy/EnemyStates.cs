using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStates : State
{
    //protected StateMachine stateMachine
    protected Enemy_FSM enemy;
    protected Animator eAnim;
    protected EnemyState enemyState;
  
    public void SetEStateMachine(Enemy_FSM _enemy , StateMachine _stateMachine)
    {
        enemy = _enemy;

        //stateMachine에 enemy 클래스로부터 받은 _stateMachine을 등록;
        base.SetStateMachine(_stateMachine);
    }
    public override void Initialize()
    {
        eAnim = enemy.CallEnemyAnime();
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnEnter()
    {
        eAnim.SetInteger("eState", (int)enemyState);
    }
}
