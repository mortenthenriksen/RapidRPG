using Godot;

namespace Game.Autoload;

public partial class CustomSignals : Node
{

	public static CustomSignals Instance { get; private set; }

	[Signal]
	public delegate void EnemyHitByBulletEventHandler(Vector2 position);

	[Signal]
	public delegate void EnemyDamageRecievedEventHandler(float damageAmount, Vector2 position);

	[Signal]
	public delegate void EnemyHealthDepletedEventHandler(float health, Vector2 deathPosition);   

	[Signal]
	public delegate void EnemyDamageDealtEventHandler(float damageAmount);

	[Signal]
	public delegate void UpdateExperienceBarEventHandler();

	[Signal]
	public delegate void UpdateLevelEventHandler();

	[Signal]
	public delegate void TeleportBackToTownEventHandler(Vector2 position);

	[Signal]
	public delegate void DropItemOnGroundFromInventoryEventHandler();

	
    public override void _Notification(int what)
    {
		if (what == NotificationSceneInstantiated) 
		{
			Instance = this;
		}
    }
}
