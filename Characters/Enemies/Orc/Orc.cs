using Game.Autoload;
using Game.Manager;
using Godot; 

namespace Game.Characters;

public partial class Orc : CharacterBody2D
{ 

	[Signal]
	public delegate void UpdateOrcHealthEventHandler(float health);

	public float damageAmount = 3;

	private float speed = 150; 
	private float MAX_HEALTH = 150;
	private float health = 150; 
	private float threshold = 50;
	private float separationRadius = 200; // Radius for separation detection
	private float separationStrength = 500; // Strength of the separation force
	private float avoidanceRadius = 30; // Radius for obstacle avoidance detection
	private float avoidanceAngle = 90; // Angle to adjust direction when avoiding obstacles
	
	private bool isDead = false;
	private bool isTakingDamage = false;
	private bool isAttacking = false;
	private bool hasDealtDamage = false;
	private Vector2 direction;
	private float knockBackForce = 15;
	private float elapsedTime;

	private ProgressBar healthBarOrc; 
	private Label healthLabelOrc;
	private AnimatedSprite2D animatedSprite2D; 
	private Dave player; 
	private Area2D detectionArea;
	private Area2D obstacleDetectionArea;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		healthLabelOrc = GetNode<Label>("HealthBarOrc/HealthLabelOrc");
		healthBarOrc = GetNode<ProgressBar>("HealthBarOrc");
		player = GetNode<Dave>("/root/Main/Dave");
		detectionArea = GetNode<Area2D>("DetectionArea");
		obstacleDetectionArea = GetNode<Area2D>("ObstacleDetectionArea");

		health = MAX_HEALTH * MultiplierManager.Instance.GetMultiplier();

		UpdateHealthBar();

		// CustomSignals.Instance.EnemyDamageRecieved += OnEnemyDamageRecieved;
	}


	public override void _PhysicsProcess(double delta)
	{
		elapsedTime += (float) delta;
		direction = GlobalPosition.DirectionTo(player.GetCurrentPlayerPosition());
		var distance = GlobalPosition.DistanceTo(player.GetCurrentPlayerPosition());
		var velocity = direction * speed * (float)delta;

		Vector2 separationForce = CalculateSeparationForce();
		Vector2 avoidanceDirection = AdjustDirectionForObstacles(direction);

		velocity = (avoidanceDirection * speed + separationForce * separationStrength) * (float)delta;

		if (distance > threshold && health > 0)
		{
			// MoveAndCollide(velocity);
		}

		DealDamageToDave();

		if (health > 0)
		{
			if (!isTakingDamage)
			{
				if (distance < threshold)
				{
					if (!isAttacking)
					{
						isAttacking = true;
						animatedSprite2D.Play("attack01");
					}
				}
				else if (direction.X != 0) 
				{
					if (direction.X > 0)
					{	
						animatedSprite2D.FlipH = false;
					}
					else
					{
						animatedSprite2D.FlipH = true;
					}
					animatedSprite2D.Play("run");
				}
			}

			if (isTakingDamage)
			{
				animatedSprite2D.Play("hurt");
			}
		}
		else if (health <= 0 && !animatedSprite2D.IsPlaying())
		{
			QueueFree();
		}
	}

	
	private void DealDamageToDave()
	{
		// trying to make damage seem logical
		var bodies = detectionArea.GetOverlappingBodies();
		foreach (Node body in bodies)
		{
			if (body is Dave dave)
			{
				if (isAttacking && !hasDealtDamage)
				{
					CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageDealt, damageAmount);
					hasDealtDamage = true;
					break;
				}
			}
		}
	}

	private void OnAnimationFinished()
	{
		isTakingDamage = false;
		isAttacking = false;
		hasDealtDamage = false;
	}

	public void TakeDamage(float damageAmount) 
	{
		// health -= damageAmount;
		UpdateHealthBar();	
		if (!isDead)
		{
			isTakingDamage = true;
			Position -= direction.Normalized() * knockBackForce;
			CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageRecieved, Position);
		}
		if (health > 0) 
		{   
			elapsedTime = 0.0f;
		}
		else if (health <= 0 && !isDead) 
		{   
			isDead = true; 
			animatedSprite2D.Play("death");
			OnEnemyHealthDepleted(health);
		} 
	}

	private Vector2 CalculateSeparationForce()
	{
		Vector2 separationForce = Vector2.Zero;
		var bodies = detectionArea.GetOverlappingBodies();

		foreach (Node body in bodies)
		{
			if (body is Orc orc && orc != this)
			{
				Vector2 difference = GlobalPosition - orc.GlobalPosition;
				float distance = difference.Length();

				if (distance < separationRadius)
				{
					separationForce += difference.Normalized() / distance;
				}
			}
		}

		return separationForce;
	}

	private Vector2 AdjustDirectionForObstacles(Vector2 direction)
	{
		var bodies = obstacleDetectionArea.GetOverlappingBodies();

		foreach (Node body in bodies)
		{
			if (body is Orc orc && orc != this)
			{
				Vector2 difference = orc.GlobalPosition - GlobalPosition;
				float distance = difference.Length();

				if (distance < avoidanceRadius)
				{
					float angle = Mathf.DegToRad(avoidanceAngle);
					direction = direction.Rotated(angle);
					break;
				}
			}
		}

		return direction;
	}


	private void UpdateHealthBar()
	{
		EmitSignal(SignalName.UpdateOrcHealth, health);
		healthBarOrc.MaxValue = MAX_HEALTH * MultiplierManager.Instance.GetMultiplier();
		healthBarOrc.Value = health * MultiplierManager.Instance.GetMultiplier();
		healthLabelOrc.Text = $"Health: {healthBarOrc.Value.ToString("F0")}";
		if (health <= 0)
		{
			healthBarOrc.Visible = false;
			healthLabelOrc.Visible = false;
		}
	}

	private void OnEnemyHealthDepleted(float health)
	{	
		CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyHealthDepleted, health, GlobalPosition);
	}


	public float GetHealth()
	{
		return health;
	}
}
