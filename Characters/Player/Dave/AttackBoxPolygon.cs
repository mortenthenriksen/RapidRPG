using Godot;
using System;

public partial class AttackBoxPolygon : CollisionPolygon2D
{
    public void RotateAttackBox(Vector2 direction)
    {
        // Calculate angle in radians (-π to π)
        float angle = Mathf.Atan2(direction.Y, direction.X);
        
        // Convert to degrees and shift range to 0 to 360
        float degrees = Mathf.RadToDeg(angle);
        if (degrees < 0)
            degrees += 360;

        // Lock rotation to 4 quadrants
        float lockedRotation = degrees switch
        {
            >= 315 or < 45 => 0,     // Right
            >= 45 and < 135 => 90,   // Down
            >= 135 and < 225 => 180, // Left
            >= 225 and < 315 => 270, // Up
            _ => 0
        };

        // - 90 to adjust for the initial position
        RotationDegrees = lockedRotation - 90;
    }
}

