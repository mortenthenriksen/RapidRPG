using Game.Characters;
using Game.State;
using Godot;
using System;

public partial class ArmouredSkeletonIdle : State
{
    [Export]
    private ArmouredSkeleton armouredSkeleton;

    [Export]
    private int moveSpeed = 0;

    private Dave player;

    private Vector2 moveDirection;
    private float wanderTime;
    private Random random = new Random();
    private AnimatedSprite2D animatedSprite2D;
    
    public override void _Ready()
    {
        player = GetNode<Dave>("/root/Main/Dave");
        animatedSprite2D = armouredSkeleton.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    private void RandomizeWander()
    {   
        moveDirection = new Vector2(random.Next(-1, 2), random.Next(-1, 2)).Normalized();
        wanderTime = random.Next(1,3);
    }

    public override void Enter()
    {
        RandomizeWander();
    }

    public override void Exit()
    {
        moveDirection = Vector2.Zero;
    }

    public override void Update(double delta)
    {
        if (wanderTime > 0)
        {
            wanderTime -= (float)delta;
        }
        else 
        {
            RandomizeWander();
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        var distance = armouredSkeleton.GlobalPosition.DistanceTo(player.GlobalPosition);

        if (armouredSkeleton != null)
        {
            armouredSkeleton.Velocity = moveDirection * moveSpeed;
        }

        if (distance < armouredSkeleton.GetRange()) 
        {
            EmitSignal(SignalName.Transitioned, this, "armouredskeletoncharge");
        }
    }
}
