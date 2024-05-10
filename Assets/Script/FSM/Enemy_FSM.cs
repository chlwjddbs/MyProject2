using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FSM : MonoBehaviour
{
    protected EnemyStateMachine eStateMachine;
    protected Animator eAnime;

    private SearchPlayer searchPlayer;

    public Transform Target { get { return searchPlayer.Target; } }

    public float TargetDis { get { return searchPlayer.TargetDis; } }
    public Vector3 TargetDir { get { return searchPlayer.TargetDir; } }
    protected float rotateSpeed = 7f;

    protected float attackRange = 3.5f;
    public float AttackRange { get { return attackRange; } }

    public float DetectRange { get { return searchPlayer.DetectRange; } } //15f;

    protected float reactionRange = 30f;
    public float ReactionRange { get { return reactionRange; } }

    public bool VisibleTarget { get { return searchPlayer.VisibelTarget; } }

    protected bool isDeath = false;

    protected virtual void Start()
    {
        eAnime = GetComponent<Animator>();
        eStateMachine = new EnemyStateMachine(this, new IdleEState());
        eStateMachine.RegisterEState(new MoveEState());
        eStateMachine.RegisterEState(new AttackEState());

        searchPlayer = GetComponent<SearchPlayer>();
    }

    protected virtual void Update()
    {
        eStateMachine.Update();
    }

    public Animator CallEnemyAnime()
    {
        return eAnime;
    }

    public State ChangeState(State newState)
    {
        return eStateMachine.ChangeState(newState);
    }

    public void EndAttack()
    {

    }

    public void PlayEnemySound()
    {

    }

    public void LookRotate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(TargetDir), Time.deltaTime * rotateSpeed);
    }
}
