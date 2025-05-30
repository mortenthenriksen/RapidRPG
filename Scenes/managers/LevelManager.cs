using System;
using Game.Autoload;
using Game.Characters;
using Godot;

namespace Game.Manager;

public partial class LevelManager : Node
{
    public static LevelManager Instance { get; private set; }

    private int levelNum = 1;
	private float experience;

    [Export]
    private Dave player;

    [Export]
    private ProgressBar experienceBar;

    // [Export]
    // private Label levelLabel;

    private AnimatedSprite2D newUp;
    private AnimatedSprite2D newUpUpper;
    private AnimatedSprite2D newUpUpper2;
    private AudioStreamPlayer2D audioStreamPlayer2D;
    private Timer lightTimer;
    private PointLight2D pointLight2D;

    public override void _Ready()
    {
        newUp = GetNode<AnimatedSprite2D>("NewUp"); 
        newUpUpper = GetNode<AnimatedSprite2D>("NewUpUpper"); 
        newUpUpper2 = GetNode<AnimatedSprite2D>("NewUpUpper2"); 
        audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        lightTimer = GetNode<Timer>("LightTimer");
        pointLight2D = GetNode<PointLight2D>("PointLight2D");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pick_up"))
        {
            LevelGainedUI();
        }

        pointLight2D.Color = new Color(1, 1, 0.8f, (float)lightTimer.TimeLeft);

        newUp.Position = player.Position + new Vector2(0, 10);
        newUpUpper.Position = player.Position + new Vector2(0, 10);
        newUpUpper2.Position = player.Position + new Vector2(0, 10);
        pointLight2D.Position = player.Position + new Vector2(0, 10);
    }

    public void HandleExperienceGained()
    {
        // Make an experience manager here, that takes mob type and  multiplier into account
        experience += 21;
		CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.UpdateExperienceBar, experience);
		
        var experienceMaxValue = ExperienceForLevel();
		experienceBar.Value = experience;
        experienceBar.MaxValue = experienceMaxValue;

		if (experienceMaxValue - experience <= 0) 
		{
            var experienceForNextLevel = Math.Abs((int)experienceMaxValue - experience);
            LevelGainedUI();
			// levelNum += 1;
			// levelLabel.Text = $"Level: {GetCurrentLevel()}";
            experienceBar.Value = experienceForNextLevel;
            experience = experienceForNextLevel;
            // CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.UpdateLevel);
		}
    }


    private double ExperienceForLevel()
    {   
        // tweak this to make leveling slower / faster
        var baseExp = 100;
        var logBase = 1.5;
        var offset = 1;
        var power = 1.5;
        return (int)baseExp * Math.Pow(Math.Log(GetCurrentLevel() + offset, logBase), power);
    }


    public int GetCurrentLevel()
    {
        return levelNum;
    }

    public void IncreaseLevel()
    {
        levelNum++;
        LevelGainedUI();
    }

    private void LevelGainedUI()
    {
        lightTimer.Start();
        newUp.Play();
        newUpUpper.Play();
        newUpUpper2.Play();
        audioStreamPlayer2D.Play();

    }

    public override void _Notification(int what)
    {
		if (what == NotificationSceneInstantiated) 
		{
			Instance = this;
		}
    }
}
