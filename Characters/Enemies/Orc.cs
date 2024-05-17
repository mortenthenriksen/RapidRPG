using Godot; 
using System;

public partial class Orc : Enemies, IEnemies
{ 
	[Signal]
	public delegate void HealthDepletedOrcEventHandler(float health);   

	[Signal]
	public delegate void UpdateHealthEventHandler(float health);

	public float damageAmount = 1;

	private float speed = 200; 
	private float health = 5; 
	private float threshold = 35;
	
	private bool isDead = false;
	private bool isHurt = false;
	private float elapsedTime;

	private CustomSignals customSignals;
	private AnimatedSprite2D animatedSprite2D; 
	private CharacterBody2D player; 
	private DamageNumbers damageNumbersOrigin;


	public override void _Ready()
	{
		customSignals = GetNode<CustomSignals>("/root/CustomSignals");
		damageNumbersOrigin = GetNode<DamageNumbers>("/root/DamageNumbers");

		customSignals.HealthDepletedEnemy += HandleHealthOrcDepleted;

		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		player = GetNode<CharacterBody2D>("/root/Game/Dave");

	}

    public void HandleHealthOrcDepleted(float health)
    {	
        EmitSignal(SignalName.HealthDepletedOrc, health);
	}


    private void OnHurtBoxBodyEntered(Node2D body) 
	{
		if (body is Dave) 
		{
			if (body == null) { return; }
			customSignals.EmitSignal(nameof(CustomSignals.DamagePlayer), damageAmount);
		} else {
			return;
		}
		
	}

	private void OnHurtBoxBodyExited(Node2D body) {
		if (body == null) { return; }
	}


	public override void _PhysicsProcess(double delta)
	{
		elapsedTime += (float) delta;
		var direction = GlobalPosition.DirectionTo(player.GlobalPosition);
		var velocity = direction * speed * (float)delta;
		var distance = GlobalPosition.DistanceTo(player.GlobalPosition);

		if (distance > threshold && health > 0)
		{
			MoveAndCollide(velocity);
		}


		if (health > 0)
		{
			if (elapsedTime > 0.1f) {
				isHurt = false;
				if (distance < threshold)
				{
					animatedSprite2D.Play("attack");
				}
				else
				{
					animatedSprite2D.Play("run");
				}
			}	
		}
		else if (health <= 0 && !animatedSprite2D.IsPlaying())
		{
			QueueFree();
		}
	}

	public void TakeDamage(float damageAmount) {
		health -= damageAmount;
		if (health > 0) 
		{   
			isHurt = true;
			elapsedTime = 0.0f;
			damageNumbersOrigin.DisplayNumber(damageAmount, GlobalPosition, false);
		}
		else if (health <= 0 && !isDead) 
		{   
			isDead = true; 
			animatedSprite2D.Play("death");
			HandleHealthOrcDepleted(health);
		} 
	}


	// very important to also remove the eventhandler from the dying orc and not just the dying orc
    protected override void Dispose(bool disposing)
    {
		customSignals.HealthDepletedEnemy -= HandleHealthOrcDepleted;
        base.Dispose(disposing);
    }
}

