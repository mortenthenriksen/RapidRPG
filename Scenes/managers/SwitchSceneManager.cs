using System;
using Game.Autoload;
using Game.Characters;
using Godot;

namespace Game.Manager;

public partial class SwitchSceneManager : Node
{
	[Export]
	private Dave player;

	[Export]
	private PackedScene townScenePacked;
	
	private readonly Vector4 townCameraBounds = new(0, 1998, 1318, 2946);
	private Camera2D playerCamera;


	public override void _Ready()
	{
		CustomSignals.Instance.TeleportBackToTown += OnTeleportBackToTown;
		player = GetNode<Dave>("/root/Main/Dave");
		playerCamera = player.GetNode<Camera2D>("Camera2D");
	}

	
	public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("exit"))
		{
			GetTree().Quit();	
		}
    }

	private void SetTownCameraBounds(bool enabled)
	{
		if (playerCamera == null) return;

		if (enabled)
		{
			playerCamera.LimitLeft = (int)townCameraBounds.X;
			playerCamera.LimitTop = (int)townCameraBounds.Y;
			playerCamera.LimitRight = (int)townCameraBounds.Z;
			playerCamera.LimitBottom = (int)townCameraBounds.W;
		}
		else
		{
			playerCamera.LimitLeft = -10000000;
			playerCamera.LimitTop = -10000000;
			playerCamera.LimitRight = 10000000;
			playerCamera.LimitBottom = 10000000;
		}
	}


    private void OnTeleportBackToTown(Vector2 position)
	{
		var isInTownVector = new Vector2(600, 2600);
		var currentPosition = player.Position;

		// Check if player is within 1000 pixels of town position
		if ((Math.Abs(currentPosition.X - isInTownVector.X) < 1000) &&
			(Math.Abs(currentPosition.Y - isInTownVector.Y) < 1000))
		{
			// Teleport to alternate position
			SetTownCameraBounds(false);
			// hard-coded for the old positon, this should be saved somewhere before teleporting
			player.Position = position;
			player.resetLastWorldPosition();
		}
		else
		{
			SetTownCameraBounds(true);
			player.Position = isInTownVector;
		}
	}

}
