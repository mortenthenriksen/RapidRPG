using Game.Autoload;
using Game.Characters;
using Godot;

namespace Game.Manager;


public partial class DamageManager : Node
{

	public static DamageManager Instance { get; private set; }

	[Export]
	private Dave player;

	[Export]
	private PackedScene orcScene;

	[Export]
	private PackedScene archerScene; 
	
	[Export]
	private DamageNumbers damageNumbers;
	
	public override void _Notification(int what)
    {
		if (what == NotificationSceneInstantiated) 
		{
			Instance = this;
		}
    }
	
	public override void _Ready()
	{
		// CustomSignals.Instance.EnemyHitByBullet += OnEnemyHitByBullet;
		CustomSignals.Instance.EnemyDamageDealt += OnPlayerDamageReceived;
		CustomSignals.Instance.EnemyDamageRecieved += OnEnemyDamageRecieved;
	}


    // private void OnEnemyHitByBullet(Vector2 position)
	// {
	// 	damageNumbers.DisplayNumber(TotalDamageAmount(), position, false);
	// 	CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.EnemyDamageRecieved, TotalDamageAmount());
	// }

	private void OnEnemyDamageRecieved(float damageAmount, Vector2 position)
	{
		damageNumbers.DisplayNumber((int)damageAmount, position, false);
	}


    private void OnPlayerDamageReceived(float damageAmount)
    {
		damageNumbers.DisplayNumber((int)damageAmount, player.Position, false);
		player.PlayerDamageReceived((int)damageAmount);
    }


	public float GetTotalDamageAmount()
	{
		// the final returning of the max dmg, will be affected by a lot of modifers later on
		// make it depend on the class chosen B-), just buy the character pack already
		return MainStatManager.Instance.GetStrengthValue() + EquippedItemsManager.Instance.GetAddedDamage();
	}
}
