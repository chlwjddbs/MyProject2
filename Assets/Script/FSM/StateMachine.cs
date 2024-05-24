using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine
{
    protected State currentState;
    public State CurrentState { get { return currentState; } }

    protected State previousState;
    public State PreviousState { get { return previousState; } }

    protected float elapsedTime = 0;
    public float ElapsedTime { get { return elapsedTime; } }

    public Dictionary<string, State> states = new Dictionary<string, State>();

    //ó���� ������ ��Ÿ���� �����Ȱɷ� �����ϱ� ���� ��Ÿ���� �� ä���ֱ� ���� ���Ǵ�Ƽ�� �⺻������ �־��ش�.
    protected float attackCoolTime = Mathf.Infinity;
    public float AttackCoolTime { get { return attackCoolTime; } }

    //�����ڸ� ���� State�� �ʱⰪ�� ������ �ش�.


    //Update ��Ȱ�� �� �Լ��� ����� �������� State�߿� Ȱ��ȭ�� State(currentState)�� �����Ѵ�.
    //Update�� ���� �������� �ʴ� ������ Enemy�� ������ ���̻� Update�� ���� �ʰ��ϱ� ���ؼ� Enemy�� Update���� �ҷ��ͼ� ����ϱ� �����̴�.
    public void Update(float _deltaTime)
    {       
        currentState.OnUpdate();
        elapsedTime += _deltaTime;
    }

    public void UpdateElapsedTime()
    {
        elapsedTime += Time.deltaTime;
    }

    public void AttackTimeCount()
    {
        attackCoolTime += Time.deltaTime;
    }

    public void ResetAttackCoolTime()
    {
        attackCoolTime = 0f;
    }

    //����� State�� ���
    public virtual void RegisterState(State setState)
    {
        //����� ������Ʈ�� �ڽ�(stateMachine)�� ������ �� �ְ� ���ش�.
        setState.SetStateMachine(this);

        //�Ѱܹ��� state�� ��ųʸ��� �����Ѵ�.
        states[setState.ToString()] = setState;
    }


    public virtual State ChangeState(State newState)
    {
        //���°� ��ȯ�ɶ� newType���� ��ȯ�Ͽ� ��ųʸ����� ����� ���¸� ã�Ƽ� ������.
        //�ٸ� State���� ChageState�� newState �Ű������� �Ѱ��ٶ� new IdleState������ �Ѱ� �ֱ� ������ newState�� �ٷ� ����ϸ� �׻� �ʱ�ȭ �� ������ �޴´�.
        //�׷��� ������ ��ųʸ����� Type�� Ű�� ����� State�� ������ ���� ��ȯ�� �����ش�.
        var newType = newState.ToString();
        if (newType == currentState?.ToString())
        {
            return currentState;
        }

        if (currentState != null)
        {
            currentState.OnExit();
        }

        previousState = currentState;
        currentState = states[newType];
        currentState.OnEnter();
        elapsedTime = 0f;

        return currentState;
    }

    public void LoadData(string _loadState, float _loadAttackCoolTime)
    {
        ChangeState(states[_loadState]);
        attackCoolTime = _loadAttackCoolTime;
    }
}