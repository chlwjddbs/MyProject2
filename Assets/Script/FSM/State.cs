using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected StateMachine stateMachine;

    public State() { }

    public virtual void SetStateMachine(StateMachine _stateMachine)
    {
        stateMachine = _stateMachine;

        Initialize();
    }

    public virtual void Initialize() { }
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public abstract void OnUpdate();

    public virtual void LoadData() { }
}