using Game.Autoload;
using Game.Characters;
using Game.Manager;
using Godot;
using System;

public partial class Main : Node2D
{
	
	[Export]
	private PackedScene treeScene;

	[Export]
	private PackedScene orcScene;

	[Export]
	private PackedScene townScenePacked;

	[Export]
	private Dave player;

	[Export]
	private PackedScene itemDropScene;

	[Export]
	private DamageNumbers damageNumbers;

	private Orc spawnedOrc;
	private PathFollow2D pathFollow2D;
	private CanvasLayer gameOverScreen;
	private TileMapLayer tileMapLayer;
	private Handlers handlers;


	public override void _Ready()
	{
		pathFollow2D = GetNode<PathFollow2D>("/root/Main/Dave/Path2D/PathFollow2D");
		tileMapLayer = GetNode<TileMapLayer>("TileMapAroundThePlayer");
		gameOverScreen = GetNode<CanvasLayer>("GameOverScreen");
		handlers = GetNode<Handlers>("Handlers");

		player.PlayerHealthDepleted += OnPlayerHealthDepleted; 
		CustomSignals.Instance.EnemyHealthDepleted += OnEnemyHealthDepleted;
		CustomSignals.Instance.TeleportBackToTown += OnTeleportBackToTown;
	}

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("exit"))
		{
			GetTree().Quit();	
		}
    }


    private void SpawnMob() {
		spawnedOrc = orcScene.Instantiate() as Orc;
		Random random = new Random();
		pathFollow2D.ProgressRatio = (float) random.NextDouble();
		spawnedOrc.GlobalPosition = pathFollow2D.GlobalPosition;
		GetTree().Root.AddChild(spawnedOrc);
	}


    private void SpawnTree() {
		var newTree = treeScene.Instantiate() as StaticBody2D;
		Random random = new Random();
		pathFollow2D.ProgressRatio = (float) random.NextDouble();
		newTree.GlobalPosition = pathFollow2D.GlobalPosition;
		GetTree().Root.AddChild(newTree);
	}

	private async void OnTeleportBackToTown()
    {	
		if (GetTree().CurrentScene != townScenePacked.Instantiate())
		{
			var townScene = townScenePacked.Instantiate();
			GetTree().Root.AddChild(townScene);
			GetTree().CurrentScene = townScene;

			// Wait for the scene to change
			await ToSignal(GetTree(), "process_frame");

			foreach (var child in this.GetChildren() )
			{
				GD.Print(child.Name);
				if (child is IEnemies)
				{
					QueueFree();
				}
			}

			player.Reparent(townScene);
			tileMapLayer.Reparent(townScene);
			handlers.Reparent(townScene);

			player.Position = new Vector2(468,500);
		}
    }


	private void OnPlayerHealthDepleted(float health) {
		gameOverScreen.Visible = true;
		GetTree().Paused = true;
	}

	private void OnEnemyHealthDepleted(float health, Vector2 position) {
		LevelManager.Instance.HandleExperienceGained();
		MakeItemDrop(position);
	}

	private void MakeItemDrop(Vector2 position) 
	{
		var newItemDrop = itemDropScene.Instantiate() as ItemDrop;
		newItemDrop.GlobalPosition = position;
		GetTree().Root.CallDeferred("add_child", newItemDrop);
	}

	private void OnMobTimerTimeout()
	{
		SpawnMob();
	}

	private void OnTreeTimerTimeout() 
	{
		SpawnTree();
	}
}
