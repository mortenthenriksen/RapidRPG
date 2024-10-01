using Godot;
using Game.Characters;
using Game.Manager;

namespace Game.Projectiles;

public partial class DropletBullet : Area2D
{
    float travelledDistance = 0; 
    const float SPEED = 600;
    const float RANGE = 800;

    public override void _Ready()
    {
        
    }

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
        if (body is Orc enemy)
        {
            enemy.TakeDamage(GetTotalDamageAmountFromDamageManager());
        }
    }
    
    
    private float GetTotalDamageAmountFromDamageManager()
    {
        return DamageManager.Instance.TotalDamageAmount();
    }
}
