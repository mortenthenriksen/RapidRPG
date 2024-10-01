using Game.Characters;
using Godot;

namespace Game.Manager;

public partial class MultiplierManager : Node
{
	public static MultiplierManager Instance { get; private set; }

	[Export]
	private Dave player;
	private Vector2 playerInitialPositon;
	private float currentMultiplier;
	private int planesMoved;

	public override void _Ready()
	{
		currentMultiplier = 1.00f;
		planesMoved = 0;
		playerInitialPositon = player.Position;
	}


	public override void _Process(double delta)
	{
		float distanceMoved = playerInitialPositon.Y - player.Position.Y;

		int currentPlane = Mathf.FloorToInt(distanceMoved / 500.0f);

		if (currentPlane != planesMoved)
		{
			if (currentPlane > planesMoved)
			{
				currentMultiplier += 0.10f;
			}
			else
			{
				currentMultiplier -= 0.10f;
			}

			planesMoved = currentPlane;
			// GD.Print("Planes Moved: ", planesMoved);
			// GD.Print("Current Multiplier: ", currentMultiplier);
		}
	}


	public float GetMultiplier()
	{
		return currentMultiplier;
	}

	public override void _Notification(int what)
    {
		if (what == NotificationSceneInstantiated) 
		{
			Instance = this;
		}
    }
}
