using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class VehicleSwitch : MonoBehaviour
    {
        [SerializeField]
        List<IVehicle> _vehicles;

        private void Update()
        {
            
        }

        //new CompositeControls(new IControls[] { new KeyboardControls(), new GamepadControls() });
    }
}
