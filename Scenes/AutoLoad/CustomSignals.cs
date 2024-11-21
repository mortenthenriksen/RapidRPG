using Godot;

namespace Game.Autoload;

[GlobalClass]

public partial class CustomSignals : Node
{

	public static CustomSignals Instance { get; private set; }

	[Signal]
	public delegate void EnemyHitByBulletEventHandler(Vector2 position);

	[Signal]
	public delegate void EnemyDamageRecievedEventHandler(Vector2 position);

	[Signal]
	public delegate void EnemyHealthDepletedEventHandler(float health, Vector2 deathPosition);   

	[Signal]
	public delegate void EnemyDamageDealtEventHandler(float damageAmount);

	[Signal]
	public delegate void UpdateExperienceBarEventHandler();

	[Signal]
	public delegate void UpdateLevelEventHandler();



	
    public override void _Notification(int what)
    {
		if (what == NotificationSceneInstantiated) 
		{
			Instance = this;
		}
    }
}
