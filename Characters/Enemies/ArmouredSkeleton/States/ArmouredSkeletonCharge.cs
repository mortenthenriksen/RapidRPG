using Game.Characters;
using Game.State;
using Godot;
using Godot.Collections;
using System;

public partial class ArmouredSkeletonCharge : State
{
    [Export]
    private ArmouredSkeleton armouredSkeleton;

    [Export]
    private int moveSpeed; 
    
    private Dave player;

    private const string RUN_ANIMATION = "run";

    private Vector2 moveDirection;
    private AnimatedSprite2D animatedSprite2D;

    public override void _Ready()
    {
        animatedSprite2D = armouredSkeleton.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        player = GetNode<Dave>("/root/Main/Dave");
    }

    public override void Enter()
    {
        animatedSprite2D.Play(RUN_ANIMATION);
    }

    public override void PhysicsUpdate(double delta)
    {
        if (player == null || armouredSkeleton == null) return;

        var distance = armouredSkeleton.GlobalPosition.DistanceTo(player.GlobalPosition);
        moveDirection = (player.GlobalPosition - armouredSkeleton.GlobalPosition).Normalized();

        // Update sprite direction based on movement
        if (moveDirection.X != 0)
        {
            animatedSprite2D.FlipH = moveDirection.X < 0;
        }

        // If within attack range, transition to attack state
        if (distance <= ArmouredSkeleton.ATTACK_RANGE)
        {
            EmitSignal(SignalName.Transitioned, this, "armouredskeletonattack");
            return;
        }
        if (distance >= armouredSkeleton.GetRange())
        {
            EmitSignal(SignalName.Transitioned, this, "armouredskeletonidle");
        }

        // Continue charging towards player
            armouredSkeleton.Velocity = moveDirection * moveSpeed;
        armouredSkeleton.MoveAndSlide();
    }

    public override void Exit()
    {
        
    }

    public override void Update(double delta)
    {
        
    }

}