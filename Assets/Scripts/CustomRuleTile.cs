using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Create Custom Rule Tile")]
public class CustomTile : RuleTile<CustomTile.Neighbor>
{
    public bool customField;

    public List<TileBase> Siblings = new List<TileBase>(); // Shared list of compatible tiles

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int OtherGroup = 4;
        public const int SameGroup = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.This:
                return tile == this || Siblings.Contains(tile);
            case Neighbor.NotThis:
                return tile != this && !Siblings.Contains(tile);
            case Neighbor.OtherGroup:
                return Siblings.Contains(tile);
            case Neighbor.SameGroup:
                return !Siblings.Contains(tile);
        }

        return base.RuleMatch(neighbor, tile);
    }



}

