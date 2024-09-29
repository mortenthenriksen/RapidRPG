using System;
using Game.Autoload;
using Godot;
using Godot.Collections;

namespace Game.Manager;

[GlobalClass]

public partial class DefenseManager : Node
{
    public static DefenseManager Instance { get; private set; }

    [Export]
    private GridContainer equippedSlots;

    private Array<string> equippedItems;
    
    private float totalDefense;


    public override void _Ready()
    {
        equippedSlots.Connect("slot_interacted", Callable.From(OnSlotInteracted));
    }

    public override void _Process(double delta)
    {        
        // GetDefenceFromEquippedItems();
    }

    private void OnSlotInteracted()
    {
        GetDefenceFromEquippedItems();
    }

    public float GetDefenceFromEquippedItems()
    {
        totalDefense = 0;
        equippedItems = (Array<string>)equippedSlots.Call("get_equipped_items");
        var itemDataJson = GetItemDataDictionary();
        foreach (var element in equippedItems)
        {
            if (GetItemDataDictionary().ContainsKey(element))
            {
                if (itemDataJson.ContainsKey(element))
                {
                    var valueDict = (Dictionary)itemDataJson[element];
                    if (valueDict.ContainsKey("Defense"))
                    {
                        totalDefense += (float)valueDict["Defense"];
                    }
                }
            }
        }
        return totalDefense;
    }

    private Dictionary GetItemDataDictionary()
    {
        return GDToCSDataConverter.Instance.GetValuesDictionaries();
    }

    public float GetTotalDefence()
    {
        return totalDefense;
    }
    
    		
    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }
}

