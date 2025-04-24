using Game.Characters;
using Game.Manager;
using Godot;

namespace Game.UniqueEffects;

public partial class LargeExplosionScene : Area2D
{   
    private Node2D audioParent;
    private bool hasDealtDamage;

    public override void _Ready()
    {   
        audioParent = GetNode<Node2D>("AudioParent");
        var largeExplosionSound = (AudioStreamPlayer2D)audioParent.GetChild(0);
        largeExplosionSound.Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!hasDealtDamage) 
        {
            foreach (var body in this.GetOverlappingBodies())
            {
                if (body is IEnemies enemies)
                {
                    enemies.TakeDamage(75);
                }
            }
        }
    }
    
    private void OnAnimatedSprite2DAnimationFinished()
    {
        QueueFree();
    }
}
