using Godot;

namespace Game.Weapons;

public partial class AttackBoxCollisionShape : CollisionShape2D
{
    public void RotateAttackBox(Vector2 direction)
    {   
        if (direction == Vector2.Up)
        {
            Position = new Vector2(0, -60);
            RotationDegrees = 90;
        }
        if (direction == Vector2.Down)
        {
            Position = new Vector2(0, 40);
            RotationDegrees = 90;
        }
        if (direction == Vector2.Left)
        {
            Position = new Vector2(-50, -10);
            RotationDegrees = 0;
        }
        if (direction == Vector2.Right)
        {
            Position = new Vector2(50, -10);
            RotationDegrees = 0;
        }
    }
}
