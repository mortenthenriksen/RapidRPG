using Godot.Collections;
using Game.Characters;
using Game.State;
using Godot;

public partial class Attack : State
{
    [Export]
    private Archer archer;
    
    [Export]
    private Dave player;

    [Export]
    private int moveSpeed = 0;

    [Export]
	private PackedScene arrowScene;

    private Array<Node2D> bodies = new Array<Node2D>();
    private Timer fireRateTimer;
    private Vector2 moveDirection;
	private Area2D detectionArea;

    public override void _Ready()
    {
        detectionArea = archer.GetNode<Area2D>("DetectionArea");
        fireRateTimer = GetNode<Timer>("FireRateTimer");
    }

    public override void Enter()
    {
        bodies = detectionArea.GetOverlappingBodies();
        fireRateTimer.Start();
    }

    public override void Exit()
    {
        bodies.Clear();
    }

    public override void Update(double delta)
    {
        
    }

    public override void PhysicsUpdate(double delta)
    {
        var direction = player.GlobalPosition - archer.GlobalPosition;

        if (archer != null)
        {
            archer.Velocity = moveDirection * moveSpeed;
        }

        if (direction.Length() > 250) 
        {
            GD.Print("should idle");
            EmitSignal(SignalName.Transitioned, this, "idle");
        }
    }

    private void DealDamageToDave()
    {
        var positionOfDave = player.Position;
        FireArrowAtDave(positionOfDave);
    }

    private void FireArrowAtDave(Vector2 positionOfDave)
    {
        var arrow = arrowScene.Instantiate() as Area2D;
		arrow.GlobalPosition = archer.Position;
		arrow.LookAt(positionOfDave);
		GetTree().Root.CallDeferred("add_child", arrow);
    }

    private void OnFireRateTimerTimeout()
    {
        if (bodies.Count > 0)
        {
            var positionOfDave = player.Position;
            FireArrowAtDave(positionOfDave);
            fireRateTimer.Start();
        }
    }
}
