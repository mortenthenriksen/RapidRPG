using Godot;
using Game.State;
using Godot.Collections;
using Game.Characters;

public partial class ArmouredSkeletonAttack : State
{
    [Export]
    private ArmouredSkeleton armouredSkeleton;

    [Export]
    private int moveSpeed = 0;

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
        detectionArea = armouredSkeleton.GetNode<Area2D>("DetectionArea");
        collisionShape2D = armouredSkeleton.GetNode<CollisionShape2D>("DetectionArea/DetectionCollision");
        animatedSprite2D = armouredSkeleton.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
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
        isAttacking = false;
    }

    public override void Update(double delta)
    {
        
    }

    public override void PhysicsUpdate(double delta)
    {
        var distance = armouredSkeleton.GlobalPosition.DistanceTo(player.GlobalPosition);

        if (armouredSkeleton != null)
        {
            armouredSkeleton.Velocity = moveDirection * moveSpeed;
        }

        if (bodies.Count == 0 || distance > ArmouredSkeleton.ATTACK_RANGE) 
        {
            isAttacking = false; 
            EmitSignal(SignalName.Transitioned, this, "armouredskeletonidle");
        }
    }

    private void OnFireRateTimerTimeout()
    {
        if (bodies.Count > 0)
        {
            isAttacking = true;
        }
    }

    private void OnAnimatedSprite2DAnimationFinishedState()
    {
        if (animatedSprite2D.Animation == "attack01")
        {
            isAttacking = false; 
            
            if (bodies.Count > 0)
            {
                fireRateTimer.Start();
            }
            else
            {
                EmitSignal(SignalName.Transitioned, this, "armouredskeletonidle");
            }
        }
        else if (animatedSprite2D.Animation == "hurt")
        {
            // Handle hurt animation completion
            isAttacking = false;
        }
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }
}
