using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class GameState : MonoBehaviour
    {
        private readonly List<IPlayer> _players = new();

        private void Awake()
        {
            var spawnPoints = new List<ISpawnPoint>(FindObjectsByType<SpawnPoint>());
            var map = new Map(spawnPoints);

            _players.Add(new LocalPlayer(map));
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
