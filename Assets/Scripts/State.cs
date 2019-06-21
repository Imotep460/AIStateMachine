using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class State
{
    // In order to assign a method to a field, to do this I want to use a delegate
    // a simple way to do this is using the "Action" as it handels the creation of the delegate itself.
    public Action ActiveAction;
    public Action OnEnterAction;
    public Action OnExitAction;

    // the action can also be setup like this:
    //public Action ActiveAction, OnEnterAction, OnExitAction;


    // Setup constructor to take the parameters and assign them to our fields for state.
    public State(Action active, Action onEnter, Action onExit)
    {
        ActiveAction = active;
        OnEnterAction = onEnter;
        OnExitAction = onExit;
    }

    // While a state is active in the statemachine call the Execute method to invoke the ActiveAction. Thus doing whatever the (specific) state says we can do.
    public void Execute()
    {
        if (ActiveAction != null)
            ActiveAction.Invoke();
    }

    // Whenever a new state is added in the statemachine call OnEnter method to invoke the OnEnterAction.
    public void OnEnter()
    {
        if (OnEnterAction != null)
            OnEnterAction.Invoke();
    }

    // Whenever a state is removed from the state machine call the OnEcit method to invoke the OnExitAction.
    public void OnExit()
    {
        if (OnExitAction != null)
            OnExitAction.Invoke();
    }
}
