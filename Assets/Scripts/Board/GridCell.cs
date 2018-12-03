using System;
using System.Collections.Generic;
using UnityEngine;

public class GridCell {

    /// <summary>
    /// The position of this grid cell in world coordinates
    /// </summary>
    public Vector3 WorldPosition { get; set; }
    public Vector2 GridPosition { get; set; }

    public IBoardTile Tile { get; set; }
    public List<GameObject> Objects { get; set; }

}
