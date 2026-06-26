using System.Collections.Generic;

namespace BananaParty.VehicleGame
{
    public class Map
    {
        public readonly List<ISpawnPoint> SpawnPoints;

        public Map(List<ISpawnPoint> spawnPoints)
        {
            SpawnPoints = spawnPoints;
        }
    }
}
