using Godot;
using System.Linq;

namespace Game.Characters;

public partial class Dave : CharacterBody2D
{
	private float speed = 300;
	private float MAX_HEALTH = 75.0f;
	private float health = 75.0f;
	private float healthRegenAmount = 0.25f;
	private readonly StringName MOVE_LEFT = "move_left";
	private readonly StringName MOVE_RIGHT = "move_right";
	private readonly StringName MOVE_UP = "move_up";
	private readonly StringName MOVE_DOWN = "move_down";

	[Signal]
	public delegate void UpdatePlayerHealthEventHandler(float health);

	[Signal]
	public delegate void PlayerHealthDepletedEventHandler(float health);

	private AnimatedSprite2D animatedSprite2D;
	private ProgressBar healthBar;
	private Label healthLabel;
	private Vector2 moveDirection;
	private PickupBox pickupBox;
	private ItemDrop itemDrop;
	private Timer healthRegenTimer;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		healthBar = GetNode<ProgressBar>("HealthBar");
		healthLabel = GetNode<Label>("HealthBar/HealthLabel");
		pickupBox = GetNode<PickupBox>("PickupBox");

		UpdateHealthBar();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 moveDirection = HandleInput();
		PlayAnimation(moveDirection);
		MoveAndCollide(moveDirection * speed * (float) delta);

		if (pickupBox.GetOverlappingBodies().Count > 0) 
		{
			var itemDrop = (ItemDrop)pickupBox.GetOverlappingBodies().First();
			itemDrop.PickupItem(this);
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
		return moveDirection = Input.GetVector(MOVE_LEFT, MOVE_RIGHT, MOVE_UP, MOVE_DOWN);
	}

	private void PlayAnimation(Vector2 direction) 
	{
		string action;

		if (direction == Vector2.Zero) 
		{
			action = "idle";
			direction = moveDirection;
		}

		if (Input.IsActionPressed("attack"))
		{
			action = "sword";
			direction = moveDirection;
		}
		
		if (Input.IsActionPressed("special_attack"))
		{
			action = "special";
			direction = moveDirection;
			GD.Print("test");
		}
		
		else
		{
			action = "run";
			moveDirection = direction; 
		}

		string directionSuffix = direction switch
		{
			Vector2 d when d == Vector2.Up => "Up",
			Vector2 d when d == Vector2.Down => "Down",
			Vector2 d when d == Vector2.Left => "Left",
			Vector2 d when d == Vector2.Right => "Right",
			// slightly cursed, but i dont care :)
			Vector2 d when d == new Vector2(-(float)0.70710677, -(float)0.70710677) => "UpLeft", 
			Vector2 d when d == new Vector2((float)0.70710677, -(float)0.70710677) => "UpRight", 
			Vector2 d when d == new Vector2(-(float)0.70710677, (float)0.70710677) => "DownLeft", 
			Vector2 d when d == new Vector2((float)0.70710677, (float)0.70710677) => "DownRight", 
			_ => ""
		};

		string animationName = action + directionSuffix;
		animatedSprite2D.Play(animationName);
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
