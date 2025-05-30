using Game.Autoload;
using Game.Inventory;
using Game.Manager;
using Game.UI;
using Godot;
using System;
using System.Collections.Generic;
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
	private Vector2 mouseDirection = Vector2.Right;
	private string action;

	private AnimatedSprite2D animatedSprite2D;
	private ProgressBar healthBar;
	private Label healthLabel;
	private ProgressBar rageBar;
	private Label rageLabel;
	private PickupBox pickupBox;
	private Area2D attackBoxBasic;
	private Area2D attackBoxSpin;
	private AttackBoxPolygon basicAttackBoxPolygonShape;
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

    // Replace the single item reference with a list
    private List<ItemDrop> nearbyItems = new List<ItemDrop>();
    private bool isNearItem => nearbyItems.Count > 0;

	private float shoesEffectTimer = 0f;
	private const float SHOES_EFFECT_INTERVAL = 0.5f; // Adjust this value to change the interval
	private float pickupCooldown = 0.2f; // Adjust this value to change the cooldown duration
	private float currentPickupCooldown = 0f;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		healthBar = GetNode<ProgressBar>("UserInterface/SkillBar/MarginContainer/HBoxContainer/VBoxHealth/HealthBar");
		healthLabel = GetNode<Label>("UserInterface/SkillBar/MarginContainer/HBoxContainer/VBoxHealth/HealthLabel");

		rageBar = GetNode<ProgressBar>("UserInterface/SkillBar/MarginContainer/HBoxContainer/VBoxRage/RageBar");
		rageLabel = GetNode<Label>("UserInterface/SkillBar/MarginContainer/HBoxContainer/VBoxRage/RageLabel");

		pickupBox = GetNode<PickupBox>("PickupBox");
		attackBoxBasic = GetNode<Area2D>("AttackBoxBasic");
		attackBoxSpin = GetNode<Area2D>("AttackBoxSpin");

		basicAttackBoxPolygonShape = GetNode<AttackBoxPolygon>("%BasicAttackBoxPolygonShape");

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
		// Update pickup cooldown
		if (currentPickupCooldown > 0)
		{
			currentPickupCooldown -= (float)delta;
		}

		Vector2 mousePosition = GetGlobalMousePosition();
		mouseDirection = (mousePosition - GlobalPosition).Normalized();

		if (!isAttacking || (isAttacking && action == "spinAttack"))
		{
			moveDirection = HandleInput();
			if (moveDirection != Vector2.Zero)
			{
				lastDirection = moveDirection;
				basicAttackBoxPolygonShape.RotateAttackBox(mouseDirection);

				shoesEffectTimer += (float)delta;
				if (shoesEffectTimer >= SHOES_EFFECT_INTERVAL)
				{
					EquippedItemsManager.Instance.FindUniqueEffectForShoes();
					shoesEffectTimer = 0f;
				}
			}

			PlayAnimation(moveDirection);
			float currentSpeed = (isAttacking && action == "spinAttack") ? speed * 0.5f : speed;
			MoveAndCollide(moveDirection * currentSpeed * (float)delta);
		}

		if (isAttacking && !hasDealtDamage)
		{	
			if (action == "sword")
			{
				foreach (var body in attackBoxBasic.GetOverlappingBodies())
				{
					if (body is IEnemies enemies)
					{
						AttackEnemy(enemies);
						hasDealtDamage = true;
					}
				}
			}

			else if (action == "spinAttack")
			{
				foreach (var body in attackBoxSpin.GetOverlappingBodies())
				{
					if (body is IEnemies enemy)
					{
						AttackEnemy(enemy);
						hasDealtDamage = true;
					}
				}
			}
		}
	}

    private void AttackEnemy(CharacterBody2D body)
    {
		if (body is IEnemies enemies)
        {
			if (action == "sword")
			{
            	enemies.TakeDamage(DamageManager.Instance.GetTotalDamageAmount());
			}
			else if (action == "spinAttack")
			{
				enemies.TakeDamage(DamageManager.Instance.GetTotalDamageAmount() * 0.6f);
			}
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

    // Modify OnPickupBoxBodyEntered
    private void OnPickupBoxBodyEntered(Node2D body)
    {
        if (body is ItemDrop item)
        {
            nearbyItems.Add(item);
            item.HighlightLabel();
        }
    }

    // Modify OnPickupBoxBodyExited
    private void OnPickupBoxBodyExited(Node2D body)
    {
        if (body is ItemDrop item)
        {
            nearbyItems.Remove(item);
            item.UnhighlightLabel();
        }
    }

	private Vector2 HandleInput()
	{
		if (!isDashing)
		{
			Vector2 moveDirection = Input.GetVector(MOVE_LEFT, MOVE_RIGHT, MOVE_UP, MOVE_DOWN);
			return moveDirection;
		}
		return moveDirection;
	}

	private void PlayAnimation(Vector2 direction) 
	{
		animatedSprite2D.SpeedScale = 1;

		if (!inventoryPanel.GetIsMouseHoveringInventory() && !isDashing)
		{
			if (Input.IsActionPressed("attack") && !isAttacking && currentPickupCooldown <= 0) 
			{
                if (isNearItem)
                {
                    // Get the closest item to pick up
                    var closestItem = nearbyItems
                        .OrderBy(item => GlobalPosition.DistanceSquaredTo(item.GlobalPosition))
                        .FirstOrDefault();

					if (closestItem != null)
					{
						closestItem.PickupItem(this);
						nearbyItems.Remove(closestItem);
						currentPickupCooldown = 0.2f; // Short cooldown after pickup
                    }
                }
				else if (currentPickupCooldown <= 0)
				{
					// Normal attack logic
					audioStreamPlayer2D.Play();
					animatedSprite2D.SpeedScale = EquippedItemsManager.Instance.GetTotalAttackSpeed();
					EquippedItemsManager.Instance.FindUniqueEffectForWeapon();
					action = "sword";
					isAttacking = true;
					direction = mouseDirection;
				}
			}
		}

		if (Input.IsActionPressed("spinAttack") && !isAttacking)
		{
			audioStreamPlayer2D.Play();
			action = "spinAttack";
			isAttacking = true;
		}

		else if (Input.IsActionJustPressed("special_attack") && !isSpecialOnCooldown && isAttacking)
		{
			action = "special";
			isAttacking = true;
			specialCooldownTimer.Start();
			isSpecialOnCooldown = true;
		}


		else if (Input.IsActionJustPressed("dash") && !isDashOnCooldown)
		{
			isDashing = true;
			animatedSprite2D.SelfModulate = new Color(3f, 3f, 3f, 1f);
			if (dashTween != null)
			{
				dashTween.Kill();
			}

			dashTween = GetTree().CreateTween();
			Vector2 targetPosition = Position + lastDirection * 225;

			dashTween
				.TweenProperty(this, "position", targetPosition, 0.7f)
				.SetTrans(Tween.TransitionType.Cubic)
				.SetEase(Tween.EaseType.Out)
				.Connect("finished", new Callable(this, nameof(OnDashComplete)));

			dashCooldownTimer.Start();
			isDashOnCooldown = true;
		}

		else if (Input.IsActionJustPressed("teleport"))
		{
			// GD.Print("port back to town shorty");
			CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.TeleportBackToTown);
		}
		else if (isAttacking)
		{
			// Keep the current action and direction
			direction = mouseDirection;
		}

		else if (direction != Vector2.Zero)
		{
			animatedSprite2D.SpeedScale = 1;
			action = "run";
		}
		else
		{
			action = "idle";
		}

		string directionSuffix = GetDirectionSuffix(direction != Vector2.Zero ? direction : (isAttacking ? mouseDirection : lastDirection));
		animatedSprite2D.Play(action + directionSuffix);
	}


    private void OnDashComplete()
	{
		isDashing = false;
		animatedSprite2D.SelfModulate = new Color(1, 1, 1, 1);
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
		if (direction == Vector2.Zero)
			return "";

		// Calculate angle in radians (-π to π)
		float angle = Mathf.Atan2(direction.Y, direction.X);
		
		// Convert to degrees and shift range to 0 to 360
		float degrees = Mathf.RadToDeg(angle);
		if (degrees < 0)
			degrees += 360;

		// Check which quadrant we're in
		// Right quadrant: -45 to 45 degrees
		// Down quadrant: 45 to 135 degrees
		// Left quadrant: 135 to 225 degrees
		// Up quadrant: 225 to 315 degrees
		return degrees switch
		{
			>= 315 or < 45 => "Right",
			>= 45 and < 135 => "Down",
			>= 135 and < 225 => "Left",
			>= 225 and < 315 => "Up",
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

	public bool getIsDashing()
	{
		return isDashing;
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
