using Godot;
using System;
using System.Collections;

public partial class Game : Node2D
{

	[Signal]
	public delegate void UpdateExperienceBarEventHandler();

	[Signal]
	public delegate void UpdateLevelEventHandler();
	

	private PackedScene mobScene = ResourceLoader.Load<PackedScene>("res://Characters/Enemies/Orc.tscn");
	private PackedScene treeScene = ResourceLoader.Load<PackedScene>("res://Environment/roundTree.tscn");
	private PackedScene itemDropScene = ResourceLoader.Load<PackedScene>("res://InventoryGD/ItemDrop.tscn");
	private Dave player;
	private Orc orc;
	private Orc newMob;
	private float experience;
	private float health;
	private PathFollow2D pathFollow2D;
	private CanvasLayer gameOverScreen;
	private ProgressBar experienceBar;
	private Label levelLabel;
	private int levelNum = 1;
	private CustomSignals customSignals;
	


	public override void _Ready()
	{
		customSignals = GetNode<CustomSignals>("/root/CustomSignals");
		
		pathFollow2D = GetNode<PathFollow2D>("/root/Game/Dave/Path2D/PathFollow2D");
		gameOverScreen = GetNode<CanvasLayer>("GameOverScreen");
		experienceBar = GetNode<ProgressBar>("/root/Game/Dave/ExperienceBar");
		levelLabel = GetNode<Label>("/root/Game/Dave/ExperienceBar/LevelLabel");

		player = GetNode<Dave>("Dave");
		orc = GetNode<Orc>("Orc");

		// ChangeWindowSize(new Vector2(1280, 720));
	}

	private void ChangeWindowSize(Vector2 newSize)
	{
		GetWindow().Size = (Vector2I) newSize;
	}

	private void SpawnMob() {
		newMob = mobScene.Instantiate() as Orc;
		Random random = new Random();
		pathFollow2D.ProgressRatio = (float) random.NextDouble();
		newMob.GlobalPosition = pathFollow2D.GlobalPosition;

		newMob.HealthDepletedOrc += OnEnemyHealthDepleted;

		GetTree().Root.AddChild(newMob);
	}
	

	private void SpawnTree() {
		var newTree = treeScene.Instantiate() as StaticBody2D;
		Random random = new Random();
		pathFollow2D.ProgressRatio = (float) random.NextDouble();
		newTree.GlobalPosition = pathFollow2D.GlobalPosition;
		GetTree().Root.AddChild(newTree);
	}

	private void OnMobTimerTimeout(){
		SpawnMob();
	}

	private void OnTreeTimerTimeout() {
		SpawnTree();
	}


	public void OnDaveHealthDepleted(float health) {
		gameOverScreen.Visible = true;
		GetTree().Paused = true;
	}

	public void OnEnemyHealthDepleted(float health, Vector2 position) {
		experience += 10;
		EmitSignal(SignalName.UpdateExperienceBar, experience);
			
		experienceBar.Value = experience % 100;
		if (experience % 100 == 0) 
		{
			levelNum += 1;
			levelLabel.Text = $"Level: {levelNum}";
			EmitSignal(SignalName.UpdateLevel);
		}
		MakeItemDrop(position);
	}


	public void MakeItemDrop(Vector2 position) 
	{
		var newItemDrop = itemDropScene.Instantiate() as ItemDrop;
		newItemDrop.GlobalPosition = position;
		GetTree().Root.CallDeferred("add_child", newItemDrop);
	}
}
