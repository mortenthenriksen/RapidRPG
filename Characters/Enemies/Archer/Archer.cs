using Game.Manager;
using Godot;

namespace Game.Characters;

public partial class Archer : CharacterBody2D
{
    [Signal]
    public delegate void UpdateArcherHealthEventHandler(float health);

    [Export]
	private PackedScene arrowScene;

	private float damageAmount = 2;

	private float speed = 110; 
	private float MAX_HEALTH = 75;
	private float health = 75; 

	private ProgressBar healthBarOrc; 
	private Label healthLabelOrc;

	private AnimatedSprite2D animatedSprite2D; 
	private Dave player; 

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		healthLabelOrc = GetNode<Label>("HealthBar/HealthLabel");
		healthBarOrc = GetNode<ProgressBar>("HealthBar");
		player = GetNode<Dave>("/root/Main/Dave");

		health = MAX_HEALTH * MultiplierManager.Instance.GetMultiplier();

		UpdateHealthBar();
	}


	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();

		if (Velocity.Length() > 0)
		{
			animatedSprite2D.Play("run");
		}
		else 
		{
			animatedSprite2D.Play("idle");
		}

		if (Velocity.X > 0)
		{
			animatedSprite2D.FlipH = false;
		}
		else 
		{
			animatedSprite2D.FlipH = true;
		}
	}

	private void UpdateHealthBar()
	{
		EmitSignal(SignalName.UpdateArcherHealth, health);
		healthBarOrc.MaxValue = MAX_HEALTH * MultiplierManager.Instance.GetMultiplier();
		healthBarOrc.Value = health * MultiplierManager.Instance.GetMultiplier();
		healthLabelOrc.Text = $"Health: {healthBarOrc.Value.ToString("F0")}";
		if (health <= 0)
		{
			healthBarOrc.Visible = false;
			healthLabelOrc.Visible = false;
		}
	}
}
