using Godot;

public partial class Staff : Area2D 
{	

	[Export]
	private PackedScene bulletScene; 

	private Marker2D shootingPoint;
	private Area2D staff;

	private float elapsedTime;
	private float timeOfLastFire;
	private float fireRate = 0.15f;
	private float damageAmount = 10.0f; 

	public override void _Ready() 
	{
		shootingPoint = GetNode<Marker2D>("Marker2D/StaffSprite/ShootingPoint");
	}

	public override void _PhysicsProcess(double delta)
	{
		elapsedTime += (float) delta;

		var mousePosition = GetGlobalMousePosition();

		LookAt(mousePosition);

		if (elapsedTime > timeOfLastFire + fireRate) {
			ShootBullet(mousePosition);
			timeOfLastFire = elapsedTime;
		}
	}

	private void ShootBullet(Vector2 targetPosition) {
		var bullet = bulletScene.Instantiate() as Area2D;

		bullet.GlobalPosition = shootingPoint.GlobalPosition;

		bullet.LookAt(targetPosition);

		GetTree().Root.AddChild(bullet);
	}

	public float GetDamageAmount()
	{
		return damageAmount;
	}
}
