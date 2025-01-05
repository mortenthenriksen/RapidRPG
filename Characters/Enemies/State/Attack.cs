using Godot.Collections;
using Game.Characters;
using Game.State;
using Godot;

public partial class Attack : State
{
    [Export]
    private Archer archer;

    [Export]
    private int moveSpeed = 0;

    [Export]
	private PackedScene arrowScene;

    private Dave player;

    private Vector2 offset = new Vector2(0,-10);
    private Array<Node2D> bodies = new Array<Node2D>();
    private Timer fireRateTimer;
    private Vector2 moveDirection;
	private Area2D detectionArea;
	private CollisionShape2D collisionShape2D;
    private AnimatedSprite2D animatedSprite2D;
    private bool isAttacking = false;

    public override void _Ready()
    {
        detectionArea = archer.GetNode<Area2D>("DetectionArea");
        collisionShape2D = archer.GetNode<CollisionShape2D>("DetectionArea/DetectionCollision");
        animatedSprite2D = archer.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        fireRateTimer = GetNode<Timer>("FireRateTimer");
        player = GetNode<Dave>("/root/Main/Dave");
    }

    public override void Enter()
    {
        bodies = detectionArea.GetOverlappingBodies();
        fireRateTimer.Start();
    }

    public override void Exit()
    {
        bodies.Clear();
        fireRateTimer.Stop();
        
    }

    public override void Update(double delta)
    {
        
    }

    public override void PhysicsUpdate(double delta)
    {
        var distance = archer.GlobalPosition.DistanceTo(player.GlobalPosition);

        if (archer != null)
        {
            archer.Velocity = moveDirection * moveSpeed;
        }

        if (distance > archer.GetRange()) 
        {
            EmitSignal(SignalName.Transitioned, this, "idle");
        }
    }

    private void FireArrowAtDave(Vector2 positionOfDave)
    {
        var arrow = arrowScene.Instantiate() as Area2D;
		arrow.GlobalPosition = archer.Position + offset;
		arrow.LookAt(positionOfDave + offset);
		GetTree().Root.CallDeferred("add_child", arrow);
    }

    private void OnFireRateTimerTimeout()
    {
        isAttacking = true;
    }

    private void OnAnimatedSprite2DAnimationFinishedState()
    {
        if (animatedSprite2D.Animation == "attack01")
        {
            var positionOfDave = player.Position;
            FireArrowAtDave(positionOfDave);

            if (bodies.Count > 0)
            {
                fireRateTimer.Start();
            }
            isAttacking = false;
        } 
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }
}
