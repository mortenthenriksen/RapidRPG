using Game.Characters;
using Godot;

namespace Game.Sword;

public partial class Sword : Area2D
{
    [Export]
    private Dave player;

    private AnimatedSprite2D animatedSprite2D;
    private Vector2 direction;
    private bool isSwinging = false;
    private float swingCooldown = 0.3f;
    private float swingCooldownTimer = 0.0f; 
    private bool upRight;
    private bool downRight;
    private bool upLeft;
    private bool downLeft;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _Process(double delta)
    {
        direction = player.GetCurrentDirection();
        upRight = direction == Vector2.Up || direction == new Vector2((float)0.70710677, -(float)0.70710677);
        downRight = direction == Vector2.Right || direction == new Vector2((float)0.70710677, (float)0.70710677);
        upLeft = direction == Vector2.Left || direction == new Vector2(-(float)0.70710677, -(float)0.70710677);
        downLeft = direction == Vector2.Down || direction == new Vector2(-(float)0.70710677, (float)0.70710677);

        if (swingCooldownTimer > 0.0f)
        {
            swingCooldownTimer -= (float) delta;
        }

        if (Input.IsActionPressed("attack")) {
            Swing();
        } 

        else 
        {
            StopSwing();
        }
    }

    public void Swing()
    {
        if (swingCooldownTimer <= 0.0f)
        {
            isSwinging = true;
            PlayAnimation();
            swingCooldownTimer = swingCooldown;
        }
    }

    public void StopSwing()
    {
        isSwinging = false;
        animatedSprite2D.Play("idle");
    }



    private void PlayAnimation()
    {   
        if (upRight)
        {
            animatedSprite2D.Play("upRight");
        }
        else if (downRight)
        {
            animatedSprite2D.Play("downRight");
        }
        else if (upLeft)
        {
            animatedSprite2D.Play("upLeft");
        }
        else if (downLeft)
        {
            animatedSprite2D.Play("downLeft");
        }
    }
}


