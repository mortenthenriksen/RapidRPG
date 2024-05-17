using Godot;
using System;

public partial class CustomSignals : Node
{
    [Signal]
    public delegate void DamagePlayerEventHandler(float damageAmount);
    
    [Signal]
	public delegate void UpdateHealthEventHandler(float health);

    [Signal]
    public delegate void DamageReceivedEventHandler(float damageAmount);

    [Signal]
	public delegate void UpdateExperienceBarEventHandler(float experience);

    [Signal]
	public delegate void UpdateLevelEventHandler();
    
	[Signal]
	public delegate void HealthDepletedEventHandler(float health); 

    [Signal]
	public delegate void HealthDepletedEnemyEventHandler(float health);   
}
