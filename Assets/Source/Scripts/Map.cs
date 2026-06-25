using System.Collections.Generic;

namespace BananaParty.VehicleGame
{
    public class Map
    {
        List<ISpawnPoint> _spawnPoints;

        public Map(List<ISpawnPoint> spawnPoints)
        {
            _spawnPoints = spawnPoints;
        }
    }
}
