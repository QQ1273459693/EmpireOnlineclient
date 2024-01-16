using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TileMapExtensionMethods
{
    public static IEnumerable<GetTileData> GetAllTiles(this Tilemap tilemap)
    {
        // Note: Optionally call tilemap.CompressBounds() some time prior to this
        var bounds = tilemap.cellBounds;

        // loop over the bounds (from min to max) on both axes
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var cellPosition = new Vector3Int(x, y, 0);

                // get the sprite and tile object at the specified location
                var sprite = tilemap.GetSprite(cellPosition);
                var tile = tilemap.GetTile(cellPosition);

                // This is a sanity check that I've included to ensure we're only
                // looking at populated tiles. You can change this up!
                if (tile == null && sprite == null)
                {
                    continue;
                }

                // create a data-transfer-object to yield back
                var tileData = new GetTileData(x, y, sprite, tile);
                yield return tileData;
            }
        }
    }
}

public sealed class GetTileData
{
    public GetTileData(
        int x,
        int y,
        Sprite sprite,
        TileBase tile)
    {
        X = x;
        Y = y;
        Sprite = sprite;
        Tile = tile;
    }

    public int X { get; }

    public int Y { get; }

    public Sprite Sprite { get; }

    public TileBase Tile { get; }
}