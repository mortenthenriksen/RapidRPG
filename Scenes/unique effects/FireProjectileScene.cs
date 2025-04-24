using Game.Characters;
using Game.Manager;
using Godot;

public partial class FireProjectileScene : Area2D
{
    private const float SPEED = 500;
    private const float RANGE = 300;
    private float travelledDistance = 0; 
    private AnimatedSprite2D animatedSprite2D;
    private Node2D audioParent;
    private Vector2 direction;

    public override void _Ready()
    {
        direction = Vector2.Right.Rotated(Rotation);
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        audioParent = GetNode<Node2D>("AudioParent");
        var fireProjectileSound = (AudioStreamPlayer2D)audioParent.GetChild(0);
        fireProjectileSound.Play();
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
            animatedSprite2D.Play("impact");
            var fireProjectileSound = (AudioStreamPlayer2D)audioParent.GetChild(1);
            fireProjectileSound.Play();
        }
    }
}
