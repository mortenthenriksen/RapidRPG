using System;
using Game.Characters;
using Godot;

namespace Game.UI;

public partial class SkillBar : Control
{
    
    [Export]
    private Dave player;

    private Label cooldownDashLabel;
    private Label cooldownSpecialLabel;

    private TextureRect textureRectSpecial;
    private TextureRect textureRectDash;

    private Color offCooldown = new Color("ffffff");
    private Color onCooldown = new Color("ffffff69");

    public override void _Ready()
    {
        cooldownDashLabel = GetNode<Label>("%CooldownDashLabel");
        cooldownSpecialLabel = GetNode<Label>("%CooldownSpecialLabel");

        textureRectSpecial = GetNode<TextureRect>("%TextureRectSpecial");
        textureRectDash = GetNode<TextureRect>("%TextureRectDash");
    }

    public override void _Process(double delta)
    {
        DisplayCoolDownDashTimer();
        DisplayCooldownSpecialTimer();
    }

    private void DisplayCooldownSpecialTimer()
    {
        if (player.GetIsSpecialOnCooldown())
        {
            cooldownSpecialLabel.Visible = true;
            textureRectSpecial.Modulate = onCooldown;
            var timeLeft = Math.Round(player.GetSpecialTimer().TimeLeft);
            cooldownSpecialLabel.Text = timeLeft.ToString();

            if (timeLeft == 0)
            {
                cooldownSpecialLabel.Visible = false;
                textureRectSpecial.Modulate = offCooldown;
            }
        }
    }

    private void DisplayCoolDownDashTimer()
    {
        if (player.GetIsDashOnCooldown())
        {
            cooldownDashLabel.Visible = true;
            textureRectDash.Modulate = onCooldown;
            var timeLeft = Math.Round(player.GetDashTimer().TimeLeft);
            cooldownDashLabel.Text = timeLeft.ToString();

            if (timeLeft == 0)
            {
                cooldownDashLabel.Visible = false;
                textureRectDash.Modulate = offCooldown;
            }
        }
    }
}
