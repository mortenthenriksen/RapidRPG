using Game.Manager;
using Godot;

namespace Game.Inventory;

public partial class CharacterStats : MarginContainer
{
    
    private Label characterLevelLabel;


    private Label strengthLabel;
    private Label dexterityLabel;
    private Label intelligenceLabel;

    private Label totalDamageLabel;
    private Label defenseLabel;
    private Label currentMultiplierLabel;
    
    
    public override void _Ready()
    {
        characterLevelLabel = GetNode<Label>("%CharacterLevelLabel");

        strengthLabel = GetNode<Label>("%StrengthLabel");
        dexterityLabel = GetNode<Label>("%DexterityLabel");
        intelligenceLabel = GetNode<Label>("%IntelligenceLabel");

        totalDamageLabel = GetNode<Label>("%TotalDamageLabel");
        defenseLabel = GetNode<Label>("%DefenseLabel");
        currentMultiplierLabel = GetNode<Label>("%CurrentMultiplierLabel");
    }

    
    public override void _Process(double delta)
    {
        UpdateCurrentLevel();
        
        UpdateCurrentMainStats();

        UpdateCurrentDamageInfo();
        UpdateCurrentDefence();
        UpdateCurrentMultiplier();
    }
    
    private void UpdateCurrentMainStats()
    {
        strengthLabel.Text = MainStatManager.Instance.GetStrengthValue().ToString();
        dexterityLabel.Text = MainStatManager.Instance.GetDexterityValue().ToString();
        intelligenceLabel.Text = MainStatManager.Instance.GetIntelligenceValue().ToString();
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

    private void UpdateCurrentMultiplier()
    {
        currentMultiplierLabel.Text = MultiplierManager.Instance.GetMultiplier().ToString("F2");
    }

}   
