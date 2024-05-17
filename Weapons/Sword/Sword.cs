// using Godot;
// using System;


// public partial class Sword : Area2D
// {

//     private AnimatedSprite2D animatedSprite2D;
//     private Dave player; 
//     private Area2D sword;
//     private Vector2 direction;
//     private bool isSwinging = false;
//     private float swingCooldown = 0.3f;
//     private float swingCooldownTimer = 0.0f; 
//     private bool upRight;
//     private bool downRight;
//     private bool upLeft;
//     private bool downLeft;

//     public override void _Ready()
//     {
//         animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
//         sword = GetNode<Area2D>("/root/Game/Dave/Sword");
//         player = GetNode<Dave>("/root/Game/Dave");
//     }

//     public void Swing()
//     {
//         if (swingCooldownTimer <= 0.0f)
//         {
//             isSwinging = true;
//             PlayAnimation(direction);
//             swingCooldownTimer = swingCooldown;
//             foreach (var HurtBox in sword.GetOverlappingBodies())
//             {
//                 if (HurtBox is Orc orc)
//                 {
//                     orc.TakeDamage();
//                 }
//             }
//         }
//     }

//     public void StopSwing()
//     {
//         isSwinging = false;
//         animatedSprite2D.Play("idle");
//     }

//     public override void _Process(double delta)
//     {
//         direction = player.GetCurrentDirection();
//         this.upRight = direction == Vector2.Up || direction == Vector2.Up + Vector2.Right;
//         this.downRight = direction == Vector2.Right || direction == Vector2.Right + Vector2.Down;
//         this.upLeft = direction == Vector2.Left || direction == Vector2.Left + Vector2.Up;
//         this.downLeft = direction == Vector2.Down || direction == Vector2.Down + Vector2.Left;

//         if (swingCooldownTimer > 0.0f)
//         {
//             swingCooldownTimer -= (float) delta;
//         }

//         if (Input.IsActionPressed(InputConstants.ATTACK)) {
//             Swing();
//         } 

//         else 
//         {
//             StopSwing();
//         }
//     }


//     private void PlayAnimation(Vector2 direction)
//     {   
//         if (upRight)
//         {
//             animatedSprite2D.Play("slashUpRight");
//         }
//         else if (downRight)
//         {
//             animatedSprite2D.Play("slashDownRight");
//         }
//         else if (upLeft)
//         {
//             animatedSprite2D.Play("slashUpLeft");
//         }
//         else if (downLeft)
//         {
//             animatedSprite2D.Play("slashDownLeft");
//         }
//     }

//     private void RotateSword() 
//     {
//         if (upRight)
//         {
//             RotationDegrees = 180; // Rotate the sword to the upper right
//         }
//         else if (downRight)
//         {
//             RotationDegrees = 270; // Rotate the sword to the lower right
//         }
//         else if (upLeft)
//         {
//             RotationDegrees = 90; // Rotate the sword to the upper left
//         }
//         else if (downLeft)
//         {
//             RotationDegrees = 0; // Rotate the sword to the lower left
//         }
//     }  
// }


