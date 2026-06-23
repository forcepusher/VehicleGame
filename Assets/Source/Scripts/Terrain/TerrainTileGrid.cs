using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public static class TerrainTileGrid
    {
        public const float TileSize = 4000f;
        public const float OriginX = -10000f;
        public const float OriginZ = -10000f;

        public static TileCoordinate WorldToTile(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition.x - OriginX) / TileSize);
            int y = Mathf.FloorToInt((worldPosition.z - OriginZ) / TileSize);
            return new TileCoordinate(x, y);
        }

        public static TileCoordinate GetAnchor(Vector3 worldPosition)
        {
            TileCoordinate playerTile = WorldToTile(worldPosition);
            float tileCenterX = OriginX + (playerTile.X + 0.5f) * TileSize;
            float tileCenterZ = OriginZ + (playerTile.Y + 0.5f) * TileSize;

            int westColumnX = worldPosition.x < tileCenterX ? playerTile.X - 1 : playerTile.X;
            int northRowY = worldPosition.z > tileCenterZ ? playerTile.Y + 1 : playerTile.Y;

            return new TileCoordinate(westColumnX, northRowY);
        }

        public static IEnumerable<TileCoordinate> GetWindow(TileCoordinate northWest)
        {
            yield return northWest;
            yield return new TileCoordinate(northWest.X + 1, northWest.Y);
            yield return new TileCoordinate(northWest.X, northWest.Y - 1);
            yield return new TileCoordinate(northWest.X + 1, northWest.Y - 1);
        }
    }
}
