using Godot;

namespace Game.Characters;

public abstract partial class IEnemies : CharacterBody2D
{
    public abstract void TakeDamage(float damageAmount);
}
