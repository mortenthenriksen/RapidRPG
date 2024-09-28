using Game.Autoload;
using Game.Characters;
using Godot;

namespace Game.Manager;

[GlobalClass]

public partial class DamageManager : Node
{

	public static DamageManager Instance { get; private set; }

	[Export]
	private Dave player;

	[Export]
	private PackedScene orcScene;

	[Export]
	private DamageNumbers damageNumbers;

	[Export]
	private Staff staff;
	
	public override void _Notification(int what)
    {
		if (what == NotificationSceneInstantiated) 
		{
			Instance = this;
		}
    }
	
	public override void _Ready()
	{
		CustomSignals.Instance.EnemyHitByBullet += OnEnemyHitByBullet;
		CustomSignals.Instance.EnemyDamageDealt += OnPlayerDamageReceived;
	}


    private void OnEnemyHitByBullet(Vector2 position)
	{
		damageNumbers.DisplayNumber(TotalDamageAmount(), position, false);
		CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageRecieved, TotalDamageAmount());
	}


    private void OnPlayerDamageReceived(float damageAmount)
    {
		damageNumbers.DisplayNumber(damageAmount, player.Position, false);
		player.PlayerDamageReceived(damageAmount, player.Position);
    }


	public float TotalDamageAmount()
	{
		// the final returning of the max dmg, will be affected by a lot of modifers later on
		return staff.GetDamageAmount() * 5;
	}
}
