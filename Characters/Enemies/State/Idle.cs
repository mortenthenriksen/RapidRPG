using System;
using Game.State;
using Godot;

public partial class Idle : State
{
    [Export]
    private CharacterBody2D enemy;

    [Export]
    private int moveSpeed = 30;

    [Export]
    private CharacterBody2D player;

    private Vector2 moveDirection;
    private float wanderTime;
    private Random random = new Random();

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
        var direction = player.GlobalPosition - enemy.GlobalPosition;
        if (enemy != null)
        {
            enemy.Velocity = moveDirection * moveSpeed;
        }

        if (direction.Length() < 250) 
        {
            GD.Print("should attack");
            EmitSignal(SignalName.Transitioned, this, "attack");
        }
    }
}
