using Godot;
using System;

public partial class roundTree : StaticBody2D
{
    private Dave player;
    private float maxDistance = 800; // Set this to the maximum distance you want

    public override void _Ready()
    {
        player = GetTree().Root.GetNode<Dave>("/root/Game/Dave");
    }

    public override void _Process(double delta)
    {
        float distance = player.GlobalPosition.DistanceTo(GlobalPosition);

        if (distance > maxDistance) {
            QueueFree();
        }
    }
}