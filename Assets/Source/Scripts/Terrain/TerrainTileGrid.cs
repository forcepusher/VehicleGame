using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public static class TerrainTileGrid
    {
        public const float TileSize = 4000f;
        public const float OriginX = -10000f;
        public const float OriginZMax = 10000f;

        public static TileCoordinate WorldToTile(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition.x - OriginX) / TileSize);
            int y = Mathf.FloorToInt((OriginZMax - worldPosition.z) / TileSize);
            return new TileCoordinate(x, y);
        }

        public static TileCoordinate GetAnchor(Vector3 worldPosition)
        {
            TileCoordinate playerTile = WorldToTile(worldPosition);
            Vector2 local = GetLocalPositionInTile(worldPosition, playerTile);
            float half = TileSize * 0.5f;

            int anchorX = local.x < half ? playerTile.X - 1 : playerTile.X;
            int anchorY = local.y < half ? playerTile.Y - 1 : playerTile.Y;

            return new TileCoordinate(anchorX, anchorY);
        }

        public static Vector2 GetLocalPositionInTile(Vector3 worldPosition, TileCoordinate tile)
        {
            float minX = OriginX + tile.X * TileSize;
            float minZ = OriginZMax - (tile.Y + 1) * TileSize;
            return new Vector2(worldPosition.x - minX, worldPosition.z - minZ);
        }

        public static IEnumerable<TileCoordinate> GetWindow(TileCoordinate anchor)
        {
            yield return anchor;
            yield return new TileCoordinate(anchor.X + 1, anchor.Y);
            yield return new TileCoordinate(anchor.X, anchor.Y + 1);
            yield return new TileCoordinate(anchor.X + 1, anchor.Y + 1);
        }
    }
}
