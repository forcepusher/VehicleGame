using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class GameState : MonoBehaviour
    {
        [SerializeField]
        private StreamingTerrain _streamingTerrain;

        private readonly List<IPlayer> _players = new();

        private IEnumerator Start()
        {
            while (!_streamingTerrain.AllRequiredTilesLoaded)
                yield return null;

            var spawnPoints = new List<ISpawnPoint>(FindObjectsByType<SpawnPoint>());
            var map = new Map(spawnPoints);

            var localPlayer = new LocalPlayer(map);
            _players.Add(localPlayer);
        }

        private void Update()
        {
            foreach (var player in _players)
            {
                player.ManualUpdate();
            }
        }
    }
}
