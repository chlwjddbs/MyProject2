using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class DeathEState : EnemyStates
{
    private NavMeshAgent agent;

    public override void Initialize()
    {
        base.Initialize();
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnEnter()
    {
        eAnim.SetBool("isDeath", enemy.isDeath);
        //PlayEnemySound(deadSound);
        enemy.Target.GetComponent<PlayerStatus>().AddExp(enemy.Exp);
        enemy.enemyMark.SetActive(false);
        enemy.hitBox.enabled = false;
        agent.enabled = false;
        enemy.enabled = false;
    }
}
