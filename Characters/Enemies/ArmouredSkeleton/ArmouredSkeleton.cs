using System;
using Game.Autoload;
using Game.Manager;
using Godot;

namespace Game.Characters;

public partial class ArmouredSkeleton : IEnemies
{
    [Export]
    private int RANGE;

	public const float ATTACK_RANGE = 50f;

    private static float damageAmount = 2f;

	private float speed = 110; 
	private float MAX_HEALTH = 1000;
	private float health = 75;

	private struct AttackData
	{
		public string AnimationName;
		public int Weight;

		public AttackData(string animationName, int weight)
		{
			AnimationName = animationName;
			Weight = weight;
		}
	}

	private readonly AttackData[] attacks = new AttackData[]
	{
		new("attack01", 70),  // 70% chance
		new("attack02", 30)   // 30% chance
	};

	private Random random = new Random();

	private bool isTakingDamage = false;
	private bool hasDealtDamage = false;
	private bool hasDealtFirstHit = false;
	private bool hasDealtSecondHit = false;

	private ArmouredSkeletonAttack attack;
	private Dave player; 

	private ProgressBar healthBar; 
	private Label healthLabel;
	private Area2D detectionArea;
	private CollisionShape2D collisionShape2D;

	private AnimatedSprite2D animatedSprite2D;
	private AudioStreamPlayer2D deathSound; 
	private AudioStreamPlayer2D fireArrowSound; 

	public override void _Ready()
	{
		player = GetNode<Dave>("/root/Main/Dave");
		attack = GetNode<ArmouredSkeletonAttack>("ArmouredSkeletonStateMachine/ArmouredSkeletonAttack");
		
		healthBar = GetNode<ProgressBar>("HealthBar");
		healthLabel = GetNode<Label>("HealthBar/HealthLabel");
		collisionShape2D = GetNode<CollisionShape2D>("DetectionArea/DetectionCollision");
		detectionArea = GetNode<Area2D>("DetectionArea");
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		deathSound = GetNode<AudioStreamPlayer2D>("DeathSound");

		if (collisionShape2D.Shape is CircleShape2D circleShape)
		{
			circleShape.Radius = RANGE;
        }

		health = MAX_HEALTH * MultiplierManager.Instance.GetMultiplier();

		UpdateHealthBar();
	}

	public override void _Process(double delta)
	{
		if (health > 0)
		{
			if (attack.GetIsAttacking())
			{
				// When attacking, update facing direction based on player position
				animatedSprite2D.FlipH = player.GlobalPosition.X < GlobalPosition.X;
				// Random random = new Random();
				// var attack_chance = random.NextInt64(0, 10);
				// GD.Print(attack_chance);
				// if (attack_chance < 7)
				// {
				// }
				// else
				// {
				// 	animatedSprite2D.Play("attack02");
				// }
				animatedSprite2D.Play("attack01");
			}
			else if (isTakingDamage)
			{
				// animatedSprite2D.Play("hurt");
			}
			else if (Velocity.Length() > 0)
			{
				// When moving, update facing direction based on movement
				animatedSprite2D.FlipH = Velocity.X < 0;
				animatedSprite2D.Play("run");
			}
			else
			{
				animatedSprite2D.Play("idle");
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
			// Position += playerDirection * 2;-
			CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageRecieved, damageAmount, Position);
		}
		if (health <= 0)
		{
			deathSound.Play();
		}
	}


	private void OnAnimatedSprite2DAnimationFinished()
	{
		if (animatedSprite2D.Animation == "death")
		{
			QueueFree();
			OnEnemyHealthDepleted(health);
		}

		else if (animatedSprite2D.Animation == "hurt")
		{
			isTakingDamage = false;
		}
		else if (animatedSprite2D.Animation == "attack01")
		{
			hasDealtDamage = false;
		}
		else if (animatedSprite2D.Animation == "attack02")
		{
			hasDealtFirstHit = false;
			hasDealtSecondHit = false;
		}
	}
	
	private void OnAnimatedSprite2DAnimationChanged()
	{
		if (animatedSprite2D.Animation == "attack01")
		{
			hasDealtDamage = false;  
		}
	}

	private void OnAnimationFrameChanged()
	{
		if (animatedSprite2D.Animation == "attack01")
		{
			if (!hasDealtDamage)
			{
				if (animatedSprite2D.Frame >= 4 && animatedSprite2D.Frame <= 6)
				{
					CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageDealt, GetDamageAmount());
					hasDealtDamage = true;
				}
			}
		}
		else if (animatedSprite2D.Animation == "attack02")
		{
			// First hit early in animation
			if (!hasDealtFirstHit && animatedSprite2D.Frame >= 2 && animatedSprite2D.Frame <= 4)
			{
				CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageDealt, GetDamageAmount());
				hasDealtFirstHit = true;
			}
			// Second hit later in animation
			else if (!hasDealtSecondHit && animatedSprite2D.Frame >= 6 && animatedSprite2D.Frame <= 8)
			{
				CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageDealt, GetDamageAmount());
				hasDealtSecondHit = true;
			}
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

	public int GetRange()
	{
		return RANGE;
	}
}
