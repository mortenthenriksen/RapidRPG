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
	private Dave player;

	[Export]
	private PackedScene itemDropScene;

	private Orc spawnedOrc;
	private PathFollow2D pathFollow2D;
	private CanvasLayer gameOverScreen;

	public override void _Ready()
	{
		pathFollow2D = GetNode<PathFollow2D>("/root/Main/Dave/Path2D/PathFollow2D");
		gameOverScreen = GetNode<CanvasLayer>("GameOverScreen");

		player.PlayerHealthDepleted += OnPlayerHealthDepleted; 
		CustomSignals.Instance.EnemyHealthDepleted += OnEnemyHealthDepleted;
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


	private void OnPlayerHealthDepleted(float health) {
		gameOverScreen.Visible = true;
		GetTree().Paused = true;
	}

	private void OnEnemyHealthDepleted(float health, Vector2 position) {
		LevelManager.Instance.HandleExperienceGained();
		MakeItemDrop(position);
	}

	public void MakeItemDrop(Vector2 position) 
	{
		var newItemDrop = itemDropScene.Instantiate() as ItemDrop;
		newItemDrop.GlobalPosition = position;
		GetTree().Root.CallDeferred("add_child", newItemDrop);
	}

	public void MakeItemDropFromInventory(string itemName)
	{
		var newItemDrop = itemDropScene.Instantiate() as ItemDrop;
		newItemDrop.GlobalPosition = GetGlobalMousePosition();
		newItemDrop.SetItemName(itemName);
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
