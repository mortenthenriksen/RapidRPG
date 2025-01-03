using Godot;

namespace Game.Environment;

public partial class roundTree : StaticBody2D
{
    private float maxDistance = 800; // Set this to the maximum distance you want

    public override void _Ready()
    {
        // player = GetTree().Root.GetNode<Dave>("/root/Main/Dave");
    }

    public override void _Process(double delta)
    {

    }
}