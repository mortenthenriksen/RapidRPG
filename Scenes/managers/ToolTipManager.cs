using Game.Autoload;
using Game.Characters;
using Godot;
using Godot.Collections;

namespace Game.Manager;


public partial class ToolTipManager : Node2D
{
    public static ToolTipManager Instance { get; private set; }
    
    [Export]
    private GridContainer inventorySlots;

    [Export]
    private GridContainer equippedSlots;


    [Export]
    private Dave player;

    private Panel toolTipPanel;
    private Label itemNameLabel;
    private Label mainStatLabel;
    private Label mainStatValue;
    private Label secondaryStatLabel;
    private Label secondaryStatValue;
    private Label descriptionLabel;

    private Timer toolTipDelayTimer;
    private Vector2I offset = new Vector2I(200, 200);

    public override void _Ready()
    {
        // to connect to a GDScript that has an argument, has to be passed like this apparently :))
        equippedSlots.Connect("custom_mouse_entered", Callable.From((string itemName) => OnMouseEntered(itemName)));
        equippedSlots.Connect("custom_mouse_exited", Callable.From(OnMouseExited));

        inventorySlots.Connect("custom_mouse_entered", Callable.From((string itemName) => OnMouseEntered(itemName)));
        inventorySlots.Connect("custom_mouse_exited", Callable.From(OnMouseExited));
        
        
        toolTipPanel = GetNode<Panel>("%ToolTipPanel");
        toolTipDelayTimer = GetNode<Timer>("ToolTipDelayTimer");
        toolTipPanel.Visible = false;
        
        itemNameLabel = GetNode<Label>("%ItemNameLabel");
        mainStatLabel = GetNode<Label>("%MainStatLabel");
        mainStatValue = GetNode<Label>("%MainStatValue");
        secondaryStatLabel = GetNode<Label>("%SecondaryStatLabel");
        secondaryStatValue = GetNode<Label>("%SecondaryStatValue");
        descriptionLabel = GetNode<Label>("%DescriptionLabel");
    }

    public override void _Process(double delta)
    {
        Vector2 mouseGlobalPosition = GetGlobalMousePosition();
        Vector2 playerGlobalPosition = player.GlobalPosition;
        toolTipPanel.Position = mouseGlobalPosition - playerGlobalPosition + offset;
    }


    private void OnMouseEntered(string itemName)
    {   
        toolTipPanel.Visible = true;
        var itemData = GetItemDataDictionary();
        if (itemData.ContainsKey(itemName))
        {
            itemNameLabel.Text = itemName;
            var valueDict = (Dictionary)itemData[itemName];
            if (valueDict.ContainsKey("Defense"))
            {
                mainStatLabel.Text = "Defense: ";
                mainStatValue.Text = valueDict["Defense"].ToString();
                if (valueDict.ContainsKey("MainStat"))
                {
                    secondaryStatLabel.Text = "Strength: ";
                    secondaryStatValue.Text = valueDict["MainStat"].ToString();
                }
                else
                {
                    secondaryStatLabel.Text = "";
                    secondaryStatValue.Text = "";
                }
                descriptionLabel.Text = "";
            }

            if (valueDict["ItemCategory"].ToString() == "Weapon")
            {
                mainStatLabel.Text = "Attack damage: ";
                mainStatValue.Text = valueDict["Attack"].ToString();
                secondaryStatLabel.Text = "Attack speed: ";
                secondaryStatValue.Text = valueDict["AttackSpeed"].ToString();
                descriptionLabel.Text = "";
            }


            if (valueDict["ItemCategory"].ToString() == "Consumable")
            {
                mainStatLabel.Text = "Stacksize";
                mainStatValue.Text = valueDict["StackSize"].ToString();
                secondaryStatLabel.Text = valueDict["ItemCategory"].ToString();
                secondaryStatValue.Text = "";
                descriptionLabel.Text = valueDict["Description"].ToString();
            }
        }

        
        // // Check if "Iron Helmet" exists in the dictionary
        // if (godotDict.ContainsKey("Tree Branch"))
        // {
        //     var valueDict = (Godot.Collections.Dictionary)godotDict["Tree Branch"];
        //     // Check if "Defense" key exists in the value dictionary
        //     if (valueDict.ContainsKey("Defense"))
        //     {
        //         GD.Print(valueDict["Defense"]);
        //     }
        // }
    }

    private void OnMouseExited()
    {   
        toolTipPanel.Visible = false;
    }

    private Dictionary GetItemDataDictionary()
    {
        return GDToCSDataConverter.Instance.GetValuesDictionaries();
    }


    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }
}
