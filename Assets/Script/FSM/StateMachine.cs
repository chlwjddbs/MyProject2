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

    public Dictionary<System.Type, State> states = new Dictionary<System.Type, State>();

    //�����ڸ� ���� State�� �ʱⰪ�� ������ �ش�.


    //Update ��Ȱ�� �� �Լ��� ����� �������� State�߿� Ȱ��ȭ�� State(currentState)�� �����Ѵ�.
    public virtual void Update()
    {
        currentState.OnUpdate();
    }

    //����� State�� ���
    public virtual void RegisterState(State setState)
    {
        //����� ������Ʈ�� �ڽ�(stateMachine)�� ������ �� �ְ� ���ش�.
        setState.SetStateMachine(this);

        //�Ѱܹ��� state�� ��ųʸ��� �����Ѵ�.
        states[setState.GetType()] = setState;
    }


    public virtual State ChangeState(State newState)
    {
        //���°� ��ȯ�ɶ� newType���� ��ȯ�Ͽ� ��ųʸ����� ����� ���¸� ã�Ƽ� ������.
        //�ٸ� State���� ChageState�� newState �Ű������� �Ѱ��ٶ� new IdleState������ �Ѱ� �ֱ� ������ newState�� �ٷ� ����ϸ� �׻� �ʱ�ȭ �� ������ �޴´�.
        //�׷��� ������ ��ųʸ����� Type�� Ű�� ����� State�� ������ ���� ��ȯ�� �����ش�.
        var newType = newState.GetType();
        if (newType == currentState?.GetType())
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

        return currentState;
    }
}