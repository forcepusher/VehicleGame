using System;

namespace BananaParty.VehicleGame
{
    public struct TileCoordinate : IEquatable<TileCoordinate>
    {
        public int X;
        public int Y;

        public TileCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public string SceneName => $"x{X}_y{Y}";

        public bool Equals(TileCoordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is TileCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return SceneName;
        }
    }
}
