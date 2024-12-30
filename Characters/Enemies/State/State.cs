using Godot;

namespace Game.State;

public abstract partial class State : Node
{
    [Signal]
    public delegate void TransitionedEventHandler(State EnemyState, string newStateName);

    public abstract void Enter();
    
    public abstract void Exit();

    public abstract void Update(double delta);

    public abstract void PhysicsUpdate(double delta);
    
}
