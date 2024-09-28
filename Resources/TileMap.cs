using System;
using System.Collections.Generic;
using Game.Characters;
using Godot;

namespace Game.Environment;

public partial class TileMap : Godot.TileMap
{
    [Export]
    public Dave player;

    // this might cause issues if / when the player gets more movement speed, maybe we just multiply it then xd
    private const int autoTileRange = 55;
    private const float updateInterval = 0.5f;
    private float timeSinceLastUpdate = 0f;

    private Random random = new Random();

    private HashSet<Vector2I> activeTiles = new();

    public override void _Ready()
    {
        UpdateTilesAroundPlayer();
    }

    public override void _Process(double delta)
    {
        timeSinceLastUpdate += (float)delta;
        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateTilesAroundPlayer();
            timeSinceLastUpdate = 0f; 
        }
    }

    private void UpdateTilesAroundPlayer()
    {
        Vector2I playerCell = LocalToMap(player.GlobalPosition);
        HashSet<Vector2I> newActiveTiles = new();

        for (int x = -autoTileRange; x <= autoTileRange; x++) 
        {
            for (int y = -autoTileRange + 20; y <= autoTileRange - 20; y++)
            {
                Vector2I cellPosition = playerCell + new Vector2I(x, y);
                newActiveTiles.Add(cellPosition);
                if (GetCellTileData(0, cellPosition) == null)
                {
                    if (random.NextDouble() > 0.90)
                    {
                        var atlasCoordsPosRandomX = random.Next(2, 4);
                        var atlasCoordsPosRandomY = random.Next(3, 5);
                        SetCell(0, cellPosition, 1, new Vector2I(atlasCoordsPosRandomX, atlasCoordsPosRandomY));
                    }
                    else
                    {
                        SetCell(0, cellPosition, 1, new Vector2I(6, 6));
                    }
                }
            }
        }

        foreach (Vector2I cell in activeTiles)
        {
            if (!newActiveTiles.Contains(cell))
            {
                SetCell(0, cell, -1);
            }
        }

        activeTiles = newActiveTiles;
    }





    // private void TestOfTiles()
    // {
    //     for (int i = 0; i < 10; i++ )
    //     {
    //         var atlasCoordsPosRandomX = random.Next(2, 4);
    //         var atlasCoordsPosRandomY = random.Next(3, 5);
    //         GD.Print((int)atlasCoordsPosRandomX, (int)atlasCoordsPosRandomY);
    //     }
    // }
}
