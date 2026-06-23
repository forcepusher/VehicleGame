using UnityEngine;
using UnityEngine.SceneManagement;

namespace BananaParty.VehicleGame
{
    public class TileSceneEntry
    {
        public TileCoordinate Coordinate;
        public AssetBundle Bundle;
        public Scene Scene;
        public bool IsLoaded;
        public bool IsLoading;
        public float UnloadAfterTime;
    }
}
