using Godot;


namespace Game.Manager;

[GlobalClass]


public partial class MainStatManager : Node
{

	public static MainStatManager Instance { get; private set; }

	[Export]
	private DefenseManager defenseManager;

	[Export]
	private DamageManager damageManager;

	private int strengthValue;
	private int dexterityValue;
	private int intelligenceValue;


	public override void _Ready()
	{
		strengthValue = 13;
		dexterityValue = 11;
		intelligenceValue = 7;
	}


	public int GetStrengthValue()
	{
		return strengthValue;
	}

	public int GetDexterityValue()
	{
		return dexterityValue;
	}
	
	public int GetIntelligenceValue()
	{
		return intelligenceValue;
	}



	public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }
}
