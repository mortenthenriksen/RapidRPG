using System;
using Godot;

[GlobalClass]

public partial class DamageNumbers : Node
{

    [Export]
    public Theme pixelKubastaFontTheme;


    private bool isCritial = false;
    private string color;

    public void DisplayNumber(float damageAmount, Vector2 position, bool isCritial) {

        var number = new Label
        {
            GlobalPosition = position + new Vector2(0, -40),
            Text = damageAmount.ToString(),
            ZIndex = 5,
        };

        number.Theme = pixelKubastaFontTheme;
        number.AddThemeFontSizeOverride("font_size", 24);

        color = "#FFF";
        if (isCritial) {
            color = "#B22";
        } 

        AddChild(number);

        Random random = new Random();
        float horizontalOffset = (float)(random.NextDouble() * 2 - 1) * 20;

        var tween = GetTree().CreateTween();
    
        tween.SetParallel(true);
        tween
            .TweenProperty(number, "position", new Vector2(number.Position.X + horizontalOffset, (float)(number.Position.Y - 24)), 0.25)
            .SetEase(Tween.EaseType.Out);

        tween
            .TweenProperty(number, "position", new Vector2(number.Position.X + horizontalOffset, (float)number.Position.Y), 0.5)
            .SetEase(Tween.EaseType.In).SetDelay(0.25);

        tween
            .TweenProperty(number, "scale", Vector2.Zero, 0.25)
            .SetEase(Tween.EaseType.In).SetDelay(0.5);



        tween.Finished += () => {
            number.QueueFree();
        };
    }
}
