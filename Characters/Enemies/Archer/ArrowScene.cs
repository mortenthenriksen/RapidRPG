using Game.Autoload;
using Game.Characters;
using Godot;
using System.Collections.Generic;

namespace Game.Projectiles; 

public partial class ArrowScene : Area2D
{
    private static Queue<ArrowScene> pool = new Queue<ArrowScene>();

    [Export]
    private Archer archer;

    float travelledDistance = 0; 
    const float SPEED = 500;
    const float RANGE = 600;
    private Vector2 direction;

    public static ArrowScene GetArrow()
    {
        if (pool.Count > 0)
        {
            var arrow = pool.Dequeue();
            arrow.Visible = false;
            arrow.travelledDistance = 0; // Reset travelled distance
            return arrow;
        }
        return new ArrowScene();
    }

    public override void _Ready()
    {
        direction = Vector2.Right.Rotated(Rotation);
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += direction * SPEED * (float)delta;
        travelledDistance += SPEED * (float)delta;
        if (travelledDistance > RANGE)
        {
            RemoveArrow();
        }
    }
    private void OnBodyEntered(Node2D body)
    {
        if (body is Dave player && !player.getIsDashing())
        {
            player.PlayerDamageReceived(Archer.GetDamageAmount());
            CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageDealt, Archer.GetDamageAmount());
            RemoveArrow();
        }
    }


    private void RemoveArrow()
    {
        if (GetParent() != null)
        {
            CallDeferred("queue_free");
        } 
        else
        {
            pool.Enqueue(this);
        }
    }   

}
