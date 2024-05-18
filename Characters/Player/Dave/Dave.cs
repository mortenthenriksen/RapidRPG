using Godot;
using System;
using System.Linq;

public partial class Dave : CharacterBody2D
{
	private float speed = 300;
	private bool isRolling = false;
	private float health = 50.0f;
	
	[Signal]
	public delegate void UpdateHealthEventHandler(float health);
    
	[Signal]
	public delegate void HealthDepletedEventHandler(float health);    

	[Signal]
    public delegate void DamageReceivedEventHandler(float damageAmount);

	
	private CustomSignals customSignals;
	private AnimatedSprite2D animatedSprite2D;
	private ProgressBar healthBar;
	private Label healthLabel;
	private DamageNumbers damageNumbersOrigin;
	private Vector2 lastMoveDirection = new Vector2(0, 0);
	private PickupBox pickupBox;
	private ItemDrop itemDrop;

	private Node playerInventory;


	public override void _Ready()
	{
		damageNumbersOrigin = GetNode<DamageNumbers>("/root/DamageNumbers");
		playerInventory = GetNode<Node>("/root/PlayerInventory");

		customSignals = GetNode<CustomSignals>("/root/CustomSignals");
		customSignals.DamagePlayer += HandleDamagePlayer;
		customSignals.HealthDepleted += HandleHealthDepleted;
		
		
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		healthBar = GetNode<ProgressBar>("HealthBar");
		healthLabel = GetNode<Label>("HealthBar/HealthLabel");
		pickupBox = GetNode<PickupBox>("PickupBox");
	}

	private void HandleDamagePlayer(float damageAmount) {
		health -= damageAmount;
		damageNumbersOrigin.DisplayNumber(damageAmount, GlobalPosition, false);
		EmitSignal(SignalName.UpdateHealth, health);
		healthBar.Value = health;
		healthLabel.Text = $"Health: {health}";
		if (health <= 0) 
		{
			EmitSignal(nameof(CustomSignals.HealthDepleted), health);
		}
		
	}

	public void HandleHealthDepleted(float health) {
		EmitSignal(SignalName.HealthDepleted, health);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 Move = HandleInput();
		PlayAnimation(Move);
		MoveAndCollide(Move*speed* (float) delta);


		if (pickupBox.GetOverlappingBodies().Count > 0) 
		{
			var itemDrop = (ItemDrop)pickupBox.GetOverlappingBodies().First();
			GD.Print(itemDrop.Name);
			itemDrop.PickupItem(this);
		}
	}


	private Vector2 HandleInput()
	{
		Vector2 Move = new Vector2(0,0);

		if (Input.IsActionPressed(InputConstants.MOVE_UP))
		{
			Move.Y -= 1;
			lastMoveDirection = Move;
		}

		if (Input.IsActionPressed(InputConstants.MOVE_DOWN))
		{
			Move.Y += 1;
			lastMoveDirection = Move;
		}

		if (Input.IsActionPressed(InputConstants.MOVE_LEFT))
		{
			Move.X -= 1;
			lastMoveDirection = Move;
		}

		if (Input.IsActionPressed(InputConstants.MOVE_RIGHT))
		{
			Move.X += 1;
			lastMoveDirection = Move;
		}


		if (Input.IsActionPressed(InputConstants.ROLL))
		{   
			//Move += lastMoveDirection * 2.5f;
		}

		return Move;
	}

	private void PlayAnimation(Vector2 direction) 
	{
		string action;

		// if (Input.IsActionPressed(InputConstants.ROLL))
		// {
		//     action = "roll";
		//     direction = lastMoveDirection;
		// }

		if (direction == Vector2.Zero) 
		{
			action = "idle";
			direction = lastMoveDirection; 
		}
		
		else
		{
			action = "run";
		}

		string directionSuffix = direction switch
		{
			Vector2 d when d == Vector2.Up => "Up",
			Vector2 d when d == Vector2.Down => "Down",
			Vector2 d when d == Vector2.Left => "Left",
			Vector2 d when d == Vector2.Right => "Right",
			Vector2 d when d == Vector2.Up + Vector2.Right => "UpRight",
			Vector2 d when d == Vector2.Up + Vector2.Left => "UpLeft",
			Vector2 d when d == Vector2.Down + Vector2.Right => "DownRight",
			Vector2 d when d == Vector2.Down + Vector2.Left => "DownLeft",
			_ => ""
		};

		string animationName = action + directionSuffix;
		animatedSprite2D.Play(animationName);
	}
	

	public Vector2 GetCurrentDirection()
	{
		return lastMoveDirection;
	}

	public float GetHealth() {
		return health;
	}
}
