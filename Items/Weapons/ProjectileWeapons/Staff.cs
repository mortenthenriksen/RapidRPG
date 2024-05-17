using Godot;
using System;
using System.Linq;


public partial class Staff : Area2D 
{	

	PackedScene bulletScene = ResourceLoader.Load<PackedScene>("res://Projectiles/dropletBullet.tscn");
	Marker2D shootingPoint;
	private Area2D staff;

	private float elapsedTime;
	private float timeOfLastFire;
	private float fireRate = 0.15f;

	public override void _Ready() 
	{
		shootingPoint = GetNode<Marker2D>("Marker2D/StaffSprite/ShootingPoint");
	}

	public override void _PhysicsProcess(double delta)
	{
		elapsedTime += (float) delta;

		// Get the mouse position in the global coordinate system
		var mousePosition = GetGlobalMousePosition();

		// Make the staff look at the mouse position
		LookAt(mousePosition);

		if (elapsedTime > timeOfLastFire + fireRate) {
			Shoot(mousePosition);
			timeOfLastFire = elapsedTime;
		}
	}

	private void Shoot(Vector2 targetPosition) {
		var bullet = bulletScene.Instantiate() as Area2D;

		bullet.GlobalPosition = shootingPoint.GlobalPosition;
		
		// Set the bullet's rotation to point towards the target position
		bullet.LookAt(targetPosition);

		GetTree().Root.AddChild(bullet);
	}
}
