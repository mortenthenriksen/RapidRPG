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

    [Export]
    private Label levelLabel;

    private AnimatedSprite2D levelUpAnimation;
    private AnimatedSprite2D levelUpAnimation2;
    private AnimatedSprite2D levelUpAnimation3;

    public override void _Ready()
    {
        levelUpAnimation = GetNode<AnimatedSprite2D>("LevelUpAnimation");
        levelUpAnimation2 = GetNode<AnimatedSprite2D>("LevelUpAnimation2");
        levelUpAnimation3 = GetNode<AnimatedSprite2D>("LevelUpAnimation3"); 
    }

    public override void _Process(double delta)
    {
        levelUpAnimation.Position = player.Position;
        levelUpAnimation2.Position = player.Position;
        levelUpAnimation3.Position = player.Position;
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
            LevelGainedAnimation();
			levelNum += 1;
			levelLabel.Text = $"Level: {GetCurrentLevel()}";
            experienceBar.Value = experienceForNextLevel;
            experience = experienceForNextLevel;
            CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.UpdateLevel);
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

    private void LevelGainedAnimation()
    {
        levelUpAnimation.Play();
        levelUpAnimation2.Play();
        levelUpAnimation3.Play();

    }

    public override void _Notification(int what)
    {
		if (what == NotificationSceneInstantiated) 
		{
			Instance = this;
		}
    }
}
