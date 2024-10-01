using Game.Autoload;
using Game.Characters;
using Godot;

namespace Game.Manager;

[GlobalClass]


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
        experience += 1000;
		CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.UpdateExperienceBar, experience);
			
		experienceBar.Value = experience % 100;
		if (experience % 100 == 0) 
		{
            LevelGainedAnimation();
			levelNum += 1;
			levelLabel.Text = $"Level: {GetCurrentLevel()}";
            CustomSignals.Instance.EmitSignal(CustomSignals.SignalName.UpdateLevel);
		}
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
