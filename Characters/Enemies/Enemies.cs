using Godot; 
using System;

public abstract partial class Enemies : CharacterBody2D
{
    



    private CustomSignals customSignals;


    public override void _Ready()
	{
		customSignals = GetNode<CustomSignals>("/root/CustomSignals");
	}
    
    
    
}