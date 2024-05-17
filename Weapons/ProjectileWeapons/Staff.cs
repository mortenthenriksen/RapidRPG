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
	private float fireRate = 0.25f;

	public override void _Ready() 
	{
		shootingPoint = GetNode<Marker2D>("Marker2D/Staff/ShootingPoint");
	}

	public override void _PhysicsProcess(double delta)
	{
		elapsedTime += (float) delta;
		var enemiesInRange = GetOverlappingBodies();
		if ( enemiesInRange.Count > 0) {
			var targetEnemy = enemiesInRange.First();
			LookAt(targetEnemy.GlobalPosition);
		}

		if (elapsedTime > timeOfLastFire + fireRate && enemiesInRange.Count > 0) {
			Shoot();
			timeOfLastFire = elapsedTime;
		}
	}

	private void Shoot() {
		var bullet = bulletScene.Instantiate() as Area2D;

		bullet.GlobalPosition = shootingPoint.GlobalPosition;
		
		bullet.GlobalRotation = shootingPoint.GlobalRotation;

		GetTree().Root.AddChild(bullet);

	}
}
