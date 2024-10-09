using Game.Autoload;
using Game.Inventory;
using Game.Manager;
using Game.Weapons;
using Godot;
using System;
using System.Linq;

namespace Game.Characters;

public partial class Dave : CharacterBody2D
{

	[Signal]
	public delegate void UpdatePlayerHealthEventHandler(float health);

	[Signal]
	public delegate void PlayerHealthDepletedEventHandler(float health);


	private float speed = 300;
	private float MAX_HEALTH = 75.0f;
	private float health = 75.0f;
	private float healthRegenAmount = 0.25f;
	private readonly StringName MOVE_LEFT = "move_left";
	private readonly StringName MOVE_RIGHT = "move_right";
	private readonly StringName MOVE_UP = "move_up";
	private readonly StringName MOVE_DOWN = "move_down";

	private Vector2 moveDirection = Vector2.Zero;
    private Vector2 lastDirection = Vector2.Down;


	private AnimatedSprite2D animatedSprite2D;
	private ProgressBar healthBar;
	private Label healthLabel;
	private PickupBox pickupBox;
	private Area2D attackBox;
	private AttackBoxCollisionShape attackBoxCollisionShape;
	private ItemDrop itemDrop;
	private Timer healthRegenTimer;
	private bool isAttacking = false;
	private bool hasDealtDamage = false;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		healthBar = GetNode<ProgressBar>("HealthBar");
		healthLabel = GetNode<Label>("HealthBar/HealthLabel");
		pickupBox = GetNode<PickupBox>("PickupBox");
		attackBox = GetNode<Area2D>("AttackBox");
		attackBoxCollisionShape = GetNode<AttackBoxCollisionShape>("%AttackBoxCollisionShape");

		UpdateHealthBar();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isAttacking)
		{
			moveDirection = HandleInput();
			if (moveDirection != Vector2.Zero)
            {
                lastDirection = moveDirection;
				attackBoxCollisionShape.RotateAttackBox(moveDirection);
            }
			PlayAnimation(moveDirection);
			MoveAndCollide(moveDirection * speed * (float) delta);
		}

		if (pickupBox.GetOverlappingBodies().Count > 0) 
		{
			var itemDrop = (ItemDrop)pickupBox.GetOverlappingBodies().First();
			itemDrop.PickupItem(this);
		}

		if (isAttacking && !hasDealtDamage)
		{
			foreach (var body in attackBox.GetOverlappingBodies())
			{
				AttackEnemy(body);
				hasDealtDamage = true;
			}
		}
	}

    private void AttackEnemy(Node2D body)
    {
		if (body is Orc enemy)
        {
            enemy.TakeDamage(DamageManager.Instance.TotalDamageAmount());
        }
    }

    public void PlayerDamageReceived(float damageAmount, Vector2 position) 
	{
		health -= damageAmount;
		UpdateHealthBar();
		if (health <= 0) 
		{
			EmitSignal(SignalName.PlayerHealthDepleted, health);
		}
	}



	private void OnTimerTimeout()
	{
		RegenerateHealth();
	}

	private void RegenerateHealth()
	{
		health = Mathf.Min(health + healthRegenAmount, MAX_HEALTH);
		UpdateHealthBar();
	}
	
	private void UpdateHealthBar()
	{
		EmitSignal(SignalName.UpdatePlayerHealth, health);
		healthBar.MaxValue = MAX_HEALTH;
		healthBar.Value = health;
		healthLabel.Text = $"Health: {health}";
	}

	private Vector2 HandleInput()
	{
		Vector2 moveDirection = Input.GetVector(MOVE_LEFT, MOVE_RIGHT, MOVE_UP, MOVE_DOWN);
		return moveDirection;
	}

	private void PlayAnimation(Vector2 direction) 
	{
		string action;

		if (Input.IsActionPressed("attack") && !isAttacking)
		{
			animatedSprite2D.SpeedScale = EquippedItemsManager.Instance.GetAttackSpeedFromEquippedItems();
			action = "sword";
			isAttacking = true;
		}

		else if (Input.IsActionPressed("special_attack") && !isAttacking)
		{
			action = "special";
			isAttacking = true;
		}

		else if (direction != Vector2.Zero)
		{
			animatedSprite2D.SpeedScale = 1;
			action = "run";
		}

		else
		{
			animatedSprite2D.SpeedScale = 1;
			action = "idle";
		}

		// string directionSuffix = direction switch
		// {
		// 	Vector2 d when d == Vector2.Up => "Up",
		// 	Vector2 d when d == Vector2.Down => "Down",
		// 	Vector2 d when d == Vector2.Left => "Left",
		// 	Vector2 d when d == Vector2.Right => "Right",
		// 	// slightly cursed, but i dont care :)
		// 	Vector2 d when d == new Vector2(-(float)0.70710677, -(float)0.70710677) => "Left", 
		// 	Vector2 d when d == new Vector2((float)0.70710677, -(float)0.70710677) => "Right", 
		// 	Vector2 d when d == new Vector2(-(float)0.70710677, (float)0.70710677) => "Left", 
		// 	Vector2 d when d == new Vector2((float)0.70710677, (float)0.70710677) => "Right", 
		// 	_ => ""
		// };

		string directionSuffix = GetDirectionSuffix(direction != Vector2.Zero ? direction : lastDirection);
        animatedSprite2D.Play(action + directionSuffix);
	}

	private string GetDirectionSuffix(Vector2 direction)
	{
		return direction switch
		{
			Vector2 d when d == Vector2.Up => "Up",
			Vector2 d when d == Vector2.Down => "Down",
			Vector2 d when d == Vector2.Left => "Left",
			Vector2 d when d == Vector2.Right => "Right",
			// slightly cursed, but i dont care :)
			Vector2 d when d == new Vector2(-(float)0.70710677, -(float)0.70710677) => "Left", 
			Vector2 d when d == new Vector2((float)0.70710677, -(float)0.70710677) => "Right", 
			Vector2 d when d == new Vector2(-(float)0.70710677, (float)0.70710677) => "Left", 
			Vector2 d when d == new Vector2((float)0.70710677, (float)0.70710677) => "Right", 
			_ => ""
		};
	}

	private void OnAnimationFinished()
	{
		isAttacking = false;
		hasDealtDamage = false;
	}


	public Vector2 GetCurrentDirection()
	{
		return moveDirection;
	}

	public Vector2 GetCurrentPlayerPosition()
	{
		return GlobalPosition;
	}
	
	public float GetHealth() {
		return health;
	}

}
