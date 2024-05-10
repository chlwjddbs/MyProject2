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

    //생성자를 통해 State의 초기값을 설정해 준다.


    //Update 역활을 할 함수를 만들어 여러가지 State중에 활성화된 State(currentState)만 적용한다.
    public virtual void Update()
    {
        currentState.OnUpdate();
    }

    //사용할 State를 등록
    public virtual void RegisterState(State setState)
    {
        //등록할 스테이트에 자신(stateMachine)을 연결할 수 있게 해준다.
        setState.SetStateMachine(this);

        //넘겨받은 state를 딕셔너리에 저장한다.
        states[setState.GetType()] = setState;
    }


    public virtual State ChangeState(State newState)
    {
        //상태가 전환될때 newType으로 변환하여 딕셔너리에서 저장된 상태를 찾아서 꺼낸다.
        //다른 State에서 ChageState로 newState 매개변수를 넘겨줄때 new IdleState등으로 넘겨 주기 때문에 newState를 바로 사용하면 항상 초기화 된 정보를 받는다.
        //그렇게 때문에 딕셔너리에서 Type을 키로 저장된 State를 가져와 상태 변환을 시켜준다.
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