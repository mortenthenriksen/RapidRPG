
namespace Game.State;

public partial class ArcherStateMachine : BaseStateMachine<State>
{
    public override void _Ready()
    {
        initialState = GetNode<State>("ArcherIdle");
        base._Ready();
    }
}
