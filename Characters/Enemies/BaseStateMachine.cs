using System.Collections.Generic;
using Godot;

namespace Game.State;

public partial class BaseStateMachine<T> : Node where T : State
{
    // Removed [Export] because Godot does not support exporting generic types.
    protected T initialState;
    protected Dictionary<string, T> states = new Dictionary<string, T>();    
    protected T currentState;

    public override void _Ready()
    {
        foreach (var child in GetChildren()) 
        {
            if (child is T state)
            {
                states[child.Name.ToString().ToLower()] = state;
                state.Transitioned += OnTransitioned;
            }
        }

        if (initialState != null)
        {
            initialState.Enter();
            currentState = initialState;
        }
        // PrintStates();
    }

    private void OnTransitioned(State EnemyState, string newStateName)
    {
        if (EnemyState != currentState)
        {
            return;
        }

        if (states.TryGetValue(newStateName.ToLower(), out var newState))
        {
            if (newState == null)
            {
                return;
            }
        }

        if (currentState != null)
        {
            currentState.Exit();
        }

        newState.Enter();

        currentState = newState;

    }

    public override void _Process(double delta)
    {
        if (currentState != null)
        {
            currentState.Update(delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (currentState != null)
        {
            currentState.PhysicsUpdate(delta);
        }
    }



    private void PrintStates()
        {
            foreach (var kvp in states)
            {
                GD.Print($"State Name: {kvp.Key}, State Object: {kvp.Value}");
            }
        }
}
