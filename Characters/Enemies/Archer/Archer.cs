using Game.Autoload;
using Game.Manager;
using Godot;

namespace Game.Characters;

public partial class Archer : IEnemies
{
    [Signal]
    public delegate void UpdateArcherHealthEventHandler(float health);

    [Export]
	private PackedScene arrowScene;

	private static float damageAmount = 2f;

	private float speed = 110; 
	private float MAX_HEALTH = 75;
	private float health = 75; 
	private bool isTakingDamage = false;

	private ProgressBar healthBar; 
	private Label healthLabel;

	private AnimatedSprite2D animatedSprite2D;
	private AudioStreamPlayer2D audioStreamPlayer2D; 
	private Attack attack;
	private Dave player; 

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		healthBar = GetNode<ProgressBar>("HealthBar");
		healthLabel = GetNode<Label>("HealthBar/HealthLabel");
		attack = GetNode<Attack>("StateMachine/Attack");
		player = GetNode<Dave>("/root/Main/Dave");

		health = MAX_HEALTH * MultiplierManager.Instance.GetMultiplier();

		UpdateHealthBar();
	}

    public override void _Process(double delta)
    {
		if (health > 0)
		{
			if (!isTakingDamage)
			{
				animatedSprite2D.FlipH = true;
				if (Velocity.X >= 0 && player.GlobalPosition.X > GlobalPosition.X )
				{
					animatedSprite2D.FlipH = false;
				}

				if (Velocity.Length() > 0)
				{
					animatedSprite2D.Play("run");
				}
				else if (attack.GetIsAttacking())
				{
					animatedSprite2D.Play("attack01");
				}
				else 
				{
					animatedSprite2D.Play("idle");
				}
			}

			if (isTakingDamage && !attack.GetIsAttacking())
			{
				animatedSprite2D.Play("hurt");
			}
		}

		else if (health <= 0)
		{
			animatedSprite2D.Play("death");
		} 
    }

    public override void _PhysicsProcess(double delta)
	{
		MoveAndCollide(Velocity * (float)delta);
	}

	private void UpdateHealthBar()
	{
		EmitSignal(SignalName.UpdateArcherHealth, health);
		healthBar.MaxValue = MAX_HEALTH * MultiplierManager.Instance.GetMultiplier();
		healthBar.Value = health * MultiplierManager.Instance.GetMultiplier();
		healthLabel.Text = $"Health: {healthBar.Value.ToString("F0")}";
		if (health <= 0)
		{
			healthBar.Visible = false;
			healthLabel.Visible = false;
		}
	}

	public override void TakeDamage(float damageAmount) 
	{
		var playerDirection = player.GetCurrentDirection();
		health -= damageAmount;
		UpdateHealthBar();	
		if (health > 0)
		{
			isTakingDamage = true;
			Position += playerDirection * 2;
			CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageRecieved, Position);
		}
		if (health <= 0)
		{
			audioStreamPlayer2D.Play();
		}
	}

	private void OnAnimatedSprite2DAnimationFinished()
	{
		if (animatedSprite2D.Animation == "death")
		{
			QueueFree();
			OnEnemyHealthDepleted(health);
		}

		if (animatedSprite2D.Animation == "hurt")
		{
			isTakingDamage = false;
		}
	}

	private void OnEnemyHealthDepleted(float health)
	{	
		CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyHealthDepleted, health, GlobalPosition);
	}

	public static float GetDamageAmount()
	{
		return damageAmount;
	}
}
