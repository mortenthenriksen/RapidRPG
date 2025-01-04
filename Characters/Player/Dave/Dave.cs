using Game.Autoload;
using Game.Inventory;
using Game.Manager;
using Game.UI;
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
	private ProgressBar rageBar;
	private Label rageLabel;
	private PickupBox pickupBox;
	private Area2D attackBox;
	private AttackBoxCollisionShape attackBoxCollisionShape;
	private ItemDrop itemDrop;
	private InventoryPanel inventoryPanel;
	private SkillBar skillBar; 
	private AudioStreamPlayer2D audioStreamPlayer2D;
	private Timer specialCooldownTimer;
	private Timer dashCooldownTimer;
	private Tween dashTween;

	private bool isDashOnCooldown = false;
	private bool isSpecialOnCooldown = false;
	private bool isAttacking = false;
	private bool hasDealtDamage = false;
	private bool isDashing = false;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		healthBar = GetNode<ProgressBar>("UserInterface/SkillBar/MarginContainer/HBoxContainer/VBoxHealth/HealthBar");
		healthLabel = GetNode<Label>("UserInterface/SkillBar/MarginContainer/HBoxContainer/VBoxHealth/HealthLabel");

		rageBar = GetNode<ProgressBar>("UserInterface/SkillBar/MarginContainer/HBoxContainer/VBoxRage/RageBar");
		rageLabel = GetNode<Label>("UserInterface/SkillBar/MarginContainer/HBoxContainer/VBoxRage/RageLabel");

		pickupBox = GetNode<PickupBox>("PickupBox");
		attackBox = GetNode<Area2D>("AttackBox");
		attackBoxCollisionShape = GetNode<AttackBoxCollisionShape>("%AttackBoxCollisionShape");
		inventoryPanel = GetNode<InventoryPanel>("UserInterface/Inventory/InventoryPanel");
		skillBar = GetNode<SkillBar>("UserInterface/SkillBar");
		dashCooldownTimer = GetNode<Timer>("DashCooldownTimer");
		specialCooldownTimer = GetNode<Timer>("SpecialCooldownTimer");
		audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");

		UpdateHealthBar();
		UpdateRageBar();
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
				if (body is IEnemies enemies)
				{
					AttackEnemy(enemies);
					hasDealtDamage = true;
				}
			}
		}
	}

    private void AttackEnemy(CharacterBody2D body)
    {
		if (body is IEnemies enemies)
        {
            enemies.TakeDamage(DamageManager.Instance.GetTotalDamageAmount());
        }
    }

    public void PlayerDamageReceived(float damageAmount) 
	{
		if (!isDashing)
		{
			health -= damageAmount;
			UpdateHealthBar();
			if (health <= 0) 
			{
				EmitSignal(SignalName.PlayerHealthDepleted, health);
			}
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
		healthLabel.Text = $"{Math.Round(health)}" +"/" + healthBar.MaxValue.ToString();
	}

	private void UpdateRageBar()
    {
        
    }

	private Vector2 HandleInput()
	{
		Vector2 moveDirection = Input.GetVector(MOVE_LEFT, MOVE_RIGHT, MOVE_UP, MOVE_DOWN);
		return moveDirection;
	}

	private void PlayAnimation(Vector2 direction) 
	{
		string action;

		animatedSprite2D.SpeedScale = 1;
		action = "idle";

		if (!inventoryPanel.GetIsMouseHoveringInventory() && !isAttacking)
		{
			if (Input.IsActionPressed("attack"))
			{
				audioStreamPlayer2D.Play();
				animatedSprite2D.SpeedScale = EquippedItemsManager.Instance.GetTotalAttackSpeed();
				EquippedItemsManager.Instance.FindUniqueEffectForWeapon();
				action = "sword";
				isAttacking = true;
			}

			else if (Input.IsActionPressed("special_attack") && !isSpecialOnCooldown)
			{
				action = "special";
				isAttacking = true;
				specialCooldownTimer.Start();
				isSpecialOnCooldown = true;
			}

			else if (Input.IsActionPressed("spinAttack"))
			{
				audioStreamPlayer2D.Play();
				action = "spinAttack";
				isAttacking = true;
			}

			else if (Input.IsActionPressed("dash") && !isDashOnCooldown)
			{
				if (dashTween != null)
				{
					dashTween.Kill(); 
				}

				dashTween = GetTree().CreateTween();
				Vector2 targetPosition = Position + lastDirection * 150;

				dashTween
					.TweenProperty(this, "position", targetPosition, 0.2f)
					// .SetEase(Tween.EaseType.In)
					.SetTrans(Tween.TransitionType.Sine);

				dashCooldownTimer.Start();
				isDashOnCooldown = true;
			}

			else if (Input.IsActionJustPressed("teleport"))
			{
				// GD.Print("port back to town shorty");
				CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.TeleportBackToTown);
			}

			else if (direction != Vector2.Zero)
			{
				animatedSprite2D.SpeedScale = 1;
				action = "run";
			}
		}

		else if (direction != Vector2.Zero)
		{
			animatedSprite2D.SpeedScale = 1;
			action = "run";
		}

		string directionSuffix = GetDirectionSuffix(direction != Vector2.Zero ? direction : lastDirection);
        animatedSprite2D.Play(action + directionSuffix);
	}

	private void OnDashCooldownTimerTimeout()
	{
		isDashOnCooldown = false;
	}

	private void OnSpecialCooldownTimerTimeout()
	{
		isSpecialOnCooldown = false;
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

	public bool GetIsDashOnCooldown()
	{
		return isDashOnCooldown;
	}

	public Timer GetDashTimer()
	{
		return dashCooldownTimer;
	}

	public bool GetIsSpecialOnCooldown()
	{
		return isSpecialOnCooldown;
	}

	public Timer GetSpecialTimer()
	{
		return specialCooldownTimer;
	}

	public Vector2 GetCurrentDirection()
	{
		return lastDirection;
	}
	
	public float GetHealth() {
		return health;
	}

}
