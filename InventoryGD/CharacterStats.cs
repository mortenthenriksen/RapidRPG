using Game.Manager;
using Godot;

namespace Game.Inventory;

public partial class CharacterStats : MarginContainer
{
    
    private Label characterLevelLabel;
    private Label totalDamageLabel;
    private Label defenseLabel;
    private Label currentMultiplierLabel;
    
    
    public override void _Ready()
    {
        characterLevelLabel = GetNode<Label>("%CharacterLevelLabel");
        totalDamageLabel = GetNode<Label>("%TotalDamageLabel");
        defenseLabel = GetNode<Label>("%DefenseLabel");
        currentMultiplierLabel = GetNode<Label>("%CurrentMultiplierLabel");
    }

    

    public override void _Process(double delta)
    {
        UpdateCurrentDamageInfo();
        UpdateCurrentLevel();
        UpdateCurrentDefence();
    }



    private void UpdateCurrentDamageInfo()
    {
        totalDamageLabel.Text = DamageManager.Instance.TotalDamageAmount().ToString();
    }

    private void UpdateCurrentLevel()
    {
        characterLevelLabel.Text = LevelManager.Instance.GetCurrentLevel().ToString(); 
    }

    private void UpdateCurrentDefence()
    {
        defenseLabel.Text = DefenseManager.Instance.GetTotalDefence().ToString();
    }

}   
