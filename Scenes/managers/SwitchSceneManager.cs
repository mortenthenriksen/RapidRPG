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

	private Managers managers;

    public override void _Ready()
	{
		managers = (Managers)this.GetParent();

		CustomSignals.Instance.TeleportBackToTown += OnTeleportBackToTown;
	}

	
	public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("exit"))
		{
			GetTree().Quit();	
		}
    }



    private void OnTeleportBackToTown()
    {	
		var isInTownVector = new Vector2(600, 2600);
		var currentPosition = player.Position;

		// Check if player is within 1000 pixels of town position
		if ((Math.Abs(currentPosition.X - isInTownVector.X) < 1000) && 
			(Math.Abs(currentPosition.Y - isInTownVector.Y) < 1000))
		{
			// Teleport to alternate position
			player.Position = new Vector2(534, 435);
		}
		else
		{
			// Teleport to town position
			player.Position = isInTownVector;
		}

		// if (GetTree().CurrentScene != townScenePacked.Instantiate())
		// {
		// 	// var townScene = townScenePacked.Instantiate();
		// 	// var oldScene = GetTree().CurrentScene;
		// 	// GetTree().Root.AddChild(townScene);
		// 	// GetTree().CurrentScene = townScene;

		// 	// Wait for the scene to change
		// 	await ToSignal(GetTree(), "process_frame");


		// 	// GetTree().ChangeSceneToPacked(townScenePacked);

		// 	// foreach (var child in oldScene.GetChildren() )
		// 	// {
		// 	// 	// GD.Print(child.Name);
		// 	// 	if (child is not Dave && child is not Handlers)
		// 	// 	{
		// 	// 		GD.Print(child.Name);
		// 	// 		child.CallDeferred("queue_free");
		// 	// 	}
		// 	// }

		// 	// player.Reparent(townScene);
		// 	// handlers.Reparent(townScene);

		// 	// player.Position = new Vector2(468,500);
		// 	// oldScene.CallDeferred("queue_free");

		// 	// // // Force garbage collection to ensure all resources are freed
		// 	// GC.Collect();
		// 	// GC.WaitForPendingFinalizers();
		// }
    }

}
