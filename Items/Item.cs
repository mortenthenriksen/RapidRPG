using Godot;
using System;

public partial class Item : Resource
{
    [Export]
    public int ID { get; set; }
    
    [Export]
    public string Name { get; set; }
    
    [Export]
    public string PathToResource { get; set; }
    
    [Export]
    public Texture2D Icon { get; set; }

    [Export]
    public int Quantity { get; set; }
    
    [Export]
    public int StackSize { get; set; }
    
    [Export]
    public bool IsStackable { get; set; }

    public Item Copy() => Duplicate() as Item;
}

