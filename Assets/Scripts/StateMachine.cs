using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    // A state machine handles which state is active and when to transition to/from states.
    // Since it's gonna be a component on my gameobject, i wanna keep it a Monobehavior.
    // It might be helpful to think of a stack of states, like a stack of books only instead of books you have a stack of conditions and knowing the order of these conditions it can determin what to do, If a aggressive condition is on top, it knows to be aggresive, if a calm condition is on top it knows to be calm and so fourth.

    // This state machine is gonna be a Stacked Finite State Machine. which means it's gonna have a stack of states (fx, Idle/Ledig, Chase/Jagt, Attack/Angrib) 

    //// Fields:
    //From System.Collections.Generic I take in "Stack", Im keeping it generic so I can pass in a type. It is gonna be a Stack of states. Im also gonna make it a property with an auto implemented getter and setter. It could also be a simple field.
    public Stack<State> States { get; set; }

    // I want the stack to be the first thing intialised, thus using the "Awake" method (initialising my (empty) stack before the "Start" method.
    private void Awake()
    {
        // Creating an empty "Stack" of "State"(s)
        States = new Stack<State>();
    }

    // Getting the Current "Active" state I can then use that to execute said State. I want to invoke the ActiveAction on that current active State every single frame, thus I use Update.
    private void Update()
    {
        // First I check again if I do have a State in my Stack.
        if (GetCurrentState() != null)
        {
            // If I do have a current State, grab that States "ActiveAction" and Invoke it every frame.
            GetCurrentState().ActiveAction.Invoke();
        }
    }

    // A push is gonna take a State and put it on the top of the Stack. It's public because I will use it outside of StateMachine. PushState is gonna use the "ActiveAction", "OnEnterAction", and the "OnExitAction" to build the new Top State.
    public void PushState(System.Action active, System.Action onEnter, System.Action onExit)
    {
        if (GetCurrentState() != null)
            // If the CurrentState going in is not null I want to make sure to call OnExit on that State because it is being pushed down thus Exiting it's State. Goodbye current State.
            GetCurrentState().OnExit();

        // Constructing the new State.
        State state = new State(active, onEnter, onExit);
        // State.Push puts the new State on top of the Stack.
        States.Push(state);

        // Call the new States, OnEnter method because the new State is on top of my Stack and is therefore my active State.
        GetCurrentState().OnEnter();
    }

    // PopState is gonna take the State on the top and remove it, so it will fall back to the next State or whatever the last State was.
    public void PopState()
    {
        // Before we pop the current Active State, I call that States OnExit method, but first I have to check if I even have a State.
        if (GetCurrentState() != null)
        {
            GetCurrentState().OnExit();
            // To avoid problems I want to delete the ActiveAction from the previous State.
            GetCurrentState().ActiveAction = null;
            // Pop the State.
            States.Pop();

            // Now I just need to call the OnEnter method for whatever State was added to the top of the Stack by "Stack.Pop()"
            GetCurrentState().OnEnter();
        }

    }

    // Method to get the current/top State. Since it gonna be returning a State I will make it a method of type "State" in order to return a State type object, ie what State is on top IF I have a State in my Stack, if there is not a State in my Stack (thus no States in my Stack)
    // NOTE that NULL is a valid value in a Stack, this means that I don't just need to check what State is on top of the Stack I also need to check the count of the Stack to distinquish between a valid value and the bottom of the Stack.
    private State GetCurrentState()
    {
        // I treat the "States" field like a list thus I can say States.Count. The "?" represents a ternary operator, allowing me to define in-line if/else statements.
        // "Peek" looks/peeks on the top/first State and allow me to return that value. and if I do not have a state in my Stack return null.
        return States.Count > 0 ? States.Peek() : null;
    }
}
