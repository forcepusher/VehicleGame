using Igrushka.VehicleGame;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CompositeControls : IControls
{
    private readonly List<IControls> _controls;

    public CompositeControls(IEnumerable<IControls> controls)
    {
        _controls = controls.ToList();
    }

    public float Throttle => Mathf.Clamp(_controls.Sum(c => c.Throttle), -1f, 1f);
    public float Turn => Mathf.Clamp(_controls.Sum(c => c.Turn), -1f, 1f);
    public float Pitch => Mathf.Clamp(_controls.Sum(c => c.Pitch), -1f, 1f);
    public float Yaw => Mathf.Clamp(_controls.Sum(c => c.Yaw), -1f, 1f);

    public void Update()
    {
        foreach (var control in _controls)
            control.Update();
    }
}
