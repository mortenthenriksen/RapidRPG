using Godot;
using System;

public partial class HealthLabel : Label
{
    private Dave player;

    public override void _Ready()
    {
        player = GetNode<Dave>("/root/Game/Dave");
        float health = player.GetHealth();
        Text = $"Health: {health}";
    }


    private void OnDaveUpdateHealth(float health) {
        Text = $"Health: {health}";
    }
}
