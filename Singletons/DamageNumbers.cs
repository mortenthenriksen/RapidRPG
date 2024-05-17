using Godot;
using System;

public partial class DamageNumbers : Node
{

    private bool isCritial = false;
    private string color;

    public void DisplayNumber(float damageAmount, Vector2 position, bool isCritial) {

        var number = new Label
        {
            GlobalPosition = position,
            Text = damageAmount.ToString(),
            ZIndex = 5,
            LabelSettings = new LabelSettings()
        };

        color = "#FFF";
        if (isCritial) {
            color = "#B22";
        } 

        AddChild(number);

        var tween = GetTree().CreateTween();
    
        tween.SetParallel(true);
        tween.TweenProperty(
            number, "position:y", number.Position.Y -24, 0.25
        ).SetEase(Tween.EaseType.Out);

        tween.TweenProperty(
            number, "position:y", number.Position.Y, 0.5
        ).SetEase(Tween.EaseType.In).SetDelay(0.25);

        tween.TweenProperty(
            number, "scale", Vector2.Zero, 0.25
        ).SetEase(Tween.EaseType.In).SetDelay(0.5);

        tween.Finished += () => {
            number.QueueFree();
        };
    }
}
