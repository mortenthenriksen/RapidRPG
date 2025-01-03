using System;
using Game.Autoload;
using Game.Characters;
using Game.Projectiles;
using Godot;
using Godot.Collections;

namespace Game.Manager;


public partial class EquippedItemsManager : Node
{
    public static EquippedItemsManager Instance { get; private set; }

    [Export]
    private GridContainer equippedSlots;

    [Export]
    private PackedScene fireProjectileScene;

    [Export]
    private Dave player;

    private Array<string> equippedItems;
    private Dictionary<string, string> uniqueEffects = new Dictionary<string, string>();
    private Dictionary itemDataJson;
    private string uniqueEffectFromWeapon;
    
    private float baseDefense;
    private float attackSpeed;
    private float addedDamage;


    public override void _Ready()
    {
        equippedSlots.Connect("slot_interacted", Callable.From(OnSlotInteracted));
        itemDataJson = GetItemDataDictionary();
    }


    public override void _Process(double delta)
    {        
        // GetDefenceFromEquippedItems();
    }

    private void OnSlotInteracted()
    {
        GetDefenceFromEquippedItems();
        GetDamageFromEquippedItems();
        GetAttackSpeedFromEquippedItems();
        GetUniqueEffectsFromEquippedItems();
    }

    private float GetDefenceFromEquippedItems()
    {
        baseDefense = 0;
        equippedItems = (Array<string>)equippedSlots.Call("get_equipped_items");
        foreach (var element in equippedItems)
        {
            if (GetItemDataDictionary().ContainsKey(element))
            {
                if (itemDataJson.ContainsKey(element))
                {
                    var valueDict = (Dictionary)itemDataJson[element];
                    if (valueDict.ContainsKey("Defense"))
                    {
                        baseDefense += (float)valueDict["Defense"];
                    }
                }
            }
        }
        return baseDefense;
    }

    private float GetDamageFromEquippedItems()
    {
        addedDamage = 0;
        equippedItems = (Array<string>)equippedSlots.Call("get_equipped_items");
        foreach (var element in equippedItems)
        {
            if (GetItemDataDictionary().ContainsKey(element))
            {
                if (itemDataJson.ContainsKey(element))
                {
                    var valueDict = (Dictionary)itemDataJson[element];
                    if (valueDict.ContainsKey("Attack"))
                    {
                        addedDamage += (float)valueDict["Attack"];
                    }
                }
            }
        }
        return addedDamage;
    }


    public float GetAttackSpeedFromEquippedItems()
    {
        attackSpeed = 0;
        equippedItems = (Array<string>)equippedSlots.Call("get_equipped_items");
        foreach (var element in equippedItems)
        {
            if (GetItemDataDictionary().ContainsKey(element))
            {
                if (itemDataJson.ContainsKey(element))
                {
                    var valueDict = (Dictionary)itemDataJson[element];
                    if (valueDict.ContainsKey("AttackSpeed"))
                    {
                        attackSpeed += (float)valueDict["AttackSpeed"];
                    }
                }
            }
        }

        return attackSpeed;
    }

    private Dictionary<string, string> GetUniqueEffectsFromEquippedItems()
    {
        uniqueEffects.Clear();
        equippedItems = (Array<string>)equippedSlots.Call("get_equipped_items");
        foreach (var element in equippedItems)
        {
            if (GetItemDataDictionary().ContainsKey(element))
            {
                if (itemDataJson.ContainsKey(element))
                {
                    var valueDict = (Dictionary)itemDataJson[element];
                    if (valueDict.ContainsKey("UniqueEffect"))
                    {
                        var value = valueDict["UniqueEffect"].ToString();
                        var key = valueDict["ItemCategory"].ToString();
                        uniqueEffects.Add(key, value);
                    }
                }
            }
        }
        return uniqueEffects;
    }

    public void FindUniqueEffectForWeapon()
    {   
        if (uniqueEffects.ContainsKey("Weapon"))
        {
            uniqueEffectFromWeapon = uniqueEffects["Weapon"];
            if (uniqueEffectFromWeapon == "Adds a flaming projectile to your basic attacks")
            {
                var fireProjectile = fireProjectileScene.Instantiate() as Area2D;
                fireProjectile.GlobalPosition = player.Position;
                var angle = player.GetCurrentDirection().Angle();
                fireProjectile.Rotation = angle;
                GetTree().Root.CallDeferred("add_child", fireProjectile);                
            }
        }

    }


    private Dictionary GetItemDataDictionary()
    {
        return GDToCSDataConverter.Instance.GetValuesDictionaries();
    }

    public float GetTotalDefence()
    {
        return baseDefense;
    }

    public float GetAddedDamage()
    {
        return addedDamage;
    }
    
    public float GetTotalAttackSpeed()
    {
        if (attackSpeed == 0)
        {
            return 1f;
        }
        return attackSpeed;
    }
    		
    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }
}

