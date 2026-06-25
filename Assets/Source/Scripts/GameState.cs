using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class GameState : MonoBehaviour
    {
        private readonly List<IPlayer> _players = new();

        private void Awake()
        {
            _players.Add(new LocalPlayer());
        }
    }
}
