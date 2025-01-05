using Game.Autoload;
using Game.Characters;
using Godot;
using System;

namespace Game.Manager;

public partial class SwitchSceneManager : Node
{
	[Export]
	private Dave player;

	[Export]
	private PackedScene townScenePacked;

	private Handlers handlers;

    public override void _Ready()
	{
		handlers = (Handlers)this.GetParent();

		CustomSignals.Instance.TeleportBackToTown += OnTeleportBackToTown;
	}

	
	public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("exit"))
		{
			GetTree().Quit();	
		}
    }



    private async void OnTeleportBackToTown()
    {	
		if (GetTree().CurrentScene != townScenePacked.Instantiate())
		{
			// var townScene = townScenePacked.Instantiate();
			// var oldScene = GetTree().CurrentScene;
			// GetTree().Root.AddChild(townScene);
			// GetTree().CurrentScene = townScene;

			// Wait for the scene to change
			await ToSignal(GetTree(), "process_frame");

			GetTree().ChangeSceneToPacked(townScenePacked);

			// foreach (var child in oldScene.GetChildren() )
			// {
			// 	// GD.Print(child.Name);
			// 	if (child is not Dave && child is not Handlers)
			// 	{
			// 		GD.Print(child.Name);
			// 		child.CallDeferred("queue_free");
			// 	}
			// }

			// player.Reparent(townScene);
			// handlers.Reparent(townScene);

			// player.Position = new Vector2(468,500);
			// oldScene.CallDeferred("queue_free");

			// // // Force garbage collection to ensure all resources are freed
			// GC.Collect();
			// GC.WaitForPendingFinalizers();
		}
    }

}
