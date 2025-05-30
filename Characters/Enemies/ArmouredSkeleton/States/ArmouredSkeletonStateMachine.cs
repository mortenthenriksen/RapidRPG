namespace Game.State;

public partial class ArmouredSkeletonStateMachine : BaseStateMachine<State>
{
    public override void _Ready()
    {
        initialState = GetNode<State>("ArmouredSkeletonIdle");
        base._Ready();
    }
}
