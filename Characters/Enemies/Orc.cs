using Game.Autoload;
using Godot; 

namespace Game.Characters;

public partial class Orc : CharacterBody2D
{ 

	[Signal]
	public delegate void UpdateOrcHealthEventHandler(float health);

	public float damageAmount = 1;

	private float speed = 150; 
	private float MAX_HEALTH = 150;
	private float health = 150; 
	private float threshold = 50;
	private float damageTaken;
	
	private ProgressBar healthBarOrc; 
	private Label healthLabelOrc;
	
	private bool isDead = false;
	private float elapsedTime;

	private AnimatedSprite2D animatedSprite2D; 
	private Dave player; 

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		healthLabelOrc = GetNode<Label>("HealthBarOrc/HealthLabelOrc");
		healthBarOrc = GetNode<ProgressBar>("HealthBarOrc");
		player = GetNode<Dave>("/root/Main/Dave");

		UpdateHealthBar();

		// CustomSignals.Instance.EnemyDamageRecieved += OnEnemyDamageRecieved;
	}


    public override void _PhysicsProcess(double delta)
	{
		elapsedTime += (float) delta;
		var direction = GlobalPosition.DirectionTo(player.GetCurrentPlayerPosition());
		var distance = GlobalPosition.DistanceTo(player.GetCurrentPlayerPosition());
		var velocity = direction * speed * (float)delta;

		if (distance > threshold && health > 0)
		{
			// MoveAndCollide(velocity);
		}


		if (health > 0)
		{
			if (distance < threshold)
			{
				animatedSprite2D.Play("attack01");
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
		else if (health <= 0 && !animatedSprite2D.IsPlaying())
		{
			QueueFree();
		}
	}

	public void TakeDamage(float damageAmount) 
	{
		health -= damageAmount;
		UpdateHealthBar();	
		if (health > 0) 
		{   
			elapsedTime = 0.0f;
			CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyHitByBullet, Position);
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
		healthBarOrc.MaxValue = MAX_HEALTH;
		healthBarOrc.Value = health;
		healthLabelOrc.Text = $"Health: {health}";
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


	private void OnHurtBoxBodyEntered(Node2D body) 
	{
		if (body is Dave) 
		{
			if (body == null) return;
			CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageDealt, damageAmount);
		} else {
			return;
		}
		
	}

	public float GetHealth()
	{
		return health;
	}


    // very important to also remove the eventhandler from the dying orc and not just the dying orc
    // protected override void Dispose(bool disposing)
    // {
    // 	CustomSignals.Instance.EnemyDamageRecieved -= OnEnemyDamageRecieved;
    // 	base.Dispose(disposing);
    // }

}

