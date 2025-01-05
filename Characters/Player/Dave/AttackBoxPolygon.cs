using Godot;
using System;

public partial class AttackBoxPolygon : CollisionPolygon2D
{
    public void RotateAttackBox(Vector2 direction)
    {   

        if (direction == Vector2.Up)
        {
            RotationDegrees = -180;
        }
        else if (direction == Vector2.Down)
        {
            RotationDegrees = 0;
        }
        else if (direction == Vector2.Left)
        {
            RotationDegrees = 90;
        }
        else if (direction == Vector2.Right)
        {
            RotationDegrees = -90;
        }
        else if (direction == new Vector2(1, -1).Normalized()) // NE
        {
            RotationDegrees = -135;
        }
        else if (direction == new Vector2(1, 1).Normalized()) // SE
        {
            RotationDegrees = -45;
        }
        else if (direction == new Vector2(-1, 1).Normalized()) // SW
        {
            RotationDegrees = 45;
        }
        else if (direction == new Vector2(-1, -1).Normalized()) // NW
        {
            RotationDegrees = 135;
        }
    }
}
