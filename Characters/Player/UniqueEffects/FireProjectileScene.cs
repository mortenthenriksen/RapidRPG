using Game.Characters;
using Game.Manager;
using Godot;

public partial class FireProjectileScene : Area2D
{

    private float travelledDistance = 0; 
    private const float SPEED = 500;
    private const float RANGE = 600;
    private Vector2 direction;

    public override void _Ready()
    {
        direction = Vector2.Right.Rotated(Rotation);
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += direction * SPEED * (float) delta;
        travelledDistance += SPEED * (float) delta;
        if (travelledDistance > RANGE) {
            QueueFree();
        }
    }
    
    private void OnBodyEntered(Node2D body)
    {
        if (body is IEnemies enemies)
        {
            enemies.TakeDamage(DamageManager.Instance.GetTotalDamageAmount());
        }

    }
}
