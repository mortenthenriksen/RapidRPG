using Game.Autoload;
using Game.Manager;
using Godot; 

namespace Game.Characters;

public partial class Orc : IEnemies
{ 

	[Signal]
	public delegate void UpdateOrcHealthEventHandler(float health);

	public float damageAmount = 3;

	private float speed = 150; 
	private float MAX_HEALTH = 150;
	private float health = 150; 
	private float threshold = 50;
	
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

		health = MAX_HEALTH * MultiplierManager.Instance.GetMultiplier();

		UpdateHealthBar();

		// CustomSignals.Instance.EnemyDamageRecieved += OnEnemyDamageRecieved;
	}


	public override void _PhysicsProcess(double delta)
	{
		direction = GlobalPosition.DirectionTo(player.Position);
		var distance = GlobalPosition.DistanceTo(player.Position);
		var velocity = direction * speed * (float)delta;

		if (distance > threshold && health > 0)
		{
			MoveAndCollide(velocity);
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

	public override void TakeDamage(float damageAmount) 
	{
		health -= damageAmount;
		UpdateHealthBar();	
		if (!isDead)
		{
			isTakingDamage = true;
			CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageRecieved, Position);
		}
		else if (health <= 0 && !isDead) 
		{   
			isDead = true; 
			animatedSprite2D.Play("death");
			OnEnemyHealthDepleted(health);
		} 
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
