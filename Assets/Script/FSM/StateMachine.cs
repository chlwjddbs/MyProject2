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

    //처음은 무조건 쿨타임이 충족된걸로 판정하기 위에 쿨타임을 다 채워주기 위에 인피니티를 기본값으로 넣어준다.
    protected float attackCoolTime = Mathf.Infinity;
    public float AttackCoolTime { get { return attackCoolTime; } }

    //생성자를 통해 State의 초기값을 설정해 준다.


    //Update 역활을 할 함수를 만들어 여러가지 State중에 활성화된 State(currentState)만 적용한다.
    //Update를 직접 구현하지 않는 이유는 Enemy가 죽으면 더이상 Update가 돌지 않게하기 위해서 Enemy쪽 Update에서 불러와서 사용하기 때문이다.
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

    //사용할 State를 등록
    public virtual void RegisterState(State setState)
    {
        //등록할 스테이트에 자신(stateMachine)을 연결할 수 있게 해준다.
        setState.SetStateMachine(this);

        //넘겨받은 state를 딕셔너리에 저장한다.
        states[setState.ToString()] = setState;
    }


    public virtual State ChangeState(State newState)
    {
        //상태가 전환될때 newType으로 변환하여 딕셔너리에서 저장된 상태를 찾아서 꺼낸다.
        //다른 State에서 ChageState로 newState 매개변수를 넘겨줄때 new IdleState등으로 넘겨 주기 때문에 newState를 바로 사용하면 항상 초기화 된 정보를 받는다.
        //그렇게 때문에 딕셔너리에서 Type을 키로 저장된 State를 가져와 상태 변환을 시켜준다.
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