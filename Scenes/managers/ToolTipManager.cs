using System;
using System.Linq;
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

    private Tween descriptionTween;
    private readonly float PULSE_DURATION = 1.5f;
    private readonly float PULSE_SCALE_MIN = 0.98f;
    private readonly float PULSE_SCALE_MAX = 1.02f;

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
        
        StartUniqueLabelEffect();
    }

    public override void _Process(double delta)
    {

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
                mainStatLabel.Text = "Armour: ";
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
                if (valueDict.ContainsKey("UniqueEffect"))
                {
                    // GD.Print(valueDict["UniqueEffect"].ToString().Length);
                    descriptionLabel.Text = SplitTextIntoLines(valueDict["UniqueEffect"].ToString(), 30);
                    StartUniqueLabelEffect(); // Start pulsing when showing unique effect
                }
                else
                {
                    if (descriptionTween != null)
                    {
                        descriptionTween.Kill(); // Stop pulsing when no unique effect
                    }
                    descriptionLabel.Scale = Vector2.One; // Reset scale
                    descriptionLabel.Text = "";
                }
            }

            if (valueDict["ItemCategory"].ToString() == "Weapon")
            {
                mainStatLabel.Text = "Attack damage: ";
                mainStatValue.Text = valueDict["Attack"].ToString();
                secondaryStatLabel.Text = "Attack speed: ";
                secondaryStatValue.Text = valueDict["AttackSpeed"].ToString();
                if (valueDict.ContainsKey("UniqueEffect"))
                {
                    // GD.Print(valueDict["UniqueEffect"].ToString().Length);
                    descriptionLabel.Text = SplitTextIntoLines(valueDict["UniqueEffect"].ToString(), 30);
                }
                else 
                {
                    descriptionLabel.Text = "";
                }
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

    // chat is op B-)
    private string SplitTextIntoLines(string text, int lineLength)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var words = text.Split(' ');
        var result = new System.Text.StringBuilder();
        var currentLine = new System.Text.StringBuilder();

        foreach (var word in words)
        {
            // Check if adding this word would exceed the line length
            if (currentLine.Length + word.Length + 1 > lineLength)
            {
                // Add current line to result and start a new line
                if (currentLine.Length > 0)
                {
                    result.AppendLine(currentLine.ToString().Trim());
                    currentLine.Clear();
                }

                // If the word itself is longer than lineLength, split it
                if (word.Length > lineLength)
                {
                    int index = 0;
                    while (index < word.Length)
                    {
                        int length = Math.Min(lineLength, word.Length - index);
                        result.AppendLine(word.Substring(index, length));
                        index += length;
                    }
                }
                else
                {
                    currentLine.Append(word);
                }
            }
            else
            {
                // Add word to current line
                if (currentLine.Length > 0)
                    currentLine.Append(' ');
                currentLine.Append(word);
            }
        }

        // Add the last line if there's anything left
        if (currentLine.Length > 0)
            result.AppendLine(currentLine.ToString().Trim());

        return result.ToString();
    }

    private void OnMouseExited()
    {   
        if (descriptionTween != null)
        {
            descriptionTween.Kill(); // Stop pulsing when tooltip is hidden
        }
        descriptionLabel.Scale = Vector2.One; // Reset scale
        toolTipPanel.Visible = false;
    }

    private Dictionary GetItemDataDictionary()
    {
        return GDToCSDataConverter.Instance.GetValuesDictionaries();
    }

    private void StartUniqueLabelEffect()
    {
        if (descriptionTween != null)
        {
            descriptionTween.Kill();
        }

        descriptionTween = CreateTween();
        descriptionTween.SetLoops();

        // Cycle through colors
        descriptionTween.TweenProperty(
            descriptionLabel,
            "modulate",
            new Color(1, 0.5f, 0.5f), // Red tint
            1.0f
        ).SetTrans(Tween.TransitionType.Sine);

        descriptionTween.TweenProperty(
            descriptionLabel,
            "modulate",
            new Color(0.5f, 1, 0.5f), // Green tint
            1.0f
        ).SetTrans(Tween.TransitionType.Sine);

        descriptionTween.TweenProperty(
            descriptionLabel,
            "modulate",
            new Color(0.5f, 0.5f, 1), // Blue tint
            1.0f
        ).SetTrans(Tween.TransitionType.Sine);

        descriptionTween.TweenProperty(
            descriptionLabel,
            "modulate",
            Colors.White, // Back to normal
            1.0f
        ).SetTrans(Tween.TransitionType.Sine);
    }


    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }
}
