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

        public static TileCoordinate GetAnchor(TileCoordinate playerTile)
        {
            return new TileCoordinate(playerTile.X / 2 * 2, playerTile.Y / 2 * 2);
        }

        public static IEnumerable<TileCoordinate> GetWindow(TileCoordinate anchor)
        {
            yield return anchor;
            yield return new TileCoordinate(anchor.X + 1, anchor.Y);
            yield return new TileCoordinate(anchor.X, anchor.Y + 1);
            yield return new TileCoordinate(anchor.X + 1, anchor.Y + 1);
        }

        public static void AddCenterPreloadTiles(
            HashSet<TileCoordinate> targets,
            TileCoordinate anchor,
            TileCoordinate playerTile,
            Vector3 worldPosition)
        {
            float minX = OriginX + playerTile.X * TileSize;
            float minZ = OriginZMax - (playerTile.Y + 1) * TileSize;
            float localX = worldPosition.x - minX;
            float localZ = worldPosition.z - minZ;
            float half = TileSize * 0.5f;

            if (playerTile.X == anchor.X + 1 && localX > half)
            {
                targets.Add(new TileCoordinate(anchor.X + 2, anchor.Y));
                targets.Add(new TileCoordinate(anchor.X + 2, anchor.Y + 1));
            }
            else if (playerTile.X == anchor.X && localX < half)
            {
                targets.Add(new TileCoordinate(anchor.X - 1, anchor.Y));
                targets.Add(new TileCoordinate(anchor.X - 1, anchor.Y + 1));
            }

            if (playerTile.Y == anchor.Y && localZ > half)
            {
                targets.Add(new TileCoordinate(anchor.X, anchor.Y - 1));
                targets.Add(new TileCoordinate(anchor.X + 1, anchor.Y - 1));
            }
            else if (playerTile.Y == anchor.Y + 1 && localZ < half)
            {
                targets.Add(new TileCoordinate(anchor.X, anchor.Y + 2));
                targets.Add(new TileCoordinate(anchor.X + 1, anchor.Y + 2));
            }
        }
    }
}
