using Godot;
using System;

public partial class dropletBullet : Area2D
{

    float travelledDistance = 0; 
    const float SPEED = 600;
    const float RANGE = 800;
    private float damageAmount = 2.9f;

    public override void _PhysicsProcess(double delta) 
    {
        var direction = Vector2.Right.Rotated(Rotation);
        Position += direction * SPEED * (float) delta;
        travelledDistance += SPEED * (float) delta;
        if (travelledDistance > RANGE) {
            QueueFree();
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is IEnemies enemy)
        {
            //QueueFree();
            enemy.TakeDamage(damageAmount);
        }
    }
}
