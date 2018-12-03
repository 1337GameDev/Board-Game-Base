using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : IBoardTile
{
    /// <summary>
    /// The object of this tile.
    /// </summary>
    public GameObject TileObj { get; set; }

    /// <summary>
    /// The attributes and values for this tile
    /// </summary>
    public Dictionary<string, float> Attributes { get; set; }


}
